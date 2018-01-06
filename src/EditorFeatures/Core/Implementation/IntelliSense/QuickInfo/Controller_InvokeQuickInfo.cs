// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
{
    internal partial class Controller
    {
        VSCommanding.CommandState VSCommanding.ICommandHandler<InvokeQuickInfoCommandArgs>.GetCommandState(InvokeQuickInfoCommandArgs args)
        {
            AssertIsForeground();
            return VSCommanding.CommandState.Unspecified;
        }

        bool VSCommanding.ICommandHandler<InvokeQuickInfoCommandArgs>.ExecuteCommand(InvokeQuickInfoCommandArgs args, CommandExecutionContext context)
        {
            var caretPoint = args.TextView.GetCaretPoint(args.SubjectBuffer);
            if (caretPoint.HasValue)
            {
                // Invoking QuickInfo from the command, so there's no session yet.
                InvokeQuickInfo(caretPoint.Value.Position, trackMouse: false, augmentSession: null);
            }

            return true;
        }

#pragma warning disable CS0618 // IQuickInfo* is obsolete
        public void InvokeQuickInfo(int position, bool trackMouse, IQuickInfoSession augmentSession)
        {
            AssertIsForeground();
            DismissSessionIfActive();
            StartSession(position, trackMouse, augmentSession);
        }
#pragma warning restore CS0618 // IQuickInfo* is obsolete
    }
}
