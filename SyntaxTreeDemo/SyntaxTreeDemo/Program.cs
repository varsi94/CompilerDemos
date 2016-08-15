using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"../../DemoClass.cs";
            string source = File.ReadAllText(fileName);
            var compUnit = SyntaxFactory.ParseCompilationUnit(source);
            var methodLister = new MemberLister();
            methodLister.Visit(compUnit);
        }
    }
}
