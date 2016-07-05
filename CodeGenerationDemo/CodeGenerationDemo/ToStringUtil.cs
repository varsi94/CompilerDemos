using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeGenerationDemo.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerationDemo
{
	public static class ToStringUtil
	{
		private static readonly List<Type> KnownTypes = new List<Type>();
		private static CSharpCompilation compilation;
		private static Assembly generatedAssembly;

		public static void AddType<T>()
		{
			KnownTypes.Add(typeof(T));
		}

		public static void GenerateMethodsAndCompile()
		{
			var classDeclaration = RoslynHelpers.CreatePublicStaticClass("ToStringGenerator");

			//metódusok generálása
			foreach (var type in KnownTypes)
			{
				var method = RoslynHelpers.CreatePublicStaticMethod("Get" + type.Name + "String", type);
				//StringBuilder létrehozása
				method = method.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
					SyntaxFactory.VariableDeclaration(
						SyntaxFactory.IdentifierName("var"))
						.WithVariables(
							SyntaxFactory.SingletonSeparatedList(
								SyntaxFactory.VariableDeclarator(
									SyntaxFactory.Identifier("sb"))
									.WithInitializer(
										SyntaxFactory.EqualsValueClause(
											SyntaxFactory.ObjectCreationExpression(
												SyntaxFactory.IdentifierName("StringBuilder"))
											))))));

				foreach (var prop in type.GetProperties())
				{
					//TODO: propertyk kiírása
					//Érdemes használni: http://roslynquoter.azurewebsites.net/
					//Property neve
					method = method.AddBodyStatements(SyntaxFactory.ExpressionStatement(
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									SyntaxFactory.IdentifierName("sb"),
									SyntaxFactory.IdentifierName("Append")))
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SingletonSeparatedList(
										SyntaxFactory.Argument(
											SyntaxFactory.LiteralExpression(
												SyntaxKind.StringLiteralExpression,
												SyntaxFactory.Literal(prop.Name + ":\n\t"))))))));

					//Property értékének kiírása
					method = method.AddBodyStatements(SyntaxFactory.ExpressionStatement(
						SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("sb"),
								SyntaxFactory.IdentifierName("Append")))
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SingletonSeparatedList(
										SyntaxFactory.Argument(
											SyntaxFactory.InvocationExpression(
												SyntaxFactory.MemberAccessExpression(
													SyntaxKind.SimpleMemberAccessExpression,
													SyntaxFactory.MemberAccessExpression(
														SyntaxKind.SimpleMemberAccessExpression,
														SyntaxFactory.IdentifierName("obj"),
														SyntaxFactory.IdentifierName(prop.Name)),
													SyntaxFactory.IdentifierName("ToString")))))))));

					//Soremelés
					method = method.AddBodyStatements(SyntaxFactory.ExpressionStatement(
						SyntaxFactory.InvocationExpression(
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName("sb"),
								SyntaxFactory.IdentifierName("Append")))
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SingletonSeparatedList(
										SyntaxFactory.Argument(
											SyntaxFactory.LiteralExpression(
												SyntaxKind.StringLiteralExpression,
												SyntaxFactory.Literal("\n"))))))));
				}

				//Return statement
				method = method.AddBodyStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.IdentifierName("sb"),
						SyntaxFactory.IdentifierName("ToString")))));
				classDeclaration = classDeclaration.AddMembers(method);
			}

			//Forrásfájl létrehozása, bele az osztályt, amiben a megfelelő metódusok vannak.
			CompilationUnitSyntax compUnit = SyntaxFactory.CompilationUnit().AddMembers(classDeclaration);

			//TODO: fordítás

			//Usingok beállítása, + a System
			foreach (var type in KnownTypes.Union(new[] { typeof(StringBuilder), typeof(object) }))
			{
				compUnit = compUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(type.Namespace)));
			}

			compilation = CSharpCompilation.Create("SerailizerAssmebly",
				new[] { compUnit.SyntaxTree },
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)).AddReferences(
					MetadataReference.CreateFromFile(typeof(ToStringUtil).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(StringBuilder).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

			using (var ms = new MemoryStream())
			{
				compilation.Emit(ms);
				ms.Position = 0;
				generatedAssembly = Assembly.Load(ms.ToArray());
			}

		}

		public static string GetString<T>(T obj)
		{
			return
				(string)
					generatedAssembly.GetType("ToStringGenerator")
						.GetMethod("Get" + typeof(T).Name + "String")
						.Invoke(null, new object[] {obj});
		}

		public static void SaveAssembly()
		{
			using (var fs = new FileStream("output.dll", FileMode.Create))
			{
				compilation.Emit(fs);
			}
		}
	}
}
