// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using Xunit;

namespace System.Reflection.Tests
{
    public class GetTypeTests
    {
        [Fact]
        public void GetType_EmptyString()
        {
            Assembly a = typeof(GetTypeTests).GetTypeInfo().Assembly;
            Module m = a.ManifestModule;

            string typeName = "";
            string aqn = ", " + typeof(GetTypeTests).GetTypeInfo().Assembly.FullName;

            // Type.GetType
            Assert.Null(Type.GetType(typeName));
            Assert.Null(Type.GetType(aqn));

            Assert.Null(Type.GetType(typeName, throwOnError: false));
            Assert.Null(Type.GetType(aqn, throwOnError: false));

            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => Type.GetType(aqn, throwOnError: true));

            Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: true));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => Type.GetType(aqn, throwOnError: true, ignoreCase: true));

            // Assembly.GetType
            Assert.Throws<ArgumentException>(() => a.GetType(typeName));
            Assert.Null(a.GetType(aqn));

            Assert.Throws<ArgumentException>(() => a.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => a.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<ArgumentException>(() => a.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => a.GetType(typeName, throwOnError: true, ignoreCase: true));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => a.GetType(aqn, throwOnError: true, ignoreCase: false));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => a.GetType(aqn, throwOnError: true, ignoreCase: true));

            // Module.GetType
            Assert.Throws<ArgumentException>(() => m.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => m.GetType(typeName, throwOnError: false, ignoreCase: true));
            Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: false));
            Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: true));

            Assert.Throws<ArgumentException>(() => m.GetType(typeName, throwOnError: true, ignoreCase: false));
            Assert.Throws<ArgumentException>(() => m.GetType(typeName, throwOnError: true, ignoreCase: true));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => m.GetType(aqn, throwOnError: true, ignoreCase: false));
            AssertExtensions.Throws<ArgumentException>("typeName@0", () => m.GetType(aqn, throwOnError: true, ignoreCase: true));
        }

        public static IEnumerable<object[]> GetType_TestData()
        {
            yield return new object[] { "non-existent-type", null };
            yield return new object[] { typeof(MyClass1).FullName, typeof(MyClass1) };
            yield return new object[] { "System.Reflection.Tests.mYclAss1", typeof(MyClass1) };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace99.MyClASs3+inNer", null };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs399+inNer", null };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs3+inNer99", null };
            yield return new object[] { typeof(MyNamespace1.MyNamespace2.MyClass2).FullName, typeof(MyNamespace1.MyNamespace2.MyClass2) };
            yield return new object[] { typeof(MyNamespace1.MyNamespace2.MyClass3.iNner).FullName, typeof(MyNamespace1.MyNamespace2.MyClass3.iNner) };
            yield return new object[] { "System.Reflection.Tests.MyNameSPACe1.MyNAMEspace2.MyClASs3+inNer", typeof(MyNamespace1.MyNamespace2.MyClass3.iNner) };
            yield return new object[] { typeof(MyNamespace1.MyNamespace3.Foo).FullName, typeof(MyNamespace1.MyNamespace3.Foo) };
            yield return new object[] { "System.Reflection.Tests.mynamespace1.mynamespace3.foo", typeof(MyNamespace1.MyNamespace3.Foo) };
            yield return new object[] { "System.Reflection.Tests.MYNAMESPACE1.MYNAMESPACE3.FOO", typeof(MyNamespace1.MyNamespace3.Foo) };

            Type type = typeof(MyNamespace1.MynAmespace3.Goo<int>);
            yield return new object[] { type.FullName, type };
            yield return new object[] { type.FullName.ToUpper(), type };
            yield return new object[] { type.FullName.ToLower(), type };
        }

        [Theory]
        [MemberData(nameof(GetType_TestData))]
        public void GetTypeTest(string typeName, Type expectedResult)
        {
            Assembly a = typeof(GetTypeTests).GetTypeInfo().Assembly;
            Module m = a.ManifestModule;

            string aqn = typeName + ", " + a.FullName;
            if (expectedResult == null)
            {
                // Type.GetType
                Assert.Null(Type.GetType(typeName));
                Assert.Null(Type.GetType(aqn));

                Assert.Null(Type.GetType(typeName, throwOnError: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true));

                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Assembly.GetType
                Assert.Null(a.GetType(typeName));
                Assert.Null(a.GetType(aqn));

                Assert.Null(a.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(a.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => a.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => a.GetType(typeName, throwOnError: true, ignoreCase: true));
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module.GetType
                Assert.Null(m.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Null(m.GetType(typeName, throwOnError: false, ignoreCase: true));
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => m.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Throws<TypeLoadException>(() => m.GetType(typeName, throwOnError: true, ignoreCase: true));
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else if (expectedResult.FullName == typeName)
            {
                // Case-sensitive match.
                // Type.GetType
                Assert.Equal(expectedResult, Type.GetType(typeName));
                Assert.Equal(expectedResult, Type.GetType(aqn));

                Assert.Equal(expectedResult, Type.GetType(typeName, throwOnError: false));
                Assert.Equal(expectedResult, Type.GetType(aqn, throwOnError: false));

                Assert.Equal(expectedResult, Type.GetType(typeName, throwOnError: true));
                Assert.Equal(expectedResult, Type.GetType(aqn, throwOnError: true));

                // When called with "ignoreCase: true", GetType() may have a choice of matching items. The one that is chosen
                // is an implementation detail (and on the CLR, *very* implementation-dependent as it's influenced by the internal
                // layout of private hash tables.) As a result, we do not expect the same result across runtimes and so the best
                // we can do is compare the names.
                string expectedName = expectedResult.AssemblyQualifiedName;

                Assert.Equal(expectedResult, Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(expectedResult, Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(aqn, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);

                Assert.Equal(expectedResult, Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(expectedResult, Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(aqn, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);

                // Assembly.GetType
                Assert.Equal(expectedResult, a.GetType(typeName));
                Assert.Null(a.GetType(aqn));

                Assert.Equal(expectedResult, a.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, a.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Equal(expectedResult, a.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, a.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module.GetType
                Assert.Equal(expectedResult, m.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, m.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Equal(expectedResult, m.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, m.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else if (expectedResult.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                // Case-insensitive match.
                // Type.GetType
                Assert.Null(Type.GetType(typeName));
                Assert.Null(Type.GetType(aqn));

                Assert.Null(Type.GetType(typeName, throwOnError: false));
                Assert.Null(Type.GetType(aqn, throwOnError: false));

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true));
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true));

                // When called with "ignoreCase: true", GetType() may have a choice of matching items. The one that is chosen
                // is an implementation detail (and on the CLR, *very* implementation-dependent as it's influenced by the internal
                // layout of private hash tables.) As a result, we do not expect the same result across runtimes and so the best
                // we can do is compare the names.
                string expectedName = expectedResult.AssemblyQualifiedName;

                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Null(Type.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(aqn, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);

                Assert.Throws<TypeLoadException>(() => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Throws<TypeLoadException>(() => Type.GetType(aqn, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, Type.GetType(aqn, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);

                // Assembly.GetType
                Assert.Null(a.GetType(typeName));
                Assert.Null(a.GetType(aqn));

                Assert.Null(a.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, a.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(a.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => a.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, a.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => a.GetType(aqn, throwOnError: true, ignoreCase: true));

                // Module.GetType
                Assert.Null(m.GetType(typeName, throwOnError: false, ignoreCase: false));
                Assert.Equal(expectedName, m.GetType(typeName, throwOnError: false, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: false));
                Assert.Null(m.GetType(aqn, throwOnError: false, ignoreCase: true));

                Assert.Throws<TypeLoadException>(() => m.GetType(typeName, throwOnError: true, ignoreCase: false));
                Assert.Equal(expectedName, m.GetType(typeName, throwOnError: true, ignoreCase: true).AssemblyQualifiedName, StringComparer.OrdinalIgnoreCase);
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: false));
                AssertExtensions.Throws<ArgumentException>(null, () => m.GetType(aqn, throwOnError: true, ignoreCase: true));
            }
            else
            {
                throw new InvalidOperationException("TEST ERROR.");
            }
        }

        [Fact]
        public void GetType_CoreAssembly()
        {
            Assert.Equal(typeof(int), Type.GetType("System.Int32", throwOnError: true));
            Assert.Equal(typeof(int), Type.GetType("system.int32", throwOnError: true, ignoreCase: true));
        }

        [Fact]
        public void TestAssemblyNameWithInternationalChar()
        {
            Type testObj = typeof(Hello工程123.Program);
            var t = Type.GetType(testObj.AssemblyQualifiedName);
            Assert.NotNull(t);
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/37871", TestRuntimes.Mono)]
        public void GetType_GenericTypeArgumentList()
        {
            Assert.NotNull(Type.GetType("System.Reflection.Tests.GenericClass`1", throwOnError: true));
            Assert.Equal(typeof(System.Reflection.Tests.GenericClass<System.String>), Type.GetType("System.Reflection.Tests.GenericClass`1[[System.String, System.Private.CoreLib]]", throwOnError: true));
            Assert.Throws<FileNotFoundException>(() => Type.GetType("System.Reflection.Tests.GenericClass`1[[Bogus, BogusAssembly]]", throwOnError: true));
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/37871", TestRuntimes.Mono)]
        public void GetType_InvalidAssemblyName()
        {
            Assert.Null(Type.GetType("MissingAssemblyName, "));
            Assert.Null(Type.GetType("ExtraComma, ,"));
            Assert.Null(Type.GetType("ExtraComma, , System.Runtime"));
            Assert.Throws<FileLoadException>(() => Type.GetType("System.Object, System.Runtime, Version=x.y"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsTypeEquivalenceSupported))]
        public void TestTypeIdentifierAttribute()
        {
            string typeName = $"{typeof(EquivalentValueType).FullName},{typeof(TestAssembly.ClassToInvoke).Assembly.GetName().Name}";
            Type otherEquivalentValueType = Type.GetType(typeName);
            Assert.True(otherEquivalentValueType.IsEquivalentTo(typeof(EquivalentValueType)));

            var mi = typeof(MyNamespace1.ClassForTypeIdentifier).GetMethod("MyMethod");

            // Sanity check.
            object[] args = new object[1] { Activator.CreateInstance(typeof(EquivalentValueType)) };
            Assert.Equal(42, mi.Invoke(null, args));

            // Ensure we can invoke with an arg type that is duplicated in another assembly but having [TypeIdentifier].
            args = new object[1] { Activator.CreateInstance(otherEquivalentValueType) };
            Assert.Equal(42, mi.Invoke(null, args));
        }
    }

    namespace MyNamespace1
    {
        namespace MyNamespace2
        {
            public class MyClass2 { }
            public class MyClass3
            {
                public class Inner { }
                public class @inner { }
                public class iNner { }
                public class inNer { }
            }
            public class MyClass4 { }
            public class mYClass4 { }
            public class Myclass4 { }
            public class myCLass4 { }
            public class myClAss4 { }
        }

        namespace MyNamespace3
        {
            public class Foo { }
        }

        namespace MynAmespace3
        {
            public class Foo { }

            public class Goo<T> { }
            public class gOo<T> { }
            public class goO<T> { }
        }

        namespace MyNaMespace3
        {
            public class Foo { }
        }

        namespace MyNamEspace3
        {
            public class Foo { }
        }

        public class ClassForTypeIdentifier
        {
            public static int MyMethod(EquivalentValueType v) => 42;
        }
    }

    public class MyClass1 { }

    public class GenericClass<T> { }
}
