// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.Shared.Utilities
{
    /// <summary>
    /// An adapter between Roslyn's IWaitContext and editor's IWaitableUIOperationScope, which are basically 
    /// represent the same abstraction. The only place where it's needed so far is FindReferencesCommandHandler,
    /// which operates within IWaitableUIOperationScope, but calls to <see cref="IFindReferencesService"/>, which
    /// requires Roslyn's IWaitContext.
    /// Going forward this adapter can be deleted once Roslyn's IWaitContext/Indicator is retired in favor of editor's 
    /// IWaitableUIOperationExecutor/Context.
    /// </summary>
    internal class WaitContextAdapter : IWaitContext
    {
        private readonly IWaitableUIOperationScope _platformWaitScope;

        public WaitContextAdapter(IWaitableUIOperationScope waitableScope)
        {
            Requires.NotNull(waitableScope, nameof(waitableScope));
            _platformWaitScope = waitableScope;
        }

        public CancellationToken CancellationToken => _platformWaitScope.Context.CancellationToken;

        public bool AllowCancel
        {
            get => _platformWaitScope.AllowCancellation;
            set => _platformWaitScope.AllowCancellation = value;
        }

        public string Message
        {
            get => _platformWaitScope.Description;
            set => _platformWaitScope.Description = value;
        }

        public CodeAnalysis.Shared.Utilities.IProgressTracker ProgressTracker
        {
            get
            {
                return _platformWaitScope.ProgressTracker != null ? new ProgressTrackerAdapter(_platformWaitScope.ProgressTracker) : null;
            }
        }

        public void Dispose()
        {
            _platformWaitScope.Dispose();
        }
    }
}
