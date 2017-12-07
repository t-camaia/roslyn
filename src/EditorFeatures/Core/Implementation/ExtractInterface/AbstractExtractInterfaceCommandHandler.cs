// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis.Editor.Shared;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.ExtractInterface;
using Microsoft.CodeAnalysis.Navigation;
using Microsoft.CodeAnalysis.Notification;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;

namespace Microsoft.CodeAnalysis.Editor.Implementation.ExtractInterface
{
    internal abstract class AbstractExtractInterfaceCommandHandler : VisualStudio.Commanding.ICommandHandler<ExtractInterfaceCommandArgs>
    {
        public string DisplayName => EditorFeaturesResources.Extract_Interface_Command_Handler_Name;

        public VisualStudio.Commanding.CommandState GetCommandState(ExtractInterfaceCommandArgs args)
        {
            var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null ||
                !document.Project.Solution.Workspace.CanApplyChange(ApplyChangesKind.AddDocument) ||
                !document.Project.Solution.Workspace.CanApplyChange(ApplyChangesKind.ChangeDocument))
            {
                return VisualStudio.Commanding.CommandState.Undetermined;
            }

            var supportsFeatureService = document.Project.Solution.Workspace.Services.GetService<IDocumentSupportsFeatureService>();
            if (!supportsFeatureService.SupportsRefactorings(document))
            {
                return VisualStudio.Commanding.CommandState.Undetermined;
            }

            return VisualStudio.Commanding.CommandState.CommandIsAvailable;
        }

        public bool ExecuteCommand(ExtractInterfaceCommandArgs args, CommandExecutionContext context)
        {
            var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
            {
                return false;
            }

            var workspace = document.Project.Solution.Workspace;

            if (!workspace.CanApplyChange(ApplyChangesKind.AddDocument) ||
                !workspace.CanApplyChange(ApplyChangesKind.ChangeDocument))
            {
                return false;
            }

            var supportsFeatureService = document.Project.Solution.Workspace.Services.GetService<IDocumentSupportsFeatureService>();
            if (!supportsFeatureService.SupportsRefactorings(document))
            {
                return false;
            }

            var caretPoint = args.TextView.GetCaretPoint(args.SubjectBuffer);
            if (!caretPoint.HasValue)
            {
                return false;
            }

            var extractInterfaceService = document.GetLanguageService<AbstractExtractInterfaceService>();
            var result = extractInterfaceService.ExtractInterface(
                document,
                caretPoint.Value.Position,
                (errorMessage, severity) => workspace.Services.GetService<INotificationService>().SendNotification(errorMessage, severity: severity),
                context.WaitContext.CancellationToken);

            if (result == null || !result.Succeeded)
            {
                return true;
            }

            if (!document.Project.Solution.Workspace.TryApplyChanges(result.UpdatedSolution))
            {
                // TODO: handle failure
                return true;
            }

            var navigationService = workspace.Services.GetService<IDocumentNavigationService>();
            navigationService.TryNavigateToPosition(workspace, result.NavigationDocumentId, 0);

            return true;
        }
    }
}
