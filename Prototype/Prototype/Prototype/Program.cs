using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Prototype
{
    // Interface default implementation
    /*    public interface IDeepCopyable<T> where T : new()
    {
        void CopyTo(T target);
        public T DeepCopy()
        {
            T t = new T();
            CopyTo(t);
            return t;
        }
    }*/

    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, self);
            stream.Seek(0, SeekOrigin.Begin);
            object copy = formatter.Deserialize(stream);
            stream.Close();
            return (T)copy;
        }

        public static T DeepCopyXml<T>(this T self)
        {
            using(var ms = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(ms, self);
                ms.Position = 0;
                return (T)s.Deserialize(ms);
            }
        }
    }

    /*[Serializable]*/
    public class Person
    {
        public string[] Names;
        public Address Address;

        public Person()
        {

        }
        public Person(string [] names, Address address)
        {
            Names = names;
            Address = address;
        }

        public Person(Person otherPerson)
        {
            Names = otherPerson.Names;
            Address = new Address(otherPerson.Address);
        }

        public override string ToString()
        {
            return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
        }
    }

/*    [Serializable]*/
    public class Address
    {
        public string StreetName;
        public int HouseNumber;

        public Address()
        {

        }

        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public Address(Address otherAddress)
        {
            StreetName = otherAddress.StreetName;
            HouseNumber = otherAddress.HouseNumber;
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
    }

/*    [Serializable]*/
    public class Employee : Person
    {
        public int Salary;

        public Employee()
        {

        }
        public Employee(string[] names, Address address, int salary) : base(names, address)
        {
            Salary = salary;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Salary)}: {Salary}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var John = new Person(new[] { "John", "Smith" }, new Address("London Road", 123));
            Console.WriteLine(John);

            var Jane = new Person(John);
            Jane.Address.HouseNumber = 321;
            Console.WriteLine(Jane);

            /*var Jill = John.DeepCopy();*/
            var Jill = John.DeepCopyXml();
            Jill.Names[0] = "Jill";

            Console.WriteLine(Jill);
        }
    }
}
