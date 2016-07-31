using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RewriterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Compilation test = CreateTestCompilation();

            foreach (var syntaxTree in test.SyntaxTrees)
            {
                var diamondRewriter = new DiamondRewriter();
                SyntaxNode newSource = diamondRewriter.Visit(syntaxTree.GetRoot());

                if (newSource != syntaxTree.GetRoot())
                {
                    test = test.RemoveSyntaxTrees(syntaxTree);
                    test = test.AddSyntaxTrees(newSource.SyntaxTree);
                }
            }

            EmitResult result = test.Emit("dummy.exe");
            Process.Start("dummy.exe");
        }

        static Compilation CreateTestCompilation()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections.Generic;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<int> list = new List<>() {1, 2, 3};
			foreach (var x in list)
			{
				Console.WriteLine(x);
			} 
			Console.ReadLine();
        }
    }
}");

            MetadataReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference list = MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location);

            return CSharpCompilation.Create("Dummy", new[] { tree }, new[] { mscorlib, list },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }
    }
}
