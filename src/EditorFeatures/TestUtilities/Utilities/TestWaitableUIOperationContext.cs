// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.CodeAnalysis.Editor.UnitTests.Utilities
{
    internal class TestWaitableUIOperationContext : VisualStudio.Utilities.IWaitableUIOperationContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _maxUpdates;
        private int _updates;
        private List<TestWaitableUIOperationScope> _scopes = new List<TestWaitableUIOperationScope>();

        public TestWaitableUIOperationContext(int maxUpdates)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _maxUpdates = maxUpdates;
        }

        public TestWaitableUIOperationContext()
            : this(int.MaxValue)
        {
        }

        public VisualStudio.Utilities.PropertyCollection Properties => new VisualStudio.Utilities.PropertyCollection();

        public int Updates
        {
            get { return _updates; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        public IEnumerable<VisualStudio.Utilities.IWaitableUIOperationScope> Scopes => _scopes;

        public bool AllowCancellation => _scopes.All(s => s.AllowCancellation);

        public string Description => string.Join("", _scopes.Select(s => s.Description));

        private void UpdateProgress()
        {
            var result = Interlocked.Increment(ref _updates);
            if (result > _maxUpdates)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
        }

        public VisualStudio.Utilities.IWaitableUIOperationScope AddScope(bool allowCancellation, string description)
        {
            var scope = new TestWaitableUIOperationScope(this, allowCancellation, description);
            _scopes.Add(scope);
            return scope;
        }

        private void RemoveScope(TestWaitableUIOperationScope scope)
        {
            _scopes.Remove(scope);
        }

        private class TestWaitableUIOperationScope : VisualStudio.Utilities.IWaitableUIOperationScope
        {
            private readonly TestWaitableUIOperationContext _context;

            public TestWaitableUIOperationScope(TestWaitableUIOperationContext context, bool allowCancellation, string description)
            {
                this.AllowCancellation = allowCancellation;
                this.Description = description;
                _context = context;
                this.ProgressTracker = new PlatformProgressTracker();
            }

            public VisualStudio.Utilities.IWaitableUIOperationContext Context => _context;

            public bool AllowCancellation { get; set; }

            public string Description { get; set; }

            public VisualStudio.Utilities.IProgressTracker ProgressTracker { get; }

            public void Dispose()
            {
                _context.RemoveScope(this);
            }
        }
    }
}
