using System;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.AsyncVoid
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Async void function expression" + ZoneMarker.Suffix,
        "'async void' lambda or anonymous method expression not used as a direct event handler.",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class AsyncVoidFunctionExpressionWarning : Highlighting
    {
        const string SeverityId = "AsyncVoidFunctionExpression";

        [NotNull]
        readonly ITokenNode asyncKeyword;

        [NotNull]
        readonly Action removeAsyncModifier;

        internal AsyncVoidFunctionExpressionWarning(
            [NotNull] string message,
            [NotNull] ITokenNode asyncKeyword,
            [NotNull] Action removeAsyncModifier) : base(message)
        {
            this.asyncKeyword = asyncKeyword;
            this.removeAsyncModifier = removeAsyncModifier;
        }

        internal void RemoveAsyncModifier() => removeAsyncModifier();

        public override DocumentRange CalculateRange() => asyncKeyword.GetDocumentRange();
    }
}