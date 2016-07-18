using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptingApiDemo
{
    public class Person : ToStringAwareObject
    {
        public string Name { get; set; }
        public int Age => (DateTime.Now - BirthDate).Days/365;
        public DateTime BirthDate { get; set; }
    }
}
