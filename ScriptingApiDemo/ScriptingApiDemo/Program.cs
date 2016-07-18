using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptingApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person
            {
                Name = "Marci",
                BirthDate = new DateTime(1994, 5, 22)
            };

            Console.WriteLine(person.ToString());
            Console.ReadLine();
        }
    }
}
