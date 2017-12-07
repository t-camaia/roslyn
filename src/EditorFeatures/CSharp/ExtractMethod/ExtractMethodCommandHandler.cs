// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis.Editor.Implementation.ExtractMethod;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.CSharp.ExtractMethod
{
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [HandlesCommand(typeof(ExtractMethodCommandArgs))]
    [ContentType(ContentTypeNames.CSharpContentType)]
    [Name(PredefinedCommandHandlerNames.ExtractMethod)]
    [Order(After = PredefinedCommandHandlerNames.DocumentationComments)]
    internal class ExtractMethodCommandHandler :
        AbstractExtractMethodCommandHandler
    {
        [ImportingConstructor]
        public ExtractMethodCommandHandler(
            ITextBufferUndoManagerProvider undoManager,
            IEditorOperationsFactoryService editorOperationsFactoryService,
            IInlineRenameService renameService) :
            base(undoManager, editorOperationsFactoryService, renameService)
        {
        }
    }
}
