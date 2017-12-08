// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis.Editor.Shared.Utilities;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.Implementation.RenameTracking
{
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [ContentType(ContentTypeNames.XamlContentType)]
    [Name(PredefinedCommandHandlerNames.RenameTrackingCancellation)]
    [Order(After = PredefinedCommandHandlerNames.SignatureHelp)]
    [Order(After = PredefinedCommandHandlerNames.IntelliSense)]
    [Order(After = PredefinedCommandHandlerNames.AutomaticCompletion)]
    [Order(After = PredefinedCommandHandlerNames.Completion)]
    [Order(After = PredefinedCommandHandlerNames.QuickInfo)]
    [Order(After = PredefinedCommandHandlerNames.EventHookup)]
    internal class RenameTrackingCancellationCommandHandler : VisualStudio.Commanding.ICommandHandler<EscapeKeyCommandArgs>
    {
        public string DisplayName => EditorFeaturesResources.Rename_Tracking_Cancellation_Command_Handler_Name;

        public bool ExecuteCommand(EscapeKeyCommandArgs args, CommandExecutionContext context)
        {
            var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            return document != null &&
                RenameTrackingDismisser.DismissVisibleRenameTracking(document.Project.Solution.Workspace, document.Id);
        }

        public VisualStudio.Commanding.CommandState GetCommandState(EscapeKeyCommandArgs args)
        {
            return VisualStudio.Commanding.CommandState.Undetermined;
        }
    }
}
