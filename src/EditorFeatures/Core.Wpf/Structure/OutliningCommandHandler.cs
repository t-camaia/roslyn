// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.Implementation.Structure
{
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name("Outlining Command Handler")]
    [HandlesCommand(typeof(StartAutomaticOutliningCommandArgs))]
    internal sealed class OutliningCommandHandler : VisualStudio.Commanding.ICommandHandler<StartAutomaticOutliningCommandArgs>
    {
        private readonly IOutliningManagerService _outliningManagerService;

        [ImportingConstructor]
        public OutliningCommandHandler(IOutliningManagerService outliningManagerService)
        {
            _outliningManagerService = outliningManagerService;
        }

        public string DisplayName => "Outlining Command Handler"; //TODO: localize

        public bool ExecuteCommand(StartAutomaticOutliningCommandArgs args, CommandExecutionContext context)
        {
            // The editor actually handles this command, we just have to make sure it is enabled.
            return false;
        }

        public VisualStudio.Commanding.CommandState GetCommandState(StartAutomaticOutliningCommandArgs args)
        {
            var outliningManager = _outliningManagerService.GetOutliningManager(args.TextView);
            var enabled = false;
            if (outliningManager != null)
            {
                enabled = outliningManager.Enabled;
            }

            return enabled ? VisualStudio.Commanding.CommandState.Undetermined : VisualStudio.Commanding.CommandState.CommandIsAvailable;
        }
    }
}
