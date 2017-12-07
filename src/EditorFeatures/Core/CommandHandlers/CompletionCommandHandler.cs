// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.CommandHandlers
{
    [Export]
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name(PredefinedCommandHandlerNames.Completion)]
    [HandlesCommand(typeof(TabKeyCommandArgs))]
    [HandlesCommand(typeof(ToggleCompletionModeCommandArgs))]
    [HandlesCommand(typeof(TypeCharCommandArgs))]
    [HandlesCommand(typeof(ReturnKeyCommandArgs))]
    [HandlesCommand(typeof(InvokeCompletionListCommandArgs))]
    [HandlesCommand(typeof(CommitUniqueCompletionListItemCommandArgs))]
    [HandlesCommand(typeof(PageUpKeyCommandArgs))]
    [HandlesCommand(typeof(PageDownKeyCommandArgs))]
    [HandlesCommand(typeof(CutCommandArgs))]
    [HandlesCommand(typeof(PasteCommandArgs))]
    [HandlesCommand(typeof(BackspaceKeyCommandArgs))]
    [HandlesCommand(typeof(InsertSnippetCommandArgs))]
    [HandlesCommand(typeof(SurroundWithCommandArgs))]
    [HandlesCommand(typeof(AutomaticLineEnderCommandArgs))]
    [HandlesCommand(typeof(SaveCommandArgs))]
    [HandlesCommand(typeof(DeleteKeyCommandArgs))]
    [HandlesCommand(typeof(SelectAllCommandArgs))]
    [Order(After = PredefinedCommandHandlerNames.SignatureHelp,
           Before = PredefinedCommandHandlerNames.DocumentationComments)]
    internal sealed class CompletionCommandHandler : AbstractCompletionCommandHandler
    {
        [ImportingConstructor]
        public CompletionCommandHandler(IAsyncCompletionService completionService)
            : base(completionService)
        {
        }
    }
}
