// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.InteractiveWindow.Commands;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.CommandHandlers
{
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(PredefinedInteractiveCommandsContentTypes.InteractiveCommandContentTypeName)]
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
    [Order(After = PredefinedCommandHandlerNames.SignatureHelp)]
    internal sealed class InteractiveCompletionCommandHandler : AbstractCompletionCommandHandler
    {
        [ImportingConstructor]
        public InteractiveCompletionCommandHandler(IAsyncCompletionService completionService)
            : base(completionService)
        {
        }
    }
}
