using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using static System.Console;

namespace Proxy
{
    // Security Proxy
    public interface ICar
    {
        void Drive();
    }
    public class Car : ICar
    {
        public void Drive()
        {
            WriteLine("Car is being driven");
        }
    }
    public class CarProxy : ICar
    {
        private Driver driver;
        private Car car = new Car();
        public CarProxy(Driver driver)
        {
            this.driver = driver;
        }
        public void Drive()
        {
            if(driver.Age >= 16)
            {
                car.Drive();
            }
            else
            {
                Console.WriteLine("Too young");
            }

        }
    }
    public class Driver
    {
        public int Age { get; set; }
        public Driver(int age)
        {
            Age = age;
        }
    }

    // Property Proxy
    public class Property<T> where T : new()
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;
                Console.WriteLine($"Assigning value to {value}");
                this.value = value;
            }
        }
        public Property() : this(default(T))
        {

        }
        public Property(T value)
        {
            this.value = value;
        }

        public static implicit operator T(Property<T> property)
        {
            return property.value; // int n = p_int
        }

        public static implicit operator Property<T>(T value)
        {
            return new Property<T>(value); // Property<int> p = 123
        }
    }

    public class Creature
    {
        private Property<int> agility = new Property<int>();
        public int Agility
        {
            get => agility.Value;
            set => agility.Value = value;
        }
    }

    // Value Proxy
    public struct Percentage
    {
        private readonly float value;
        internal Percentage(float value)
        {
            this.value = value;
        }
        public static Percentage operator *(float f, Percentage p)
        {
            return new Percentage(f * p.value);
        }
        public static Percentage operator +(Percentage a, Percentage b)
        {
            return new Percentage(a.value + b.value);
        }
        public override string ToString()
        {
            return $"{value * 100}%";
        }
    }

    public static class PercentageExtensions
    {
        public static Percentage Percent(this int value)
        {
            return new Percentage(value / 100.0f);
        }
        public static Percentage Percent(this float value)
        {
            return new Percentage(value / 100.0f);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ICar car = new CarProxy(new Driver(22));
            car.Drive();

            var c = new Creature();
            c.Agility = 10;
            c.Agility = 10;

            Console.WriteLine(10f * 5.Percent());
            Console.WriteLine(2.Percent() + 3.Percent());
        }
    }
}
