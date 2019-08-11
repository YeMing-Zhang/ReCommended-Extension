using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Annotate method with [MustUseReturnValue] attribute" + ZoneMarker.Suffix,
        Description = "Annotates a method with the [MustUseReturnValue] attribute.")]
    public sealed class AnnotateWithMustUseReturnValue : AnnotateWithCodeAnnotation
    {
        public AnnotateWithMustUseReturnValue([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName
        {
            get
            {
                Debug.Assert(MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName != null);

                return MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName;
            }
        }

        protected override string TextSuffix => "with observable state changes";

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule psiModule)
            => declaredElement is IMethod method && !method.ReturnType.IsVoid() && !method.IsAsync;

        protected override IAttribute TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration)
            => ownerDeclaration.Attributes.FirstOrDefault(
                attribute => attribute.AssertNotNull().GetAttributeInstance().GetAttributeType().GetClrName().ShortName ==
                    PureAnnotationProvider.PureAttributeShortName);
    }
}