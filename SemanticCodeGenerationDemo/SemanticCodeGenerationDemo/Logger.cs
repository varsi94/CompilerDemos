using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticCodeGenerationDemo
{
    public class Logger
    {
        private IOutput Output { get; }

        public Logger(IOutput output)
        {
            Output = output;
        }

        public void LogPerson(Person person)
        {
            Output.Write(person.Name);
            Output.Write(person.Age);
        }
    }
}
