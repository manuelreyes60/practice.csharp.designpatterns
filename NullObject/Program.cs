using Autofac;
using System;
using System.Dynamic;
using static System.Console;

namespace NullObject
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
    }

    class ConsoleLog : ILog
    {
        public void Info(string msg)
        {
            WriteLine(msg);
        }

        public void Warn(string msg)
        {
            WriteLine("WARNING: " + msg);
        }
    }

    public class BankAccount
    {
        private ILog log;
        private int balance;

        public BankAccount(ILog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            balance += amount;
            // check for null everywhere
            log?.Info($"Deposited ${amount}, balance is now {balance}");
        }

        public void Withdraw(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                log?.Info($"Withdrew ${amount}, we have ${balance} left");
            }
            else
            {
                log?.Warn($"Could not withdraw ${amount} because balance is only ${balance}");
            }
        }
    }

    public sealed class NullLog : ILog
    {
        public void Info(string msg)
        {

        }

        public void Warn(string msg)
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<BankAccount>();
            cb.RegisterType<NullLog>().As<ILog>();
            using(var c = cb.Build())
            {
                var ba = c.Resolve<BankAccount>();
                ba.Deposit(100);
            }
        }
    }
}
