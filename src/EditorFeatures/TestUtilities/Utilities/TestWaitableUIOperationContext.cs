// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.UnitTests.Utilities
{
    internal class TestWaitableUIOperationContext : AbstractWaitableUIOperationContext
    {
        CancellationTokenSource _cancellationTokenSource;
        private readonly int _maxUpdates;
        private int _updates;

        public TestWaitableUIOperationContext(int maxUpdates)
            :base(allowCancellation: false, description: "")
        {
            _maxUpdates = maxUpdates;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public TestWaitableUIOperationContext()
            : this(int.MaxValue)
        {
        }

        public int Updates
        {
            get { return _updates; }
        }

        public override CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        protected override void OnScopeProgressChanged(IWaitableUIOperationScope changedScope)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            var result = Interlocked.Increment(ref _updates);
            if (result > _maxUpdates)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
