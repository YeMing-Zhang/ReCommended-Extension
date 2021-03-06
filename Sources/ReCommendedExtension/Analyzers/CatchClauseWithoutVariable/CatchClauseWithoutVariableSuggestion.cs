using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.CatchClauseWithoutVariable
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant declaration without exception variable" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class CatchClauseWithoutVariableSuggestion : Highlighting
    {
        const string SeverityId = "CatchClauseWithoutVariable";

        internal CatchClauseWithoutVariableSuggestion([NotNull] string message, [NotNull] ISpecificCatchClause catchClause) : base(message)
            => CatchClause = catchClause;

        [NotNull]
        internal ISpecificCatchClause CatchClause { get; }

        public override DocumentRange CalculateRange()
        {
            var document = CatchClause.GetDocumentRange().Document;
            Debug.Assert(document != null);

            return new DocumentRange(
                document,
                new TextRange(CatchClause.LPar.GetDocumentStartOffset().Offset, CatchClause.RPar.GetDocumentEndOffset().Offset));
        }
    }
}