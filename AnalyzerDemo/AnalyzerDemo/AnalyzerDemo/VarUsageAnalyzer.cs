using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerDemo
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class VarUsageAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "VarUsage";
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Naming";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.LocalDeclarationStatement);
		}

		private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
			//Ha nem var, megyünk tovább.
			if (!localDeclaration.Declaration.Type.IsVar)
			{
				return;
			}
			
			//Var-ral csak egy változót tudunk deklarálni.
			var variable = localDeclaration.Declaration.Variables.First();
			if (variable.Initializer?.Value == null)
			{
				return;
			}

			//Inicializáló kifejezés lekérdezése
			var initializerStr = variable.Initializer.Value.ToString();
			//Típus lekérdezése
			var semanticModel = context.SemanticModel;
			var typeInfo = semanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken);
			var typeStr = typeInfo.Type?.Name;
			//Ha nem tartalmazza a deklaráció a típust, akkor az nem jó.
			if (initializerStr != null && typeStr != null && !initializerStr.Contains(typeStr))
			{
				var diagnostic = Diagnostic.Create(Rule, localDeclaration.GetLocation(), variable.Identifier.Text);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
