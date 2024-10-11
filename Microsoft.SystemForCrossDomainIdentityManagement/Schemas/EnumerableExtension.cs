//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SCIM
{
    internal static class EnumerableExtension
    {
        public static int CheckedCount<T>(this IEnumerable<T> enumeration)
        {
            long longCount = enumeration.LongCount();
            int result = Convert.ToInt32(longCount);
            return result;
        }
    }
}
