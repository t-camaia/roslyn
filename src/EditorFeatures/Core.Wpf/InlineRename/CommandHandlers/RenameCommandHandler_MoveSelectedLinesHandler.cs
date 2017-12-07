// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler :
        IChainedCommandHandler<MoveSelectedLinesUpCommandArgs>, IChainedCommandHandler<MoveSelectedLinesDownCommandArgs>
    {
        public VisualStudio.Commanding.CommandState GetCommandState(MoveSelectedLinesUpCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(MoveSelectedLinesUpCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }

        public VisualStudio.Commanding.CommandState GetCommandState(MoveSelectedLinesDownCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(MoveSelectedLinesDownCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            CommitIfActiveAndCallNextHandler(args, nextHandler);
        }
    }
}
