﻿namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A C# code file contains more than one namespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a C# file contains more than one namespace. To increase long-term
    /// maintainability of the code-base, each file should contain at most one namespace.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1403FileMayOnlyContainASingleNamespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1403FileMayOnlyContainASingleNamespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1403";
        private const string Title = "File may only contain a single namespace";
        private const string MessageFormat = "File may only contain a single namespace";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A C# code file contains more than one namespace.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1403.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(this.HandleSyntaxTreeAsync);
        }

        private async void HandleSyntaxTreeAsync(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = await context.Tree.GetRootAsync(context.CancellationToken).ConfigureAwait(false);

            var descentNodes = syntaxRoot.DescendantNodes(descendIntoChildren: node => node != null && !node.IsKind(SyntaxKind.ClassDeclaration));

            bool foundNode = false;

            foreach (var node in descentNodes)
            {
                if (node.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    if (foundNode)
                    {
                        var location = NamedTypeHelpers.GetNameOrIdentifierLocation(node);
                        if (location != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                    else
                    {
                        foundNode = true;
                    }
                }
            }
        }
    }
}
