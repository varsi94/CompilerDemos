using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationDemo.Model
{
	public class Department
	{
		public string Name { get; set; }
		public string ShortName { get; set; }

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
