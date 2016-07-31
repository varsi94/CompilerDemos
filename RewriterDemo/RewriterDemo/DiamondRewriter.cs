using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RewriterDemo
{
    public class DiamondRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var type = node.Declaration.Type as GenericNameSyntax;
            if (type != null)
            {
                foreach (var variable in node.Declaration.Variables)
                {
                    var objCreation = variable.Initializer.Value as ObjectCreationExpressionSyntax;
                    if (objCreation == null)
                    {
                        continue;
                    }

                    var nameSyntax = (objCreation.Type as GenericNameSyntax);
                    if (nameSyntax == null)
                    {
                        continue;
                    }

                    if (!nameSyntax.TypeArgumentList.ChildNodes().OfType<OmittedTypeArgumentSyntax>().Any())
                    {
                        continue;
                    }

                    node = node.ReplaceNode(nameSyntax, nameSyntax.Update(nameSyntax.Identifier, type.TypeArgumentList));
                }
            }
            return node;
        }
    }
}
