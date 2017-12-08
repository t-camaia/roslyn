' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.ComponentModel.Composition
Imports Microsoft.CodeAnalysis.Editor.Implementation.ExtractInterface
Imports Microsoft.VisualStudio.Utilities

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.ExtractInterface
    <Export(GetType(VisualStudio.Commanding.ICommandHandler))>
    <ContentType(ContentTypeNames.VisualBasicContentType)>
    <Name(PredefinedCommandHandlerNames.ExtractInterface)>
    Friend Class ExtractInterfaceCommandHandler
        Inherits AbstractExtractInterfaceCommandHandler

    End Class
End Namespace
