//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public interface IBulkUpdateOperationState : IBulkOperationState<IPatch>
    {
        IReadOnlyCollection<IBulkCreationOperationContext> Dependencies { get; }
        IBulkCreationOperationContext Parent { get; }

        void AddDependency(IBulkCreationOperationContext dependency);
    }
}
