// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler :
        IChainedCommandHandler<ReorderParametersCommandArgs>,
        IChainedCommandHandler<RemoveParametersCommandArgs>,
        IChainedCommandHandler<ExtractInterfaceCommandArgs>,
        IChainedCommandHandler<EncapsulateFieldCommandArgs>
    {
        public VSCommanding.CommandState GetCommandState(ReorderParametersCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(ReorderParametersCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }

        public VSCommanding.CommandState GetCommandState(RemoveParametersCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(RemoveParametersCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }

        public VSCommanding.CommandState GetCommandState(ExtractInterfaceCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(ExtractInterfaceCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }

        public VSCommanding.CommandState GetCommandState(EncapsulateFieldCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(EncapsulateFieldCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }
    }
}
