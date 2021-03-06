﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal static class Class
    {
        static void HashSetContains<T>(HashSet<T> items, [AllowNull] T value)
        {
            if (!items.Comparer.Equals(value, default))
            {
                var contained = items.Contains(value!); // not redundant (otherwise: compiler warning)
            }
        }

        class Foo
        {
            public void Method() { }
        }

        static void Reflection()
        {
            var method = typeof(Foo).GetMethod(nameof(Foo.Method), BindingFlags.Public | BindingFlags.Instance)!; // not redundant (nullable result)
            method.Invoke(new Foo(), null);
        }

        static void Reflection_Closure()
        {
            Action action = () =>
            {
                var method = typeof(Foo).GetMethod(
                    nameof(Foo.Method),
                    BindingFlags.Public | BindingFlags.Instance)!; // not redundant (nullable result)
                method.Invoke(new Foo(), null);
            };
            action();
        }

        static void CastingNullableReturnValue()
        {
            var foo = (Foo)Activator.CreateInstance(typeof(Foo))!; // not redundant (otherwise: compiler warning)
        }

        static void UnboxingNullableValueFromDictionary(Dictionary<int, DayOfWeek?> dict)
        {
            var dayOfWeek = (DayOfWeek)dict[3]!; // not redundant (otherwise: compiler warning)
        }

        static void UnboxingNullableValue(int? x)
        {
            var a = new char[(int)x!]; // not redundant (otherwise: compiler warning)
            Console.WriteLine(a.Length);
        }

        static void UnboxingNullableValue_Closure(int? x)
        {
            Action action = () =>
            {
                var b = new char[(int)x!]; // not redundant (otherwise: compiler warning)
                Console.WriteLine(b.Length);
            };
            action();
        }

        static void Defaults<T>(List<T> list, List<string> texts)
        {
            var contains = list.Contains(default!); // not redundant (otherwise: compiler warning)
            var contains2 = texts.Contains((null as string)!); // not redundant (otherwise: compiler warning)
        }

        static async Task Promises(TaskCompletionSource<DayOfWeek?> promise)
        {
            var dayOfWeek = await promise.Task;

            Debug.Assert(dayOfWeek != null); // not redundant (dayOfWeek is nullable)
        }

        static string NotNullMethod() => "one";

        static string field = NotNullMethod().AssertNotNull();
        static string field_NFO = NotNullMethod()!;

        static string Property => NotNullMethod().AssertNotNull();
        static string Property_NFO => NotNullMethod()!;

        static Lazy<string?> PropertyLazy => new Lazy<string?>(() => NotNullMethod().AssertNotNull());
        static Lazy<string?> PropertyLazy_NFO => new Lazy<string?>(() => NotNullMethod()!);

        static string? PropertyNullable => null;

        static string Property2 { get; } = NotNullMethod().AssertNotNull();
        static string Property2_NFO { get; } = NotNullMethod()!;

        static string Property3 { get; set; } = NotNullMethod().AssertNotNull();
        static string Property3_NFO { get; set; } = NotNullMethod()!;

        static string Method() => NotNullMethod().AssertNotNull();
        static string Method_NFO() => NotNullMethod()!;

        [DebuggerStepThrough]
        [JetBrains.Annotations.NotNull]
        static T AssertNotNull<T>(this T? value) where T : class
        {
            AssertThatTrue(value != null);

            return value;
        }

        class Nested
        {
            string? field = NotNullMethod().AssertNotNull();
            string? field_NFO = NotNullMethod()!;

            string? Property => NotNullMethod().AssertNotNull();
            string? Property_NFO => NotNullMethod()!;

            string? AutoProperty { get; } = NotNullMethod().AssertNotNull();
            string? AutoProperty_NFO { get; } = NotNullMethod()!;
        }

        static void ClassConstraint<T>(T one, T? two) where T : class
        {
            var x = one.AssertNotNull();
            var x_NFO = one!;

            var y = two.AssertNotNull().AssertNotNull();
            var y_NFO = two.AssertNotNull()!;
        }

        static void ClassNullableClassConstraint<T>(T one) where T : class?
        {
            var x = one.AssertNotNull().AssertNotNull();
            var x_NFO = one.AssertNotNull()!;
        }

        static readonly string[] Words = { "one", "two", "three" };

        static readonly Dictionary<int, string> WordMap = new Dictionary<int, string>{ { 1, "one" }, { 2, "two" } };

        static readonly Dictionary<int, string[]> WordMap2 =
            new Dictionary<int, string[]> { { 1, new[] { "one", "two", "three" } }, { 2, new[] { "one", "two", "three" } } };

        static void Iterations()
        {
            var query0 = from word in Words where word.AssertNotNull().Length > 2 select word; // "AssertNotNull" must be redundant
            var query0_NFO = from word in Words where word!.Length > 2 select word; // "!" must be redundant
            var query1 = from word in Words where word != null select word; // "word != null" is always true
            var query2 = from word in Words select word.AssertNotNull(); // "AssertNotNull" must be redundant
            var query2_NFO = from word in Words select word!; // "!" must be redundant

            AssertThatNotNull(Words);
            foreach (var word in Words)
            {
                AssertThatNotNull(word);
            }

            AssertThatNotNull(WordMap);
            foreach (var (key, value) in WordMap)
            {
                AssertThatNotNull(value);
            }
            foreach (var value in WordMap.Values)
            {
                AssertThatNotNull(value);
            }

            AssertThatNotNull(WordMap2);
            foreach (var (key, values) in WordMap2)
            {
                AssertThatNotNull(values);
                foreach (var value in values)
                {
                    AssertThatNotNull(value);
                }
            }
            foreach (var values in WordMap2.Values)
            {
                AssertThatNotNull(values);
                foreach (var value in values)
                {
                    AssertThatNotNull(value);
                }
            }
        }

        static void Foo(bool b, object? s, string? x)
        {
            Action action = () =>
            {
                var text = "";
                AssertThatTrue(text != null);
                var text2 = text.AssertNotNull().Replace("a", "b");
                var text2_NFO = text!.Replace("a", "b");
                AssertThatTrue(text2 != null);
            };

            var length = Property.     AssertNotNull()         .     AssertNotNull()      .Length;
            var qqq = Property.AssertNotNull().ToList().All(char.IsDigit);
            var qqq_NFO = Property!.ToList().All(char.IsDigit);

            if (b)
            {
                if (b) { }

                AssertThatTrue(b); // must be redundant

                Console.WriteLine(b.ToString());
            }
            else
            {
                AssertThatFalse(b); // must be redundant
            }

            if (s is string)
            {
                if (s as string != null) { }
                AssertThatTrue(s as string != null);
                AssertThatFalse(s as string == null);
                AssertThatNotNull(s as string);
            }
            else
            {
                AssertThatNull(s as string);
            }

            if (PropertyNullable != null)
            {
                AssertThatTrue(PropertyNullable != null);
            }
            AssertThatFalse(PropertyNullable == null);
            AssertThatNotNull(PropertyNullable);

            AssertThatTrue(true);
            AssertThatFalse(false);
            AssertThatNull<string>(null);

            AssertThatNotNull(new object());
            new object().AssertNotNull();
            var nfo = new object()!;

            if (x != null)
            {
                AssertThatTrue(condition: x != null);
                AssertThatTrue(x != null);
                AssertThatTrue(null != x);

                AssertThatFalse(null == x);
                AssertThatFalse(x == null);

                Console.WriteLine(x.ToString());
            }
            else
            {
                AssertThatTrue(condition: x == null);
                AssertThatTrue(x == null);
                AssertThatTrue(null == x);

                AssertThatFalse(null != x);
                AssertThatFalse(x != null);

                AssertThatNull(x);
            }
        }

        static void Foo(string x)
        {
            AssertThatNotNull(x);
            x.AssertNotNull();

            var y = x!;
        }

        class A
        {
            public string NotNull { get; set; }

            public string? CanBeNull { get; set; }
        }

        static void NullPropagation1(A? canBeNull) => AssertThatTrue(canBeNull?.NotNull != null);

        static void NullPropagation2(A? canBeNull) => AssertThatNotNull(canBeNull?.NotNull);

        static void NullPropagation3(A? canBeNull) => Console.WriteLine(canBeNull?.NotNull.AssertNotNull());
        static void NullPropagation3_NFO(A? canBeNull) => Console.WriteLine(canBeNull?.NotNull!);

        static void NullPropagation4(A notNull) => AssertThatTrue(notNull?.NotNull != null); // redundant

        static void NullPropagation5(A notNull) => AssertThatNotNull(notNull?.NotNull); // redundant

        static void NullPropagation6(A notNull) => Console.WriteLine(notNull?.NotNull.AssertNotNull()); // redundant
        static void NullPropagation6_NFO(A notNull) => Console.WriteLine(notNull?.NotNull!.Length); // redundant

        static void NullPropagation7(A notNull) => AssertThatTrue(notNull?.CanBeNull != null);

        static void NullPropagation8(A notNull) => AssertThatNotNull(notNull?.CanBeNull);

        static void NullPropagation9(A notNull) => Console.WriteLine(notNull?.CanBeNull.AssertNotNull());
        static void NullPropagation9_NFO(A notNull) => Console.WriteLine(notNull?.CanBeNull!);

        [AssertionMethod]
        [ContractAnnotation("false => void")]
        static void AssertThatTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition) => Debug.Assert(condition);

        [AssertionMethod]
        [ContractAnnotation("true => void")]
        static void AssertThatFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool condition) => Debug.Assert(!condition);

        [AssertionMethod]
        [ContractAnnotation("notnull => void")]
        static void AssertThatNull<T>([AssertionCondition(AssertionConditionType.IS_NULL)] T? reference) where T : class
            => Debug.Assert(reference == null);

        [AssertionMethod]
        [ContractAnnotation("null => void")]
        static void AssertThatNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] T? reference) where T : class
            => Debug.Assert(reference != null);

        [return: NotNullIfNotNull("value")]
        static T? PassThrough<T>(T? value) where T : class => value;

        static void CheckPassedThrough(string value)
        {
            var result = PassThrough(value);

            Debug.Assert(result != null); // redundant
        }
    }
}