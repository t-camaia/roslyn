// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.LanguageServices.CSharp;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.CSharp.EventHookup
{
    internal partial class EventHookupCommandHandler :
        IChainedCommandHandler<EscapeKeyCommandArgs>
    {
        public string DisplayName => CSharpVSResources.Event_Hookup_Command_Handler;

        public void ExecuteCommand(EscapeKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            AssertIsForeground();
            EventHookupSessionManager.CancelAndDismissExistingSessions();
            nextHandler();
        }

        public VSCommanding.CommandState GetCommandState(EscapeKeyCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            AssertIsForeground();
            return nextHandler();
        }
    }
}
