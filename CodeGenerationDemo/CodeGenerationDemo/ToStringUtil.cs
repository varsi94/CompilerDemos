using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationDemo
{
	public static class ToStringUtil
	{
		private static readonly List<Type> KnownTypes = new List<Type>();

		public static void AddType<T>()
		{
			KnownTypes.Add(typeof(T));
		}

		public static void GenerateMethodsAndCompile()
		{
			throw new NotImplementedException();
		}

		public static string GetString<T>()
		{
			throw new NotImplementedException();
		}

		public static void SaveAssembly()
		{
			
		}
	}
}
