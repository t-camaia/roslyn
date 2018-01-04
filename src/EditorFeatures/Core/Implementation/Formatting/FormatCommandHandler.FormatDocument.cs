// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.Implementation.Formatting
{
    internal partial class FormatCommandHandler
    {
        public VSCommanding.CommandState GetCommandState(FormatDocumentCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return GetCommandState(args.SubjectBuffer, nextHandler);
        }

        public void ExecuteCommand(FormatDocumentCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if (!TryExecuteCommand(args, context))
            {
                nextHandler();
            }
        }

        private bool TryExecuteCommand(FormatDocumentCommandArgs args, CommandExecutionContext context)
        {
            if (!args.SubjectBuffer.CanApplyChangeDocumentToWorkspace())
            {
                return false;
            }

            var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
            {
                return false;
            }

            var formattingService = document.GetLanguageService<IEditorFormattingService>();
            if (formattingService == null || !formattingService.SupportsFormatDocument)
            {
                return false;
            }

            var result = false;
            using (context.WaitContext.AddScope(allowCancellation: true, EditorFeaturesResources.Formatting_document))
            {
                Format(args.TextView, document, null, context.WaitContext.UserCancellationToken);
                result = true;
            }

            // We don't call nextHandler, since we have handled this command.
            return result;
        }
    }
}
