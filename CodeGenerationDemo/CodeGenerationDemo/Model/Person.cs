using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationDemo.Model
{
	public class Person
	{
		public string Name { get; set; }
		public DateTime BirthDate { get; set; }
		public Department Department { get; set; }

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
