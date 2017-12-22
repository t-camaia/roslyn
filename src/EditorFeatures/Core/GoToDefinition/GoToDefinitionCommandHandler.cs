// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.Notification;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.GoToDefinition
{
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name(PredefinedCommandHandlerNames.GoToDefinition)]
    internal class GoToDefinitionCommandHandler :
        VisualStudio.Commanding.ICommandHandler<GoToDefinitionCommandArgs>
    {
        public string DisplayName => EditorFeaturesResources.Go_To_Definition_Command_Handler_Name;

        private (Document, IGoToDefinitionService) GetDocumentAndService(ITextSnapshot snapshot)
        {
            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
            return (document, document?.GetLanguageService<IGoToDefinitionService>());
        }

        public VisualStudio.Commanding.CommandState GetCommandState(GoToDefinitionCommandArgs args)
        {
            var (document, service) = GetDocumentAndService(args.SubjectBuffer.CurrentSnapshot);
            return service != null
                ? VisualStudio.Commanding.CommandState.Available
                : VisualStudio.Commanding.CommandState.Undetermined;
        }

        public bool ExecuteCommand(GoToDefinitionCommandArgs args, CommandExecutionContext context)
        {
            var subjectBuffer = args.SubjectBuffer;
            var (document, service) = GetDocumentAndService(subjectBuffer.CurrentSnapshot);
            if (service != null)
            {
                var caretPos = args.TextView.GetCaretPoint(subjectBuffer);
                if (caretPos.HasValue && TryExecuteCommand(document, caretPos.Value, service, context))
                {
                    return true;
                }
            }

            return false;
        }

        // Internal for testing purposes only.
        internal bool TryExecuteCommand(ITextSnapshot snapshot, int caretPosition, CommandExecutionContext context)
            => TryExecuteCommand(snapshot.GetOpenDocumentInCurrentContextWithChanges(), caretPosition, context);

        internal bool TryExecuteCommand(Document document, int caretPosition, CommandExecutionContext context)
            => TryExecuteCommand(document, caretPosition, document.GetLanguageService<IGoToDefinitionService>(), context);

        internal bool TryExecuteCommand(Document document, int caretPosition, IGoToDefinitionService goToDefinitionService, CommandExecutionContext context)
        {
            string errorMessage = null;

            using (context.WaitContext.AddScope(allowCancellation: true, EditorFeaturesResources.Navigating_to_definition))
            {
                if (goToDefinitionService != null &&
                    goToDefinitionService.TryGoToDefinition(document, caretPosition, context.WaitContext.CancellationToken))
                {
                    return true;
                }

                errorMessage = EditorFeaturesResources.Cannot_navigate_to_the_symbol_under_the_caret;
            }

            if (errorMessage != null)
            {
                var workspace = document.Project.Solution.Workspace;
                var notificationService = workspace.Services.GetService<INotificationService>();
                notificationService.SendNotification(errorMessage, title: EditorFeaturesResources.Go_to_Definition, severity: NotificationSeverity.Information);
            }

            return true;
        }
    }
}
