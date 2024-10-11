//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.SCIM
{
    internal static class DateTimeExtension
    {
        private const string FormatStringRoundtrip = "O";

        public static string ToRoundtripString(this DateTime dateTime)
        {
            string result = dateTime.ToString(DateTimeExtension.FormatStringRoundtrip, CultureInfo.InvariantCulture);
            return result;
        }
    }
}
