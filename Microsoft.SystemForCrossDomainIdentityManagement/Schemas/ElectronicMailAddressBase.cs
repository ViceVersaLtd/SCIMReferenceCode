//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public class ElectronicMailAddressBase : TypedValue
    {
        internal ElectronicMailAddressBase()
        {
        }

        public const string Home = "home";
        public const string Other = "other";
        public const string Work = "work";
    }
}