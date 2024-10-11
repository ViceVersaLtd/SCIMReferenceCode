//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.SCIM
{
    public interface IExtension
    {
        Type Controller { get; }
        JsonDeserializingFactory JsonDeserializingFactory { get; }
        string Path { get; }
        string SchemaIdentifier { get; }
        string TypeName { get; }

        bool Supports(HttpRequest request);
    }
}