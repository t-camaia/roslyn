' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.ComponentModel.Composition.Hosting
Imports Microsoft.CodeAnalysis.Editor.UnitTests.SignatureHelp
Imports Microsoft.CodeAnalysis.Editor.VisualBasic.SignatureHelp

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.SignatureHelp
    Public Class GetTypeExpressionSignatureHelpProviderTests
        Inherits AbstractVisualBasicSignatureHelpProviderTests

        Friend Overrides Function CreateSignatureHelpProvider() As ISignatureHelpProvider
            Return New GetTypeExpressionSignatureHelpProvider()
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.SignatureHelp)>
        Public Sub TestInvocationForGetType()
            Dim markup = <a><![CDATA[
Class C
    Sub Foo()
        Dim x = GetType($$
    End Sub
End Class
]]></a>.Value

            Dim expectedOrderedItems = New List(Of SignatureHelpTestItem)()
            expectedOrderedItems.Add(New SignatureHelpTestItem(
                                     "GetType(<typeName>) As System.Type",
                                     "Returns a System.Type object for the specified type name.",
                                     "The type name to return a System.Type object for.",
                                     currentParameterIndex:=0))
            Test(markup, expectedOrderedItems)
        End Sub
    End Class
End Namespace
