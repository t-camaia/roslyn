' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Namespace Microsoft.CodeAnalysis.Editor.UnitTests.ReferenceHighlighting
    Public Class CSharpReferenceHighlightingTests
        Inherits AbstractReferenceHighlightingTests

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub VerifyNoHighlightsWhenOptionDisabled()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document>
                            class $$Foo
                            {
                                Foo f;
                            }
                        </Document>
                    </Project>
                </Workspace>,
                optionIsEnabled:=False)
        End Sub

        <Fact>
        <Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub VerifyHighlightsForClass()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document>
                            class {|Definition:$$Foo|}
                            {
                            }
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <Fact>
        <Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub VerifyHighlightsForScriptReference()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document>
                            <ParseOptions Kind="Script"/>

                            void M()
                            {
                            }

                            [|$$Script|].M();
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub VerifyHighlightsForCSharpClassWithConstructor()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document>
                            class {|Definition:$$Foo|}
                            {
                                {|Definition:Foo|}()
                                {
                                    [|var|] x = new [|Foo|]();
                                }
                            }
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <WorkItem(538721)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub VerifyHighlightsForCSharpClassWithSynthesizedConstructor()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document>
                            class {|Definition:Foo|}
                            {
                                void Blah()
                                {
                                    var x = new [|$$Foo|]();
                                }
                            }
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        <WorkItem(528436)>
        Public Sub VerifyHighlightsOnCloseAngleOfGeneric()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document><![CDATA[
using System;
using System.Collections.Generic;
using System.Linq;

class {|Definition:Program|}
{
    static void Main(string[] args)
    {
        new List<[|Program$$|]>();
    }
}]]>
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        <WorkItem(570809)>
        Public Sub VerifyNoHighlightsOnAsyncLambda()
            VerifyHighlights(
                <Workspace>
                    <Project Language="C#" CommonReferences="true">
                        <Document><![CDATA[
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public delegate Task del();
    del ft = $$async () =>
    {
        return await Task.Yield();
    };

}]]>
                        </Document>
                    </Project>
                </Workspace>)
        End Sub

        <WorkItem(543768)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAlias1()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
namespace X
{
    using {|Definition:Q|} = System.IO;
    Class B
    {
        public void M()
        {
            $$[|Q|].Directory.Exists("");
        }
    }
}
</Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(543768)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAlias2()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
namespace X
{
    using $${|Definition:Q|} = System.IO;
    Class B
    {
        public void M()
        {
            [|Q|].Directory.Exists("");
        }
    }
}
</Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(543768)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAlias3()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
namespace X
{
    using Q = System.$$[|IO|];
    Class B
    {
        public void M()
        {
            [|Q|].Directory.Exists("");
        }
    }
}
</Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(552000)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAlias4()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document><![CDATA[
using C = System.Action;

namespace N
{
    using $${|Definition:C|} = A<C>;  // select C 
    class A<T> { }
    class B : [|C|] { }
}]]>
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(542830)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestHighlightThroughVar1()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class C
{
    void F()
    {
        $$[|var|] i = 1;
        [|int|] j = 0;
        double d;
        [|int|] k = 1;
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(542830)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestHighlightThroughVar2()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class C
{
    void F()
    {
        [|var|] i = 1;
        $$[|int|] j = 0;
        double d;
        [|int|] k = 1;
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(542830)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestHighlightThroughVar3()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document><![CDATA[
using System.Collections.Generic;

class C
{
    void F()
    {
        $$[|var|] i = new [|List|]<string>();
        int j = 0;
        double d;
        [|var|] k = new [|List|]<int>();
    }
}
                    ]]></Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(545648)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestUsingAliasAndTypeWithSameName1()
            Dim input =
<Workspace>
    <Project Language="C#" CommonReferences="true">
        <Document>
using {|Definition:$$X|} = System;

class X { }
        </Document>
    </Project>
</Workspace>
            VerifyHighlights(input)
        End Sub

        <WorkItem(545648)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestUsingAliasAndTypeWithSameName2()
            Dim input =
<Workspace>
    <Project Language="C#" CommonReferences="true">
        <Document>
using X = System;

class {|Definition:$$X|} { }
        </Document>
    </Project>
</Workspace>
            VerifyHighlights(input)
        End Sub

        <WorkItem(567959)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAccessor1()
            Dim input =
<Workspace>
    <Project Language="C#" CommonReferences="true">
        <Document>
class C
{
    string P
    {
        $$get
        {
            return P;
        }
        set
        {
            P = "";
        }
    }
}
        </Document>
    </Project>
</Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(567959)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestAccessor2()
            Dim input =
<Workspace>
    <Project Language="C#" CommonReferences="true">
        <Document>
class C
{
    string P
    {
        get
        {
            return P;
        }
        $$set
        {
            P = "";
        }
    }
}
        </Document>
    </Project>
</Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(604466)>
        <Fact(Skip:="604466"), Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub ThisShouldNotHighlightTypeName()
            Dim input =
<Workspace>
    <Project Language="C#" CommonReferences="true">
        <Document>
class C
{
    void M()
    {
        t$$his.M();
    }
}
        </Document>
    </Project>
</Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(531620)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestHighlightDynamicallyBoundMethod()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class A
{
    class B
    {
        public void {|Definition:Boo|}(int d) { } //Line 1
        public void Boo(dynamic d) { } //Line 2
        public void Boo(string d) { } //Line 3
    }
    void Aoo()
    {
        B b = new B();
        dynamic d = 1.5f; 
        b.[|Boo|](1); //Line 4
        b.$$[|Boo|](d); //Line 5
        b.Boo("d"); //Line 6
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <WorkItem(531624)>
        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestHighlightParameterizedPropertyParameter()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class C
{
    int this[int $${|Definition:i|}]
    {
        get
        {
            return this[[|i|]];
        }
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestInterpolatedString1()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class C
{
    void M()
    {
        var $${|Definition:a|} = "Hello";
        var b = "World";
        var c = $"{[|a|]}, {b}!";
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

        <Fact, Trait(Traits.Feature, Traits.Features.ReferenceHighlighting)>
        Public Sub TestInterpolatedString2()
            Dim input =
            <Workspace>
                <Project Language="C#" CommonReferences="true">
                    <Document>
class C
{
    void M()
    {
        var a = "Hello";
        var $${|Definition:b|} = "World";
        var c = $"{a}, {[|b|]}!";
    }
}
                    </Document>
                </Project>
            </Workspace>

            VerifyHighlights(input)
        End Sub

    End Class
End Namespace
