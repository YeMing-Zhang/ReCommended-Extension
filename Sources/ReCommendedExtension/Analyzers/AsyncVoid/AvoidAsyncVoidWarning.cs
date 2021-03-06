using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.AsyncVoid
{
    [RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.CodeSmell, "Avoid 'async void'" + ZoneMarker.Suffix, "", Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class AvoidAsyncVoidWarning : Highlighting
    {
        const string SeverityId = "AvoidAsyncVoid";

        [CanBeNull]
        readonly ITypeUsage typeUsage;

        internal AvoidAsyncVoidWarning([NotNull] string message, [NotNull] IMethodDeclaration methodDeclaration) : base(message)
        {
            Declaration = methodDeclaration;
            typeUsage = methodDeclaration.TypeUsage;
        }

        internal AvoidAsyncVoidWarning([NotNull] string message, [NotNull] ILocalFunctionDeclaration localFunctionDeclaration) : base(message)
        {
            Declaration = localFunctionDeclaration;
            typeUsage = localFunctionDeclaration.TypeUsage;
        }

        [NotNull]
        internal ITypeOwnerDeclaration Declaration { get; }

        public override DocumentRange CalculateRange() => typeUsage.GetDocumentRange();
    }
}