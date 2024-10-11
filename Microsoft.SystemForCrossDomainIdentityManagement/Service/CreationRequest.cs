// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public sealed class CreationRequest : SystemForCrossDomainIdentityManagementRequest<Resource>
    {
        public CreationRequest(
            HttpRequest request,
            Resource payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
