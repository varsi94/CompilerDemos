using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerationDemo.Helpers
{
	public static class RoslynHelpers
	{
		public static ClassDeclarationSyntax CreatePublicStaticClass(string name)
		{
			return SyntaxFactory.ClassDeclaration(name)
				.WithModifiers(
					SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
		}

		public static MethodDeclarationSyntax CreatePublicStaticMethod(string name, Type paramType)
		{
			return SyntaxFactory.MethodDeclaration(
				SyntaxFactory.PredefinedType(
					SyntaxFactory.Token(SyntaxKind.StringKeyword)),
				SyntaxFactory.Identifier(name))
				.WithModifiers(
					SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
						SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
				.WithParameterList(
					SyntaxFactory.ParameterList(
						SyntaxFactory.SingletonSeparatedList(
							SyntaxFactory.Parameter(
								SyntaxFactory.Identifier("obj"))
								.WithType(
									SyntaxFactory.ParseTypeName(paramType.FullName)))));
		}
	}
}
