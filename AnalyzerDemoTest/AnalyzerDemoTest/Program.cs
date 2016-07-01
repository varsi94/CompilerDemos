using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerDemoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person { Name = "Marci", Age = 22 };
            var p2 = GetPerson();
            var p3 = AdjEgySzemelyt();
        }

        static Person GetPerson()
        {
            return new Person();
        }

        static Person AdjEgySzemelyt()
        {
            return new Person();
        }
    }
}
