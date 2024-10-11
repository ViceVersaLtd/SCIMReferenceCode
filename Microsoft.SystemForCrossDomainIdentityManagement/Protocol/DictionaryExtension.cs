//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SCIM
{
    internal static class DictionaryExtension
    {
        public static void Trim(this IDictionary<string, object> dictionary)
        {
            IReadOnlyCollection<string> keys = dictionary.Keys.ToArray();
            foreach (string key in keys)
            {
                object value = dictionary[key];
                if (null == value)
                {
                    dictionary.Remove(key);
                }

                IDictionary<string, object> dictionaryValue = value as IDictionary<string, object>;
                if (dictionaryValue != null)
                {
                    dictionaryValue.Trim();
                    if (dictionaryValue.Count <= 0)
                    {
                        dictionary.Remove(key);
                    }
                }
            }
        }
    }
}