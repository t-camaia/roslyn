// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler :
        IChainedCommandHandler<CutCommandArgs>, IChainedCommandHandler<PasteCommandArgs>
    {
        public VisualStudio.Commanding.CommandState GetCommandState(CutCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(CutCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            HandlePossibleTypingCommand(args, nextHandler, span =>
            {
                nextHandler();
            });
        }

        public VisualStudio.Commanding.CommandState GetCommandState(PasteCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            return nextHandler();
        }

        public void ExecuteCommand(PasteCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            HandlePossibleTypingCommand(args, nextHandler, span =>
            {
                nextHandler();
            });
        }
    }
}
