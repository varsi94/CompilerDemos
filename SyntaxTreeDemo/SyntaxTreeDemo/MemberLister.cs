using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTreeDemo
{
    class MemberLister : CSharpSyntaxWalker
    {
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            if (node.Modifiers.Any(m => m.Kind() == SyntaxKind.PublicKeyword))
            {
                Console.WriteLine("Method: " + node.Identifier.ToString() + ": " + node.ReturnType.ToString());
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
            if (node.Modifiers.Any(m => m.Kind() == SyntaxKind.PublicKeyword))
            {
                Console.WriteLine("Property: " + node.Identifier.ToString() + ": " + node.Type.ToString());
            }
        }
    }
}
