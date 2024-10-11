//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class FeatureBase
    {
        [DataMember(Name = AttributeNames.Supported)]
        public bool Supported
        {
            get;
            set;
        }
    }
}