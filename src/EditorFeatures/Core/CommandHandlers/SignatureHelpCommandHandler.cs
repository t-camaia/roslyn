// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.SignatureHelp;
using Microsoft.CodeAnalysis.Editor.Options;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.Editor.Shared.Utilities;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Shared.TestHooks;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.CommandHandlers
{
    [Export]
    [Export(typeof(VisualStudio.Commanding.ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name(PredefinedCommandHandlerNames.SignatureHelp)]
    [HandlesCommand(typeof(TypeCharCommandArgs))]
    [HandlesCommand(typeof(InvokeSignatureHelpCommandArgs))]
    [Order(Before = PredefinedCommandHandlerNames.Completion)]
    internal class SignatureHelpCommandHandler :
        ForegroundThreadAffinitizedObject,
        IChainedCommandHandler<TypeCharCommandArgs>,
        IChainedCommandHandler<InvokeSignatureHelpCommandArgs>
    {
        private readonly IIntelliSensePresenter<ISignatureHelpPresenterSession, ISignatureHelpSession> _signatureHelpPresenter;
        private readonly IEnumerable<Lazy<IAsynchronousOperationListener, FeatureMetadata>> _asyncListeners;
        private readonly IList<Lazy<ISignatureHelpProvider, OrderableLanguageMetadata>> _signatureHelpProviders;

        public string DisplayName => EditorFeaturesResources.Signature_Help_Command_Handler_Name;

        [ImportingConstructor]
        public SignatureHelpCommandHandler(
            [ImportMany] IEnumerable<Lazy<ISignatureHelpProvider, OrderableLanguageMetadata>> signatureHelpProviders,
            [ImportMany] IEnumerable<Lazy<IAsynchronousOperationListener, FeatureMetadata>> asyncListeners,
            [ImportMany] IEnumerable<Lazy<IIntelliSensePresenter<ISignatureHelpPresenterSession, ISignatureHelpSession>, OrderableMetadata>> signatureHelpPresenters)
            : this(ExtensionOrderer.Order(signatureHelpPresenters).Select(lazy => lazy.Value).FirstOrDefault(),
                   signatureHelpProviders, asyncListeners)
        {
        }

        // For testing purposes.
        public SignatureHelpCommandHandler(
            IIntelliSensePresenter<ISignatureHelpPresenterSession, ISignatureHelpSession> signatureHelpPresenter,
            [ImportMany] IEnumerable<Lazy<ISignatureHelpProvider, OrderableLanguageMetadata>> signatureHelpProviders,
            [ImportMany] IEnumerable<Lazy<IAsynchronousOperationListener, FeatureMetadata>> asyncListeners)
        {
            _signatureHelpProviders = ExtensionOrderer.Order(signatureHelpProviders);
            _asyncListeners = asyncListeners;
            _signatureHelpPresenter = signatureHelpPresenter;
        }

        private bool TryGetController(EditorCommandArgs args, out Controller controller)
        {
            AssertIsForeground();

            // If args is `InvokeSignatureHelpCommandArgs` then sig help was explicitly invoked by the user and should
            // be shown whether or not the option is set.
            if (!(args is InvokeSignatureHelpCommandArgs) && !args.SubjectBuffer.GetFeatureOnOffOption(SignatureHelpOptions.ShowSignatureHelp))
            {
                controller = null;
                return false;
            }

            // If we don't have a presenter, then there's no point in us even being involved.  Just
            // defer to the next handler in the chain.
            if (_signatureHelpPresenter == null)
            {
                controller = null;
                return false;
            }

            controller = Controller.GetInstance(
                args, _signatureHelpPresenter,
                new AggregateAsynchronousOperationListener(_asyncListeners, FeatureAttribute.SignatureHelp),
                _signatureHelpProviders);

            return true;
        }

        private bool TryGetControllerCommandHandler<TCommandArgs>(TCommandArgs args, out VisualStudio.Commanding.ICommandHandler commandHandler)
            where TCommandArgs : EditorCommandArgs
        {
            AssertIsForeground();
            if (!TryGetController(args, out var controller))
            {
                commandHandler = null;
                return false;
            }

            commandHandler = controller;
            return true;
        }

        private VisualStudio.Commanding.CommandState GetCommandStateWorker<TCommandArgs>(
            TCommandArgs args,
            Func<VisualStudio.Commanding.CommandState> nextHandler)
            where TCommandArgs : EditorCommandArgs
        {
            AssertIsForeground();
            return TryGetControllerCommandHandler(args, out var commandHandler)
                ? CommandHandlerUtils.GetCommandState(commandHandler, args, nextHandler)
                : nextHandler();
        }

        private void ExecuteCommandWorker<TCommandArgs>(
            TCommandArgs args,
            Action nextHandler,
            CommandExecutionContext context)
            where TCommandArgs : EditorCommandArgs
        {
            AssertIsForeground();
            if (!TryGetControllerCommandHandler(args, out var commandHandler))
            {
                nextHandler();
            }
            else
            {
                CommandHandlerUtils.ExecuteCommand(commandHandler, args, nextHandler, context);
            }
        }

        VisualStudio.Commanding.CommandState IChainedCommandHandler<TypeCharCommandArgs>.GetCommandState(TypeCharCommandArgs args, System.Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            AssertIsForeground();
            return GetCommandStateWorker(args, nextHandler);
        }

        void IChainedCommandHandler<TypeCharCommandArgs>.ExecuteCommand(TypeCharCommandArgs args, System.Action nextHandler, CommandExecutionContext context)
        {
            AssertIsForeground();
            ExecuteCommandWorker(args, nextHandler, context);
        }

        VisualStudio.Commanding.CommandState IChainedCommandHandler<InvokeSignatureHelpCommandArgs>.GetCommandState(InvokeSignatureHelpCommandArgs args, Func<VisualStudio.Commanding.CommandState> nextHandler)
        {
            AssertIsForeground();
            return GetCommandStateWorker(args, nextHandler);
        }

        void IChainedCommandHandler<InvokeSignatureHelpCommandArgs>.ExecuteCommand(InvokeSignatureHelpCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            AssertIsForeground();
            ExecuteCommandWorker(args, nextHandler, context);
        }

        internal bool TryHandleEscapeKey(EscapeKeyCommandArgs commandArgs)
        {
            if (!TryGetController(commandArgs, out var controller))
            {
                return false;
            }

            return controller.TryHandleEscapeKey();
        }

        internal bool TryHandleUpKey(UpKeyCommandArgs commandArgs)
        {
            if (!TryGetController(commandArgs, out var controller))
            {
                return false;
            }

            return controller.TryHandleUpKey();
        }

        internal bool TryHandleDownKey(DownKeyCommandArgs commandArgs)
        {
            if (!TryGetController(commandArgs, out var controller))
            {
                return false;
            }

            return controller.TryHandleDownKey();
        }
    }
}
