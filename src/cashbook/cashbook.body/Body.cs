﻿using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.body.data.contract;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{
	public class Body
	{
		Repository repo;
		Func<Transaction[], Cashbook> cashbookFactory;

		public Body(Repository repo, Func<Transaction[],Cashbook> cashbookFactory) {
			this.cashbookFactory = cashbookFactory;
			this.repo = repo;
		}


		public void Deposit(DateTime transactionDate, double amount, string description, bool force,
							Action<Balance> onSuccess, Action<string> onError
		) {
			var transactions = this.repo.Load_all_transactions ().ToArray();
			var cb = this.cashbookFactory (transactions);

			cb.Validate_transaction_date (transactionDate, force,
				() => {
					this.repo.Make_deposit(transactionDate, Math.Abs(amount), description);
					var newBalance = cb.Calculate_end_of_month_balance(transactionDate);
					onSuccess(newBalance);
				},
				onError);
		}
	}
}