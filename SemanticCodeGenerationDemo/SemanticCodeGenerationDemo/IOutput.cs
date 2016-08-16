using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticCodeGenerationDemo
{
    public interface IOutput
    {
        void Write<T>(T value);

        void WritePersonData(string name, int age);
    }
}
