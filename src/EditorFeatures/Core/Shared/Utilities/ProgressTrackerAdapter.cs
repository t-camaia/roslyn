// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Shared.Utilities;

namespace Microsoft.CodeAnalysis.Editor.Shared.Utilities
{
    internal class ProgressTrackerAdapter : IProgressTracker
    {
        private readonly VisualStudio.Utilities.IProgressTracker _platformProgressTracker;

        public ProgressTrackerAdapter(VisualStudio.Utilities.IProgressTracker platformProgressTracker)
        {
            Requires.NotNull(platformProgressTracker, nameof(platformProgressTracker));
            _platformProgressTracker = platformProgressTracker;
        }

        public int CompletedItems => _platformProgressTracker.CompletedItems;

        public int TotalItems => _platformProgressTracker.TotalItems;

        public void AddItems(int count)
        {
            _platformProgressTracker.AddItems(count);
        }

        public void Clear()
        {
            _platformProgressTracker.Clear();
        }

        public void ItemCompleted()
        {
            _platformProgressTracker.ItemCompleted();
        }
    }
}
