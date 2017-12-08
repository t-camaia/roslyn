// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.CodeAnalysis.Editor.CommandHandlers
{
    /// <summary>
    /// Implements a execute in interactive command handler.
    /// This class is separated from the <see cref="IExecuteInInteractiveCommandHandler"/>
    /// in order to ensure that the interactive command can be exposed without the necessity
    /// to load any of the interactive dll files just to get the command's status.
    /// </summary>
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name("Interactive Command Handler")]
    internal class ExecuteInInteractiveCommandHandler
        : VisualStudio.Commanding.ICommandHandler<ExecuteInInteractiveCommandArgs>
    {
        private readonly IEnumerable<Lazy<IExecuteInInteractiveCommandHandler, ContentTypeMetadata>> _executeInInteractiveHandlers;

        public string DisplayName => EditorFeaturesResources.Execute_In_Interactive_Command_Handler_Name;

        [ImportingConstructor]
        public ExecuteInInteractiveCommandHandler(
            [ImportMany]IEnumerable<Lazy<IExecuteInInteractiveCommandHandler, ContentTypeMetadata>> executeInInteractiveHandlers)
        {
            _executeInInteractiveHandlers = executeInInteractiveHandlers;
        }

        private Lazy<IExecuteInInteractiveCommandHandler> GetCommandHandler(ITextBuffer textBuffer)
        {
            return _executeInInteractiveHandlers
                .Where(handler => handler.Metadata.ContentTypes.Contains(textBuffer.ContentType.TypeName))
                .SingleOrDefault();
        }

        bool VisualStudio.Commanding.ICommandHandler<ExecuteInInteractiveCommandArgs>.ExecuteCommand(ExecuteInInteractiveCommandArgs args, CommandExecutionContext context)
        {
            return GetCommandHandler(args.SubjectBuffer)?.Value.ExecuteCommand(args, context) ?? false;
        }

        VisualStudio.Commanding.CommandState VisualStudio.Commanding.ICommandHandler<ExecuteInInteractiveCommandArgs>.GetCommandState(ExecuteInInteractiveCommandArgs args)
        {
            return GetCommandHandler(args.SubjectBuffer) == null
                ? VisualStudio.Commanding.CommandState.CommandIsUnavailable
                : VisualStudio.Commanding.CommandState.CommandIsAvailable;
        }
    }
}
