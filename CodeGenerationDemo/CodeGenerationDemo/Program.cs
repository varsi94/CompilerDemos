using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerationDemo.Model;

namespace CodeGenerationDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var person = new Person
			{
				Name = "Marci",
				BirthDate = new DateTime(1994, 5, 22),
				Department = new Department
				{
					Name = "Automatizálási és Alkalmazott Informatikai Tanszék",
					ShortName = "AUT"
				}
			};

			ToStringUtil.AddType<Person>();
			ToStringUtil.AddType<Department>();
			ToStringUtil.GenerateMethodsAndCompile();

			Console.WriteLine(person.ToString());

			ToStringUtil.SaveAssembly();
		}
	}
}
