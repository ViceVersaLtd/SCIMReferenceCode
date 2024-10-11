//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;

namespace Microsoft.SCIM
{
    public abstract class ConfigurationFactory<TConfiguration, TException>
        where TException : Exception
    {
        public abstract TConfiguration Create(
            Lazy<TConfiguration> defaultConfiguration,
            out TException configurationError);
    }
}