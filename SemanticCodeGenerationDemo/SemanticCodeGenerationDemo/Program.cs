using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace SemanticCodeGenerationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //get Demo Class
            var workspace = MSBuildWorkspace.Create();
            var projectPath = @"..\..\SemanticCodeGenerationDemo.csproj";
            var project = workspace.OpenProjectAsync(projectPath).Result;
            var demoClass = project.Documents.SingleOrDefault(d => d.Name == "Logger.cs");
            
            //Get constructor
            var constructor = demoClass.GetSyntaxRootAsync().Result
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First(c => c.Identifier.ToString() == "Logger")
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .First();

            //get semantic model and syntax generator
            var semanticModel = demoClass.GetSemanticModelAsync().Result;
            var symbol = semanticModel.GetDeclaredSymbol(constructor.ParameterList.Parameters.First());
            var type = symbol.Type;

            //generate class
            var generator = SyntaxGenerator.GetGenerator(project);
            var stubDeclaration = (ClassDeclarationSyntax)generator.ClassDeclaration(type.Name + "Stub", 
                accessibility: Accessibility.Public, interfaceTypes: new[]
                {
                    SyntaxFactory.IdentifierName(
                        type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                });

            //implement interface
            foreach (var member in type.GetMembers().OfType<IMethodSymbol>())
            {
                var methodDeclaration = (MethodDeclarationSyntax)generator.AsPublicInterfaceImplementation(
                    generator.MethodDeclaration(member), SyntaxFactory.ParseTypeName(type.Name));
                methodDeclaration = methodDeclaration.AddBodyStatements(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Console"),
                            SyntaxFactory.IdentifierName("WriteLine")))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal(member.Name + " called!"))))))));
                foreach (var parameterSymbol in member.Parameters)
                {
                    //write input parameters
                    methodDeclaration = methodDeclaration.AddBodyStatements(SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("Console"),
                                SyntaxFactory.IdentifierName("WriteLine")))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.BinaryExpression(
                                                SyntaxKind.AddExpression,
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    SyntaxFactory.Literal($"{parameterSymbol.Name}: ")),
                                                SyntaxFactory.IdentifierName(parameterSymbol.Name))))))));
                }
                stubDeclaration = stubDeclaration.AddMembers(methodDeclaration);
            }
            
            //create compilation unit
            var compUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(stubDeclaration)
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(IOutput).Namespace)), 
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(Console).Namespace)));

            var compilation = CSharpCompilation.Create("Dummy", new [] { compUnit.SyntaxTree },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Program).Assembly.Location)
                }, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //load assembly into memory after compilation
            Assembly assembly;
            using (var ms = new MemoryStream())
            using (var fs = new FileStream("output.dll", FileMode.Create))
            {
                var emitResult = compilation.Emit(ms);
                ms.WriteTo(fs);
                assembly = Assembly.Load(ms.ToArray());
            }

            var stub = Activator.CreateInstance(assembly.GetType(type.Name + "Stub")) as IOutput;
            var logger = new Logger(stub);
            logger.LogPerson(new Person {Name = "Marci", Age = 22});
            logger.LogString("Dummy szöveg!");
            Console.ReadLine();
        }
    }
}
