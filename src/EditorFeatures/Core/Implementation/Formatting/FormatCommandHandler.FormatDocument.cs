// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;

namespace Microsoft.CodeAnalysis.Editor.Implementation.Formatting
{
    internal partial class FormatCommandHandler
    {
        public VisualStudio.Commanding.CommandState GetCommandState(FormatDocumentCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
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

            context.WaitContext.AllowCancellation = true;
            context.WaitContext.Description = EditorFeaturesResources.Formatting_document;

            Format(args.TextView, document, null, context.WaitContext.CancellationToken);
            
            // We don't call nextHandler, since we have handled this command.
            return true;
        }
    }
}
