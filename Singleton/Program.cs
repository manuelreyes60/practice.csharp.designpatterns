using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Singleton
{
    public interface IDatabase
    {
        int GetPopulation(string name);
    }

    public class SingletonDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;

        public static SingletonDatabase Instance => instance.Value;
        public int GetPopulation(string name) => capitals[name];

        private SingletonDatabase()
        {
            Console.WriteLine("Init Database");
            capitals = File.ReadAllLines("capitals.txt").Batch(2).ToDictionary(
                list => list.ElementAt(0).Trim(),
                list => int.Parse(list.ElementAt(1))
            );
        }
        
        private static Lazy<SingletonDatabase> instance = new Lazy<SingletonDatabase>(() => new SingletonDatabase());
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = SingletonDatabase.Instance;
            Console.WriteLine(db.GetPopulation("Mexico"));
        }
    }
}