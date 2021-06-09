using System;
using System.Collections.Generic;
using System.Text;

namespace Builder
{
    public class HtmlElement
    {
        public string Name, Text;
        public List<HtmlElement> Elements = new List<HtmlElement>();
        private const int indentSize = 2;

        public HtmlElement()
        {

        }

        public HtmlElement(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indentSize * indent);
            sb.AppendLine($"{i}<{Name}>");
            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', indentSize * (indent + 1)));
                sb.AppendLine(Text);
            }
             
            foreach(var e in Elements)
            {
                sb.Append(e.ToStringImpl(indent + 1));
            }

            sb.AppendLine($"{i}</{Name}>");
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    class HtmlBuilder
    {
        private readonly string rootName;
        HtmlElement root = new HtmlElement();
        public HtmlBuilder(string rootName)
        {
            this.rootName = rootName;
            root.Name = rootName;
        }

        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new HtmlElement { Name = rootName };
        }
    }

    public class Person
    {
        public string Name;
        public string Position;
        public class Builder : PersonJobBuilder<Builder>
        {

        }
        public static Builder New => new Builder();
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }

    public abstract class PersonBuilder
    {
        protected Person person = new Person();
        public Person Build()
        {
            return person;
        }
    }

    public class PersonInfoBuilder <SELF> : PersonBuilder
        where SELF : PersonInfoBuilder<SELF>
    {
        public SELF Called(string name)
        {
            person.Name = name;
            return (SELF)this;
        }
    }

    public class PersonJobBuilder<SELF> : PersonInfoBuilder<PersonJobBuilder<SELF>>
        where SELF : PersonJobBuilder<SELF>
    {
        public SELF WorksAs(string position)
        {
            person.Position = position;
            return (SELF)this;
        }
    }

    public class AnotherPerson
    {
        public string StreetAddress, PostCode, City;
        public string CompanyName, Position;
        public int AnnualIncome;

        public override string ToString()
        {
            return $"{nameof(StreetAddress)}: {StreetAddress}, " +
                $"{nameof(PostCode)}: {PostCode}, " +
                $"{nameof(City)}: {City}, " +
                $"{nameof(CompanyName)}: {CompanyName}, " +
                $"{nameof(Position)}: {Position}, " +
                $"{nameof(AnnualIncome)}: {AnnualIncome}";
        }
    }

    public class AnotherPersonBuilder
    {
        protected AnotherPerson person = new AnotherPerson();
        public AnotherPersonJobBuilder Works => new AnotherPersonJobBuilder(person);
        public AnotherPersonAddressBuilder Lives => new AnotherPersonAddressBuilder(person);
        public static implicit operator AnotherPerson(AnotherPersonBuilder apb)
        {
            return apb.person;
        }
    }

    public class AnotherPersonJobBuilder : AnotherPersonBuilder
    {
        public AnotherPersonJobBuilder(AnotherPerson person)
        {
            this.person = person;
        }
        public AnotherPersonJobBuilder At(string companyName)
        {
            person.CompanyName = companyName;
            return this;
        }

        public AnotherPersonJobBuilder AsA(string position)
        {
            person.Position = position;
            return this;
        }

        public AnotherPersonJobBuilder Earning(int amount)
        {
            person.AnnualIncome = amount;
            return this;
        }
    }

    public class AnotherPersonAddressBuilder : AnotherPersonBuilder
    {
        public AnotherPersonAddressBuilder(AnotherPerson person)
        {
            this.person = person;
        }

        public AnotherPersonAddressBuilder At(string streetAddress)
        {
            person.StreetAddress = streetAddress;
            return this;
        }

        public AnotherPersonAddressBuilder WithPostCode(string postcode)
        {
            person.PostCode = postcode;
            return this;
        }

        public AnotherPersonAddressBuilder In(string city)
        {
            person.City = city;
            return this;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var hello = "hello";
            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(hello);
            sb.Append("/<p>");
            Console.WriteLine(sb);

            var words = new[] { "hello", "world" };
            sb.Clear();
            sb.Append("<ul>");
            foreach (var word in words)
            {
                sb.AppendFormat("<li>{0}</li>", word);
            }
            sb.Append("</ul>");
            Console.WriteLine(sb);

            var builder = new HtmlBuilder("ul");
            builder.AddChild("li", "Hello").AddChild("li", "World");
            Console.WriteLine(builder);

            var me = Person.New.Called("Manuel").WorksAs("MyJob").Build();
            Console.WriteLine(me);

            var apb = new AnotherPersonBuilder();
            AnotherPerson person = apb.Works.At("Fabrikam").AsA("Eng").Earning(100000).Lives.At("123 London").In("London").WithPostCode("123");
            Console.WriteLine(person);
        }
    }
}
