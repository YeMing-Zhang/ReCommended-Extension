using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [ElementProblemAnalyzer(
        typeof(ICSharpTreeNode),
        HighlightingTypes = new[] { typeof(RedundantAssertionStatementSuggestion), typeof(RedundantInlineAssertionSuggestion) })]
    public sealed class ControlFlowAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        [Pure]
        static ICSharpExpression TryGetOtherOperand(
            [NotNull] IEqualityExpression equalityExpression,
            EqualityExpressionType equalityType,
            [NotNull] TokenNodeType operandNodeType)
        {
            if (equalityExpression.EqualityType == equalityType)
            {
                if (IsLiteral(equalityExpression.RightOperand, operandNodeType))
                {
                    return equalityExpression.LeftOperand;
                }

                if (IsLiteral(equalityExpression.LeftOperand, operandNodeType))
                {
                    return equalityExpression.RightOperand;
                }
            }

            return null;
        }

        [Pure]
        static bool IsLiteral(IExpression expression, [NotNull] TokenNodeType tokenType)
            => (expression as ICSharpLiteralExpression)?.Literal?.GetTokenType() == tokenType;

        [NotNull]
        readonly NullnessProvider nullnessProvider;

        [NotNull]
        readonly AssertionMethodAnnotationProvider assertionMethodAnnotationProvider;

        [NotNull]
        readonly AssertionConditionAnnotationProvider assertionConditionAnnotationProvider;

        public ControlFlowAnalyzer([NotNull] CodeAnnotationsCache codeAnnotationsCache)
        {
            nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            assertionMethodAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionMethodAnnotationProvider>();
            assertionConditionAnnotationProvider = codeAnnotationsCache.GetProvider<AssertionConditionAnnotationProvider>();
        }

        void AnalyzeAssertions(
            [NotNull] ElementProblemAnalyzerData data,
            [NotNull] IHighlightingConsumer consumer,
            [NotNull] ICSharpTreeNode rootNode,
            [NotNull] ICSharpControlFlowGraph controlFlowGraph)
        {
            var assertions = Assertion.CollectAssertions(assertionMethodAnnotationProvider, assertionConditionAnnotationProvider, rootNode);

            assertions.ExceptWith(
                from highlightingInfo in consumer.Highlightings
                where highlightingInfo != null
                let redundantAssertionHighlighting = highlightingInfo.Highlighting as RedundantAssertionSuggestion
                where redundantAssertionHighlighting != null
                select redundantAssertionHighlighting.Assertion);

            if (assertions.Count == 0)
            {
                return; // no (new) assertions found
            }

            var inspector = CSharpControlFlowGraphInspector.Inspect(controlFlowGraph, data.GetValueAnalysisMode());

            var alwaysSuccessTryCastExpressions =
                new HashSet<IAsExpression>(inspector.AlwaysSuccessTryCastExpressions ?? Array.Empty<IAsExpression>());

            foreach (var assertion in assertions)
            {
                switch (assertion.AssertionConditionType)
                {
                    case AssertionConditionType.IS_TRUE:
                        AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(consumer, inspector, alwaysSuccessTryCastExpressions, assertion, true);
                        break;

                    case AssertionConditionType.IS_FALSE:
                        AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(consumer, inspector, alwaysSuccessTryCastExpressions, assertion, false);
                        break;

                    case AssertionConditionType.IS_NOT_NULL:
                        AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(consumer, inspector, alwaysSuccessTryCastExpressions, assertion, false);
                        break;

                    case AssertionConditionType.IS_NULL:
                        AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(consumer, inspector, alwaysSuccessTryCastExpressions, assertion, true);
                        break;
                }
            }
        }

        void AnalyzeWhenExpressionIsKnownToBeTrueOrFalse(
            [NotNull] IHighlightingConsumer context,
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeTrue)
        {
            if (assertion is AssertionStatement assertionStatement)
            {
                // pattern: Assert(true); or Assert(false);
                Debug.Assert(CSharpTokenType.TRUE_KEYWORD != null);
                Debug.Assert(CSharpTokenType.FALSE_KEYWORD != null);
                if (IsLiteral(assertionStatement.Expression, isKnownToBeTrue ? CSharpTokenType.TRUE_KEYWORD : CSharpTokenType.FALSE_KEYWORD))
                {
                    context.AddHighlighting(
                        new RedundantAssertionStatementSuggestion(
                            $"Assertion is redundant because the expression is always {(isKnownToBeTrue ? "true" : "false")}.",
                            assertionStatement));
                }

                if (assertionStatement.Expression is IEqualityExpression equalityExpression)
                {
                    // pattern: Assert(x != null); when x is known to be null or not null
                    Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
                    var expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.NE, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(inspector, alwaysSuccessTryCastExpressions, expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is always true.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is always false.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }

                    // pattern: Assert(x == null); when x is known to be null or not null
                    expression = TryGetOtherOperand(equalityExpression, EqualityExpressionType.EQEQ, CSharpTokenType.NULL_KEYWORD);
                    if (expression != null)
                    {
                        switch (GetExpressionNullReferenceState(inspector, alwaysSuccessTryCastExpressions, expression))
                        {
                            case CSharpControlFlowNullReferenceState.NOT_NULL:
                                if (!isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is always false.",
                                            assertionStatement));
                                }
                                break;

                            case CSharpControlFlowNullReferenceState.NULL:
                                if (isKnownToBeTrue)
                                {
                                    context.AddHighlighting(
                                        new RedundantAssertionStatementSuggestion(
                                            "Assertion is redundant because the expression is always true.",
                                            assertionStatement));
                                }
                                break;
                        }
                    }
                }
            }
        }

        void AnalyzeWhenExpressionIsKnownToBeNullOrNotNull(
            [NotNull] IHighlightingConsumer context,
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            [NotNull] Assertion assertion,
            bool isKnownToBeNull)
        {
            if (assertion is AssertionStatement assertionStatement)
            {
                // pattern: Assert(null);
                Debug.Assert(CSharpTokenType.NULL_KEYWORD != null);
                if (isKnownToBeNull && IsLiteral(assertionStatement.Expression, CSharpTokenType.NULL_KEYWORD))
                {
                    context.AddHighlighting(
                        new RedundantAssertionStatementSuggestion(
                            "Assertion is redundant because the expression is always null.",
                            assertionStatement));
                }

                // pattern: Assert(x); when x is known to be null or not null
                switch (GetExpressionNullReferenceState(inspector, alwaysSuccessTryCastExpressions, assertionStatement.Expression))
                {
                    case CSharpControlFlowNullReferenceState.NOT_NULL:
                        if (!isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is never null.",
                                    assertionStatement));
                        }
                        break;

                    case CSharpControlFlowNullReferenceState.NULL:
                        if (isKnownToBeNull)
                        {
                            context.AddHighlighting(
                                new RedundantAssertionStatementSuggestion(
                                    "Assertion is redundant because the expression is always null.",
                                    assertionStatement));
                        }
                        break;
                }
            }

            if (!isKnownToBeNull &&
                assertion is InlineAssertion inlineAssertion &&
                GetExpressionNullReferenceState(inspector, alwaysSuccessTryCastExpressions, inlineAssertion.QualifierExpression) ==
                CSharpControlFlowNullReferenceState.NOT_NULL)
            {
                context.AddHighlighting(
                    new RedundantInlineAssertionSuggestion("Assertion is redundant because the expression is never null.", inlineAssertion));
            }
        }

        [Pure]
        CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(
            [NotNull] CSharpControlFlowGraphInspector inspector,
            [NotNull] HashSet<IAsExpression> alwaysSuccessTryCastExpressions,
            ICSharpExpression expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case IReferenceExpression referenceExpression:
                        if (referenceExpression is IConditionalAccessExpression conditionalAccessExpression &&
                            conditionalAccessExpression.HasConditionalAccessSign)
                        {
                            var referenceState = GetExpressionNullReferenceStateByAnnotations(referenceExpression);
                            if (referenceState == CSharpControlFlowNullReferenceState.NOT_NULL)
                            {
                                expression = conditionalAccessExpression.ConditionalQualifier;
                                continue;
                            }
                        }

                        var nullReferenceState = inspector.GetExpressionNullReferenceState(referenceExpression, true);

                        if (nullReferenceState == CSharpControlFlowNullReferenceState.UNKNOWN)
                        {
                            return GetExpressionNullReferenceStateByAnnotations(referenceExpression);
                        }

                        return nullReferenceState;

                    case IAsExpression asExpression when alwaysSuccessTryCastExpressions.Contains(asExpression):
                        return CSharpControlFlowNullReferenceState.NOT_NULL;

                    case IObjectCreationExpression _: return CSharpControlFlowNullReferenceState.NOT_NULL;

                    case IInvocationExpression invocationExpression:
                        if (invocationExpression.InvokedExpression is IReferenceExpression invokedExpression)
                        {
                            return GetExpressionNullReferenceStateByAnnotations(invokedExpression);
                        }

                        goto default;

                    default: return CSharpControlFlowNullReferenceState.UNKNOWN;
                }
            }
        }

        [Pure]
        CSharpControlFlowNullReferenceState GetExpressionNullReferenceStateByAnnotations([NotNull] IReferenceExpression referenceExpression)
        {
            switch (referenceExpression.Reference.Resolve().DeclaredElement)
            {
                case IFunction function when nullnessProvider.GetInfo(function) == CodeAnnotationNullableValue.NOT_NULL:
                    return CSharpControlFlowNullReferenceState.NOT_NULL;

                case ITypeOwner typeOwner when !typeOwner.Type.IsDelegateType():
                    if (typeOwner is IAttributesOwner attributesOwner &&
                        nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL)
                    {
                        return CSharpControlFlowNullReferenceState.NOT_NULL;
                    }

                    goto default;

                default: return CSharpControlFlowNullReferenceState.UNKNOWN;
            }
        }

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var controlFlowGraph = (ICSharpControlFlowGraph)ControlFlowBuilder.GetGraph(element);
            if (controlFlowGraph != null)
            {
                AnalyzeAssertions(data, consumer, element, controlFlowGraph);
            }
        }
    }
}