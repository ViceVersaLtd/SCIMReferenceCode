//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public abstract class RoleBase : TypedItem
    {
        [DataMember(Name = AttributeNames.Display, IsRequired = false, EmitDefaultValue = false)]
        public string Display
        {
            get;
            set;
        }

        [DataMember(Name = AttributeNames.Value, IsRequired = false, EmitDefaultValue = false)]
        public string Value
        {
            get;
            set;
        }
    }
}