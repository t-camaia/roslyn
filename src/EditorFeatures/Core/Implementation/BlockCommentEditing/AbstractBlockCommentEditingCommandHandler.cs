// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Editor.Commands;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.CodeAnalysis.Editor.Implementation.BlockCommentEditing
{
    internal abstract class AbstractBlockCommentEditingCommandHandler : BaseAbstractBlockCommentEditingCommandHandler, 
        ICommandHandler<ReturnKeyCommandArgs>
    {
        protected AbstractBlockCommentEditingCommandHandler(
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService)
            : base(undoHistoryRegistry, editorOperationsFactoryService)
        {
        }

        public CommandState GetCommandState(ReturnKeyCommandArgs args, Func<CommandState> nextHandler) => nextHandler();

        public void ExecuteCommand(ReturnKeyCommandArgs args, Action nextHandler)
        {
            if (TryHandleReturnKey(args.SubjectBuffer, args.TextView))
            {
                return;
            }

            nextHandler();
        }
    }
}
