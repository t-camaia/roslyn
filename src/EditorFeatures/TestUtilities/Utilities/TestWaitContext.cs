// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.UnitTests.Utilities
{
    public sealed class TestWaitContext : IWaitContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _maxUpdates;
        private int _updates;
        private readonly IProgressTracker _progressTracker;

        public TestWaitContext(int maxUpdates)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _maxUpdates = maxUpdates;
            _progressTracker = new ProgressTracker((_1, _2) => UpdateProgress());
        }

        IProgressTracker IWaitContext.ProgressTracker => _progressTracker;

        public int Updates
        {
            get { return _updates; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        private void UpdateProgress()
        {
            var result = Interlocked.Increment(ref _updates);
            if (result > _maxUpdates)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public bool AllowCancel
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public string Message
        {
            get
            {
                return "";
            }

            set
            {
            }
        }

        public void Dispose()
        {
        }
    }

    internal class PlatformProgressTracker : VisualStudio.Utilities.IProgressTracker
    {
        private int _completedItems;
        private int _totalItems;

        private readonly Action<int, int> _updateActionOpt;

        public PlatformProgressTracker()
            : this(null)
        {
        }

        public PlatformProgressTracker(Action<int, int> updateActionOpt)
        {
            _updateActionOpt = updateActionOpt;
        }

        public int CompletedItems => _completedItems;

        public int TotalItems => _totalItems;

        public void AddItems(int count)
        {
            Interlocked.Add(ref _totalItems, count);
            Update();
        }

        public void ItemCompleted()
        {
            Interlocked.Increment(ref _completedItems);
            Update();
        }

        public void Clear()
        {
            _totalItems = 0;
            _completedItems = 0;
            Update();
        }

        private void Update()
        {
            _updateActionOpt?.Invoke(_completedItems, _totalItems);
        }
    }

    public class TestCommandExecutionContext : CommandExecutionContext
    {
        public TestCommandExecutionContext()
            : this(new TestWaitableUIOperationContext())
        {
        }

        public TestCommandExecutionContext(VisualStudio.Utilities.IWaitableUIOperationContext waitContext) 
            : base(waitContext)
        {
        }
    }

    public sealed class TestWaitableUIOperationContext : VisualStudio.Utilities.IWaitableUIOperationContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _maxUpdates;
        private int _updates;
        private readonly VisualStudio.Utilities.IProgressTracker _progressTracker;

        public TestWaitableUIOperationContext(int maxUpdates = int.MaxValue)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _maxUpdates = maxUpdates;
            _progressTracker = new PlatformProgressTracker((_1, _2) => UpdateProgress());
        }

        VisualStudio.Utilities.IProgressTracker VisualStudio.Utilities.IWaitableUIOperationContext.ProgressTracker => _progressTracker;

        public VisualStudio.Utilities.PropertyCollection Properties => new VisualStudio.Utilities.PropertyCollection();

        public int Updates
        {
            get { return _updates; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        private void UpdateProgress()
        {
            var result = Interlocked.Increment(ref _updates);
            if (result > _maxUpdates)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public bool AllowCancellation
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public string Description
        {
            get
            {
                return "";
            }

            set
            {
            }
        }

        public void Dispose()
        {
        }
    }
}
