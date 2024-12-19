using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var queries = new string[][]
            {
                    new[] { "CREATE_ACCOUNT", "1", "account1" },
                    new[] { "CREATE_ACCOUNT", "2", "account2" },
                    new[] { "DEPOSIT", "3", "account1", "2000" },
                    new[] { "DEPOSIT", "4", "account2", "2000" },
                    new[] { "SCHEDULE_PAYMENT", "5", "account2", "300", "15" },
                    new[] { "SCHEDULE_PAYMENT", "6", "account2", "300", "10" },
                    new[] { "TRANSFER", "7", "account1", "account2", "500" },
                    new[] { "MERGE_ACCOUNTS", "8", "account1", "non-existing" },
                    new[] { "MERGE_ACCOUNTS", "9", "account1", "account1" },
                    new[] { "MERGE_ACCOUNTS", "10", "account1", "account2" },
                    new[] { "DEPOSIT", "11", "account1", "100" },
                    new[] { "DEPOSIT", "12", "account2", "100" },
                    new[] { "CANCEL_PAYMENT", "13", "account2", "payment1" },
                    new[] { "CANCEL_PAYMENT", "14", "account1", "payment2" },
                    new[] { "GET_BALANCE", "15", "account2", "1" },
                    new[] { "GET_BALANCE", "16", "account2", "10" },
                    new[] { "GET_BALANCE", "17", "account1", "12" },
                    new[] { "DEPOSIT", "20", "account1", "100" }
            };

            var results = Solution(queries);
            Console.WriteLine(string.Join(", ", results));
            Console.ReadLine();
        }

        public static string[] Solution(string[][] queries)
        {
            var bankingSystem = new BankingSystem();
            var results = new List<string>();

            foreach (var query in queries)
            {
                string result = string.Empty;
                switch (query[0])
                {
                    case "CREATE_ACCOUNT":
                        result = bankingSystem.CreateAccount(query[1], query[2]);
                        break;
                    case "DEPOSIT":
                        result = bankingSystem.Deposit(query[1], query[2], decimal.Parse(query[3]));
                        break;
                    case "TRANSFER":
                        result = bankingSystem.Transfer(query[1], query[2], query[3], decimal.Parse(query[4]));
                        break;
                    case "TOP_SPENDERS":
                        result = bankingSystem.TopSpenders(query[1], int.Parse(query[2]));
                        break;
                    case "SCHEDULE_PAYMENT":
                        result = bankingSystem.SchedulePayment(query[1], query[2], decimal.Parse(query[3]), int.Parse(query[4]));
                        break;
                    case "CANCEL_PAYMENT":
                        result = bankingSystem.CancelPayment(query[1], query[2], query[3]);
                        break;
                    case "MERGE_ACCOUNTS":
                        result = bankingSystem.MergeAccounts(query[1], query[2], query[3]);
                        break;
                    case "GET_BALANCE":
                        result = bankingSystem.GetBalance(query[1], query[2], query[3]);
                        break;
                }
                results.Add(result);
            }

            return results.ToArray();
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static string ConcatString(string a, string b)
        {
            return a + " "+ b;
        }

        public static bool LetterExits(string word, string letter)
        {
            if(word.Contains(letter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class BankingSystem
    {
        private readonly Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        private readonly List<ScheduledPayment> scheduledPayments = new List<ScheduledPayment>();
        private int paymentCounter = 0;

        public string CreateAccount(string timestamp, string accountId)
        {
            if (accounts.ContainsKey(accountId))
            {
                return "false";
            }

            accounts[accountId] = new Account { AccountId = accountId, Balance = 0, TotalOutgoing = 0 };
            return "true";
        }

        public string Deposit(string timestamp, string accountId, decimal amount)
        {
            if (!accounts.ContainsKey(accountId))
            {
                return "";
            }

            accounts[accountId].Balance += amount;
            ProcessScheduledPayments(timestamp);
            return accounts[accountId].Balance.ToString();
        }

        public string Transfer(string timestamp, string sourceAccountId, string targetAccountId, decimal amount)
        {
            if (!accounts.ContainsKey(sourceAccountId) || !accounts.ContainsKey(targetAccountId) || sourceAccountId == targetAccountId)
            {
                return "";
            }

            var sourceAccount = accounts[sourceAccountId];
            var targetAccount = accounts[targetAccountId];

            if (sourceAccount.Balance < amount)
            {
                return "";
            }

            sourceAccount.Balance -= amount;
            sourceAccount.TotalOutgoing += amount;
            targetAccount.Balance += amount;

            return sourceAccount.Balance.ToString();
        }

        public string TopSpenders(string timestamp, int n)
        {
            // Ensure all scheduled payments up to the current timestamp are processed
            ProcessScheduledPayments(timestamp);

            var topSpenders = accounts.Values
                .OrderByDescending(a => a.TotalOutgoing)
                .ThenBy(a => a.AccountId)
                .Take(n)
                .Select(a => $"{a.AccountId}({a.TotalOutgoing})");

            return string.Join(", ", topSpenders);
        }

        public string SchedulePayment(string timestamp, string accountId, decimal amount, int delay)
        {
            if (!accounts.ContainsKey(accountId))
            {
                return "";
            }

            var paymentId = $"payment{++paymentCounter}";
            var scheduledPayment = new ScheduledPayment
            {
                PaymentId = paymentId,
                AccountId = accountId,
                Amount = amount,
                ScheduledTime = int.Parse(timestamp) + delay,
                Status = "Scheduled"
            };

            scheduledPayments.Add(scheduledPayment);
            return paymentId;
        }

        public string CancelPayment(string timestamp, string accountId, string paymentId)
        {
            var payment = scheduledPayments.FirstOrDefault(p => p.PaymentId == paymentId && p.AccountId == accountId);
            if (payment == null || payment.Status != "Scheduled" || payment.ScheduledTime < int.Parse(timestamp))
            {
                return "false";
            }

            payment.Status = "Cancelled";
            return "true";
        }

        public string MergeAccounts(string timestamp, string accountId1, string accountId2)
        {
            if (accountId1 == accountId2 || !accounts.ContainsKey(accountId1) || !accounts.ContainsKey(accountId2))
            {
                return "false";
            }

            var account1 = accounts[accountId1];
            var account2 = accounts[accountId2];

            account1.Balance += account2.Balance;
            account1.TotalOutgoing += account2.TotalOutgoing;

            foreach (var payment in scheduledPayments.Where(p => p.AccountId == accountId2))
            {
                payment.AccountId = accountId1;
            }

            accounts.Remove(accountId2);
            return "true";
        }

        public string GetBalance(string timestamp, string accountId, string timeAt)
        {
            if (!accounts.ContainsKey(accountId))
            {
                return "";
            }

            ProcessScheduledPayments(timeAt);
            return accounts[accountId].Balance.ToString();
        }

        private void ProcessScheduledPayments(string timestamp)
        {
            var currentTime = int.Parse(timestamp);
            var paymentsToProcess = scheduledPayments
                .Where(p => p.ScheduledTime <= currentTime && p.Status == "Scheduled")
                .OrderBy(p => p.ScheduledTime)
                .ThenBy(p => p.PaymentId)
                .ToList();

            foreach (var payment in paymentsToProcess)
            {
                var account = accounts[payment.AccountId];
                if (account.Balance >= payment.Amount)
                {
                    account.Balance -= payment.Amount;
                    account.TotalOutgoing += payment.Amount;
                    payment.Status = "Completed";
                }
                else
                {
                    payment.Status = "Failed";
                }
            }
        }
    }

    public class Account
    {
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalOutgoing { get; set; }
    }

    public class ScheduledPayment
    {
        public string PaymentId { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public int ScheduledTime { get; set; }
        public string Status { get; set; }
    }
}