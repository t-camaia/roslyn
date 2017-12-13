// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.UnitTests.Utilities
{
    internal class TestCommandExecutionContext : CommandExecutionContext
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
}
