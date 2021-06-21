using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Command
{
    public class BankAccount
    {
        private int balance;
        private int overdraftLimit = -500;

        public void Deposit(int amount)
        {
            balance += amount;
            WriteLine($"Deposited ${amount}, balance is now {balance}");
        }

        public bool Withdraw(int amount)
        {
            if(balance - amount >= overdraftLimit)
            {
                balance -= amount;
                WriteLine($"Withdraw ${amount}, balance is now {balance}");
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{nameof(balance)}: {balance}";
        }
    }
    public interface ICommand
    {
        void Call();
        void Undo();
    }
    public class BankAcoountCommand: ICommand
    {
        private BankAccount account;

        public enum Action
        {
            Deposit, Withdraw
        }

        private Action action;
        private int amount;
        private bool succeeded;

        public BankAcoountCommand(BankAccount account, Action action, int amount)
        {
            this.account = account;
            this.action = action;
            this.amount = amount;
        }

        public void Call()
        {
            switch (action)
            {
                case Action.Deposit:
                    account.Deposit(amount);
                    succeeded = true;
                    break;
                case Action.Withdraw:
                    succeeded = account.Withdraw(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Undo()
        {
            if (!succeeded) return;
            switch (action)
            {
                case Action.Deposit:
                    account.Withdraw(amount);
                    break;
                case Action.Withdraw:
                    account.Deposit(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    // Composite Command
    public class BankAccount2
    {
        private int balance;
        private int overdraftLimit = -500;

        public BankAccount2(int balance = 0)
        {
            this.balance = balance;
        }

        public void Deposit(int amount)
        {
            balance += amount;
            Console.WriteLine($"Deposited ${amount}, balance is now {balance}");
        }

        public bool Withdraw(int amount)
        {
            if (balance - amount >= overdraftLimit)
            {
                balance -= amount;
                Console.WriteLine($"Withdrew ${amount}, balance is now {balance}");
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{nameof(balance)}: {balance}";
        }
    }

    public abstract class Command
    {
        public abstract void Call();
        public abstract void Undo();
        public bool Success;
    }

    public class BankAccountCommand : Command
    {
        private BankAccount2 account;

        public enum Action
        {
            Deposit, Withdraw
        }

        private Action action;
        private int amount;
        private bool succeeded;

        public BankAccountCommand(BankAccount2 account, Action action, int amount)
        {
            this.account = account;
            this.action = action;
            this.amount = amount;
        }

        public override void Call()
        {
            switch (action)
            {
                case Action.Deposit:
                    account.Deposit(amount);
                    succeeded = true;
                    break;
                case Action.Withdraw:
                    succeeded = account.Withdraw(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Undo()
        {
            if (!succeeded) return;
            switch (action)
            {
                case Action.Deposit:
                    account.Withdraw(amount);
                    break;
                case Action.Withdraw:
                    account.Deposit(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    abstract class CompositeBankAccountCommand : List<BankAccountCommand>, ICommand
    {
        public virtual void Call()
        {
            ForEach(cmd => cmd.Call());
        }

        public virtual void Undo()
        {
            foreach (var cmd in
              ((IEnumerable<BankAccountCommand>)this).Reverse())
            {
                cmd.Undo();
            }
        }

    }

    class MoneyTransferCommand : CompositeBankAccountCommand
    {
        public MoneyTransferCommand(BankAccount2 from,
          BankAccount2 to, int amount)
        {
            AddRange(new[]
            {
        new BankAccountCommand(from,
          BankAccountCommand.Action.Withdraw, amount),
        new BankAccountCommand(to,
          BankAccountCommand.Action.Deposit, amount)
      });
        }

        public override void Call()
        {
            bool ok = true;
            foreach (var cmd in this)
            {
                if (ok)
                {
                    cmd.Call();
                    ok = cmd.Success;
                }
                else
                {
                    cmd.Success = false;
                }
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            BankAccount ba = new BankAccount();
            var commands = new List<BankAcoountCommand>
            {
                new BankAcoountCommand(ba, BankAcoountCommand.Action.Deposit, 100),
                new BankAcoountCommand(ba, BankAcoountCommand.Action.Withdraw, 1500),
            };

            WriteLine(ba);

            foreach(var c in commands)
            {
                c.Call();
            }

            WriteLine(ba);

            foreach(var c in Enumerable.Reverse(commands))
            {
                c.Undo();
            }

            WriteLine(ba);


            var ba2 = new BankAccount2();
            var cmdDeposit = new BankAccountCommand(ba2,
              BankAccountCommand.Action.Deposit, 100);
            var cmdWithdraw = new BankAccountCommand(ba2,
              BankAccountCommand.Action.Withdraw, 1000);
            cmdDeposit.Call();
            cmdWithdraw.Call();
            WriteLine(ba);
            cmdWithdraw.Undo();
            cmdDeposit.Undo();
            WriteLine(ba);


            var from = new BankAccount2();
            from.Deposit(100);
            var to = new BankAccount2();

            var mtc = new MoneyTransferCommand(from, to, 1000);
            mtc.Call();


            // Deposited $100, balance is now 100
            // balance: 100
            // balance: 0

            WriteLine(from);
            WriteLine(to);
        }
    }
}
