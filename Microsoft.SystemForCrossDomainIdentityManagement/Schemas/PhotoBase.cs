//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.SCIM
{
    [DataContract]
    public class PhotoBase : TypedValue
    {
        internal PhotoBase()
        {
        }

        public const string Photo = "photo";
        public const string Thumbnail = "thumbnail";
    }
}
