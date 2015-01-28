// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Microsoft.CodeAnalysis.Editor.CSharp.SignatureHelp;
using Microsoft.CodeAnalysis.Editor.UnitTests.SignatureHelp;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.SignatureHelp
{
    public class GenericNamePartiallyWrittenSignatureHelpProviderTests : AbstractCSharpSignatureHelpProviderTests
    {
        internal override ISignatureHelpProvider CreateSignatureHelpProvider()
        {
            return new GenericNamePartiallyWrittenSignatureHelpProvider();
        }

        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void NestedGenericUnterminated()
        {
            var markup = @"
class G<T> { };

class C
{
    void Foo()
    {
        G<G<int>$$
    }
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("G<T>", string.Empty, string.Empty, currentParameterIndex: 0));

            Test(markup, expectedOrderedItems);
        }

        [WorkItem(544088)]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void DeclaringGenericTypeWith1ParameterUnterminated()
        {
            var markup = @"
class G<T> { };

class C
{
    void Foo()
    {
        [|G<$$
    |]}
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("G<T>", string.Empty, string.Empty, currentParameterIndex: 0));

            Test(markup, expectedOrderedItems);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void CallingGenericAsyncMethod()
        {
            var markup = @"
using System.Threading.Tasks;
class Program
{
    void Main(string[] args)
    {
        Foo<$$
    }
    Task<int> Foo<T>()
    {
        return Foo<T>();
    }
}
";
            var documentation = @"
Usage:
  int x = await Foo();";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("(awaitable) Task<int> Program.Foo<T>()", documentation, string.Empty, currentParameterIndex: 0));

            // TODO: Enable the script case when we have support for extension methods in scripts
            Test(markup, expectedOrderedItems, usePreviousCharAsTrigger: false, sourceCodeKind: Microsoft.CodeAnalysis.SourceCodeKind.Regular);
        }

        [WorkItem(7336, "DevDiv_Projects/Roslyn")]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void EditorBrowsable_GenericMethod_BrowsableAlways()
        {
            var markup = @"
class Program
{
    void M()
    {
        new C().Foo<$$
    }
}
";

            var referencedCode = @"
public class C
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
    public void Foo<T>(T x)
    { }
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("void C.Foo<T>(T x)", string.Empty, string.Empty, currentParameterIndex: 0));

            TestSignatureHelpInEditorBrowsableContexts(markup: markup,
                                                       referencedCode: referencedCode,
                                                       expectedOrderedItemsMetadataReference: expectedOrderedItems,
                                                       expectedOrderedItemsSameSolution: expectedOrderedItems,
                                                       sourceLanguage: LanguageNames.CSharp,
                                                       referencedLanguage: LanguageNames.CSharp);
        }

        [WorkItem(7336, "DevDiv_Projects/Roslyn")]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void EditorBrowsable_GenericMethod_BrowsableNever()
        {
            var markup = @"
class Program
{
    void M()
    {
        new C().Foo<$$
    }
}
";

            var referencedCode = @"
public class C
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Foo<T>(T x)
    { }
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("void C.Foo<T>(T x)", string.Empty, string.Empty, currentParameterIndex: 0));

            TestSignatureHelpInEditorBrowsableContexts(markup: markup,
                                                       referencedCode: referencedCode,
                                                       expectedOrderedItemsMetadataReference: new List<SignatureHelpTestItem>(),
                                                       expectedOrderedItemsSameSolution: expectedOrderedItems,
                                                       sourceLanguage: LanguageNames.CSharp,
                                                       referencedLanguage: LanguageNames.CSharp);
        }

        [WorkItem(7336, "DevDiv_Projects/Roslyn")]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void EditorBrowsable_GenericMethod_BrowsableAdvanced()
        {
            var markup = @"
class Program
{
    void M()
    {
        new C().Foo<$$
    }
}
";

            var referencedCode = @"
public class C
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public void Foo<T>(T x)
    { }
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("void C.Foo<T>(T x)", string.Empty, string.Empty, currentParameterIndex: 0));

            TestSignatureHelpInEditorBrowsableContexts(markup: markup,
                                                       referencedCode: referencedCode,
                                                       expectedOrderedItemsMetadataReference: expectedOrderedItems,
                                                       expectedOrderedItemsSameSolution: expectedOrderedItems,
                                                       sourceLanguage: LanguageNames.CSharp,
                                                       referencedLanguage: LanguageNames.CSharp,
                                                       hideAdvancedMembers: false);

            TestSignatureHelpInEditorBrowsableContexts(markup: markup,
                                                       referencedCode: referencedCode,
                                                       expectedOrderedItemsMetadataReference: new List<SignatureHelpTestItem>(),
                                                       expectedOrderedItemsSameSolution: expectedOrderedItems,
                                                       sourceLanguage: LanguageNames.CSharp,
                                                       referencedLanguage: LanguageNames.CSharp,
                                                       hideAdvancedMembers: true);
        }

        [WorkItem(7336, "DevDiv_Projects/Roslyn")]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void EditorBrowsable_GenericMethod_BrowsableMixed()
        {
            var markup = @"
class Program
{
    void M()
    {
        new C().Foo<$$
    }
}
";

            var referencedCode = @"
public class C
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
    public void Foo<T>(T x)
    { }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Foo<T, U>(T x, U y)
    { }
}";
            var expectedOrderedItemsMetadataReference = new List<SignatureHelpTestItem>();
            expectedOrderedItemsMetadataReference.Add(new SignatureHelpTestItem("void C.Foo<T>(T x)", string.Empty, string.Empty, currentParameterIndex: 0));

            var expectedOrderedItemsSameSolution = new List<SignatureHelpTestItem>();
            expectedOrderedItemsSameSolution.Add(new SignatureHelpTestItem("void C.Foo<T>(T x)", string.Empty, string.Empty, currentParameterIndex: 0));
            expectedOrderedItemsSameSolution.Add(new SignatureHelpTestItem("void C.Foo<T, U>(T x, U y)", string.Empty, string.Empty, currentParameterIndex: 0));

            TestSignatureHelpInEditorBrowsableContexts(markup: markup,
                                                       referencedCode: referencedCode,
                                                       expectedOrderedItemsMetadataReference: expectedOrderedItemsMetadataReference,
                                                       expectedOrderedItemsSameSolution: expectedOrderedItemsSameSolution,
                                                       sourceLanguage: LanguageNames.CSharp,
                                                       referencedLanguage: LanguageNames.CSharp);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void GenericExtensionMethod()
        {
            var markup = @"
interface IFoo
{
    void Bar<T>();
}

static class FooExtensions
{
    public static void Bar<T1, T2>(this IFoo foo) { }
}

class Program
{
    static void Main()
    {
        IFoo f = null;
        f.[|Bar<$$
    |]}
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>
            {
                new SignatureHelpTestItem("void IFoo.Bar<T>()", currentParameterIndex: 0),
                new SignatureHelpTestItem("(extension) void IFoo.Bar<T1, T2>()", currentParameterIndex: 0),
            };

            // Extension methods are supported in Interactive/Script (yet).
            Test(markup, expectedOrderedItems, sourceCodeKind: SourceCodeKind.Regular);
        }

        [WorkItem(544088)]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void InvokingGenericMethodWith1ParameterUnterminated()
        {
            var markup = @"
class C
{
    /// <summary>
    /// Method Foo
    /// </summary>
    /// <typeparam name=""T"">Method type parameter</typeparam>
    void Foo<T>() { }

    void Bar()
    {
        [|Foo<$$
    |]}
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("void C.Foo<T>()",
                    "Method Foo", "Method type parameter", currentParameterIndex: 0));

            Test(markup, expectedOrderedItems);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void TestInvocationOnTriggerBracket()
        {
            var markup = @"
class G<S, T> { };

class C
{
    void Foo()
    {
        [|G<$$
    |]}
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("G<S, T>", string.Empty, string.Empty, currentParameterIndex: 0));

            Test(markup, expectedOrderedItems, usePreviousCharAsTrigger: true);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void TestInvocationOnTriggerComma()
        {
            var markup = @"
class G<S, T> { };

class C
{
    void Foo()
    {
        [|G<int,$$
    |]}
}";

            var expectedOrderedItems = new List<SignatureHelpTestItem>();
            expectedOrderedItems.Add(new SignatureHelpTestItem("G<S, T>", string.Empty, string.Empty, currentParameterIndex: 1));

            Test(markup, expectedOrderedItems, usePreviousCharAsTrigger: true);
        }

        [WorkItem(1067933)]
        [Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)]
        public void InvokedWithNoToken()
        {
            var markup = @"
// foo<$$";

            Test(markup);
        }
    }
}
