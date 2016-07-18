using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace AnalyzerDemo
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VarUsageCodeFixProvider)), Shared]
	public class VarUsageCodeFixProvider : CodeFixProvider
	{
		private const string Title = "Change Var";

		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create(VarUsageAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			// Find the type declaration identified by the diagnostic.
			var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: Title,
					createChangedDocument: c => ChangeVarAsync(context.Document, declaration, c),
					equivalenceKey: Title),
				diagnostic);
		}

		private async Task<Document> ChangeVarAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken cancellationToken)
		{
			//A szemantikus modellre szükség van, hogy a jobb oldal típusát kiderítsük.
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var typeInfo = semanticModel.GetTypeInfo(declaration.Declaration.Variables.First().Initializer.Value);

			//Módosítjuk a kifejezést.
			LocalDeclarationStatementSyntax declaration2 = declaration.Update(declaration.Modifiers,
				declaration.Declaration.Update(SyntaxFactory.ParseTypeName(typeInfo.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)), declaration.Declaration.Variables),
				declaration.SemicolonToken);

			//Újraformázzuk, hogy ne romoljon el a formázás.
			var formatted = declaration2.WithAdditionalAnnotations(Formatter.Annotation);
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(declaration, formatted);

			return document.WithSyntaxRoot(newRoot);
		}
	}
}