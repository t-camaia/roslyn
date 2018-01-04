// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler :
        IChainedCommandHandler<UndoCommandArgs>, IChainedCommandHandler<RedoCommandArgs>
    {
        public VSCommanding.CommandState GetCommandState(UndoCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return GetCommandState(nextHandler);
        }

        public VSCommanding.CommandState GetCommandState(RedoCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return GetCommandState(nextHandler);
        }

        public void ExecuteCommand(UndoCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if (_renameService.ActiveSession != null)
            {
                for (int i = 0; i < args.Count && _renameService.ActiveSession != null; i++)
                {
                    _renameService.ActiveSession.UndoManager.Undo(args.SubjectBuffer);
                }
            }
            else
            {
                nextHandler();
            }
        }

        public void ExecuteCommand(RedoCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            if (_renameService.ActiveSession != null)
            {
                for (int i = 0; i < args.Count && _renameService.ActiveSession != null; i++)
                {
                    _renameService.ActiveSession.UndoManager.Redo(args.SubjectBuffer);
                }
            }
            else
            {
                nextHandler();
            }
        }
    }
}
