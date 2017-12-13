// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.CodeAnalysis.Editor.Implementation.BlockCommentEditing
{
    internal abstract class PlatformAbstractBlockCommentEditingCommandHandler : BaseAbstractBlockCommentEditingCommandHandler, 
        VisualStudio.Commanding.ICommandHandler<ReturnKeyCommandArgs>
    {
        protected PlatformAbstractBlockCommentEditingCommandHandler(
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService)
            : base (undoHistoryRegistry, editorOperationsFactoryService)
        {
        }

        public string DisplayName => EditorFeaturesResources.Block_Comment_Editing_Command_Handler_Name;

        public VisualStudio.Commanding.CommandState GetCommandState(ReturnKeyCommandArgs args) => VisualStudio.Commanding.CommandState.Undetermined;

        public bool ExecuteCommand(ReturnKeyCommandArgs args, CommandExecutionContext context)
        {
            return TryHandleReturnKey(args.SubjectBuffer, args.TextView);
        }
    }
}
