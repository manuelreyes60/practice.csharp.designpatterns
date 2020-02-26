using Chain_of_Responsibility_First_Look.Business;
using Chain_of_Responsibility_First_Look.Business.Models;
using System;
using System.Globalization;

namespace Chain_of_Responsibility_First_Look
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new User("Filip Ekberg", 
                "870101XXXX", 
                new RegionInfo("SE"), 
                new DateTimeOffset(1987, 01, 29, 00, 00, 00, TimeSpan.FromHours(2)));


            Console.WriteLine(user.Age);
            var processor = new UserProcessor();

            var ressult = processor.Register(user);

            Console.WriteLine(ressult);
        }
    }
}
