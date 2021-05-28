using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInversionPrinciple
{
    public enum Relationship
    {
        Parent,
        Child,
        Sibling
    }

    public class Person
    {
        public string Name;
    }

    public interface IReleationshipBrowser
    {
        IEnumerable<Person> FindAllChildrenOf(string name);
    }

    public class Relationships : IReleationshipBrowser
    {
        private List<(Person, Relationship, Person)> relations = new List<(Person, Relationship, Person)>();

        public void AddParentAndChild(Person parent, Person child)
        {
            relations.Add((parent, Relationship.Parent, child));
            relations.Add((child, Relationship.Child, parent));
        }

        public IEnumerable<Person> FindAllChildrenOf(string name)
        {
            return relations.Where(x => x.Item1.Name == name && x.Item2 == Relationship.Parent).Select(r => r.Item3);
        }

        public List<(Person, Relationship, Person)> Relations => relations;
    }

    public class Research
    {
        // Better
        public Research(IReleationshipBrowser browser)
        {
            foreach(var p in browser.FindAllChildrenOf("Manuel"))
            {
                Console.WriteLine($"Manuel has a child named {p.Name}");
            }
        }

        // BAD
/*        public Research(Relationships relationships)
        {
            var relations = relationships.Relations;
            foreach (var r in relations.Where(x => x.Item1.Name == "Manuel" && x.Item2 == Relationship.Parent))
            {
                Console.WriteLine($"Manuel has a child named {r.Item3.Name}");
            }
        }*/
    }

    class Program
    {
        public string Name;
        static void Main(string[] args)
        {
            var parent = new Person { Name = "Manuel" };
            var child1 = new Person { Name = "Chris" };
            var child2 = new Person { Name = "Mary" };

            var relationships = new Relationships();
            relationships.AddParentAndChild(parent, child1);
            relationships.AddParentAndChild(parent, child2);

            new Research(relationships);
        }
    }
}
