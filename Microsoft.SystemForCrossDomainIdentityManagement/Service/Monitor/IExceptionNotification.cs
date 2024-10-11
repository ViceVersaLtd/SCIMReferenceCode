// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System;

namespace Microsoft.SCIM
{
    public interface IExceptionNotification : INotification<Exception>
    {
        bool Critical { get; set; }
    }
}
