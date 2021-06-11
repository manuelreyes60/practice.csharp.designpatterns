using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factories
{
    //factory
    public class Point
    {
        private double x, y;

        private Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static class Factory
        {
            public static Point NewCartesianPoint(double x, double y) => new Point(x, y);

            public static Point NewPolarPoint(double rho, double theta) => new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }

        public override string ToString() => $"{nameof(x)}: {x}, {nameof(y)}: {y}";
    }

    //async factory
    public class Foo
    {
        private Foo()
        {

        }
        private async Task<Foo> InitAsync()
        {
            await Task.Delay(1000);
            return this;
        }
        public static Task<Foo> CreateAsync()
        {
            var result = new Foo();
            return result.InitAsync();
        }
    }

    // abstract factory
    public interface IHotDrink
    {
        void Consume();
    }

    internal class Tea : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("Nice Tee!");
        }
    }

    internal class Coffe : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("Nice Coffe!");
        }
    }

    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }

    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Preparing {amount} ml of tee!");
            return new Tea();
        }
    }

    internal class CoffeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Preparing {amount} ml of coffe!");
            return new Coffe();
        }
    }

    public class HotDrinkMachine
    {
        /// Violates Open Close Principle

        /*        public enum AvailableDrink
                {
                    Coffe, Tea
                }

                private Dictionary<AvailableDrink, IHotDrinkFactory> factories = new Dictionary<AvailableDrink, IHotDrinkFactory>();

                public HotDrinkMachine()
                {
                    foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
                    {
                        var factory = (IHotDrinkFactory)Activator.CreateInstance(Type.GetType("Factories." + Enum.GetName(typeof(AvailableDrink), drink) + "Factory"));
                        factories.Add(drink, factory);
                    }
                }

                public IHotDrink MakeDrink(AvailableDrink drink, int amount)
                {
                    return factories[drink].Prepare(amount);
                }*/

        private List<Tuple<string, IHotDrinkFactory>> factories = new List<Tuple<string, IHotDrinkFactory>>();
        public HotDrinkMachine()
        {
            foreach(var t in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                if(typeof(IHotDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
                {
                    factories.Add(Tuple.Create(t.Name.Replace("Factory", string.Empty), (IHotDrinkFactory)Activator.CreateInstance(t)));
                }
            }
        }

        public IHotDrink MakeDrink()
        {
            Console.WriteLine("Available Drinks:");
            for(var index = 0; index < factories.Count; index++)
            {
                var tuple = factories[index];
                Console.WriteLine($"{index}: {tuple.Item1}");
            }

            while (true)
            {
                string s;
                if((s = Console.ReadLine()) != null && int.TryParse(s, out int i) && i >= 0 && i < factories.Count)
                {
                    Console.WriteLine("Specify Amount:");
                    s = Console.ReadLine();
                    if ((s != null && int.TryParse(s, out int amount) && i >= 0 && i < factories.Count))
                    {
                        return factories[i].Item2.Prepare(amount);
                    }
                }

                Console.WriteLine("Incorrect Input, try again!");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var point = Point.Factory.NewPolarPoint(1.0, Math.PI / 2);
            Console.WriteLine(point);

            /*Foo x = await Foo.CreateAsync();*/

            var machine = new HotDrinkMachine();
            /*var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Tea, 100);*/
            /*drink.Consume();*/

            var drink = machine.MakeDrink();
            drink.Consume();
        }
    }
}
