// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.Editor.Shared.Options;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Operations;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Editor.Implementation.BlockCommentEditing
{
    internal abstract class ModernAbstractBlockCommentEditingCommandHandler : VisualStudio.Commanding.ICommandHandler<ReturnKeyCommandArgs>
    {
        private readonly ITextUndoHistoryRegistry _undoHistoryRegistry;
        private readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

        protected ModernAbstractBlockCommentEditingCommandHandler(
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService)
        {
            Contract.ThrowIfNull(undoHistoryRegistry);
            Contract.ThrowIfNull(editorOperationsFactoryService);

            _undoHistoryRegistry = undoHistoryRegistry;
            _editorOperationsFactoryService = editorOperationsFactoryService;
        }

        public string DisplayName => "Block Comment Editing Command Handler"; //TODO: localize it

        public VisualStudio.Commanding.CommandState GetCommandState(ReturnKeyCommandArgs args) => VisualStudio.Commanding.CommandState.Undetermined;

        public bool ExecuteCommand(ReturnKeyCommandArgs args, CommandExecutionContext context)
        {
            return TryHandleReturnKey(args);
        }

        private bool TryHandleReturnKey(ReturnKeyCommandArgs args)
        {
            var subjectBuffer = args.SubjectBuffer;
            var textView = args.TextView;

            if (!subjectBuffer.GetFeatureOnOffOption(FeatureOnOffOptions.AutoInsertBlockCommentStartString))
            {
                return false;
            }

            var caretPosition = textView.GetCaretPoint(subjectBuffer);
            if (caretPosition == null)
            {
                return false;
            }

            var exteriorText = GetExteriorTextForNextLine(caretPosition.Value);
            if (exteriorText == null)
            {
                return false;
            }

            using (var transaction = _undoHistoryRegistry.GetHistory(args.TextView.TextBuffer).CreateTransaction(EditorFeaturesResources.Insert_new_line))
            {
                var editorOperations = _editorOperationsFactoryService.GetEditorOperations(args.TextView);

                editorOperations.InsertNewLine();
                editorOperations.InsertText(exteriorText);

                transaction.Complete();
                return true;
            }
        }

        protected abstract string GetExteriorTextForNextLine(SnapshotPoint caretPosition);
    }
}
