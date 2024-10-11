//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.SCIM
{
    public class PluralUnsecuredEventTokenFactory : UnsecuredEventTokenFactory
    {
        public PluralUnsecuredEventTokenFactory(string issuer)
            : base(issuer)
        {
        }

        public override IEventToken Create(IDictionary<string, object> events)
        {
            if (null == events)
            {
                throw new ArgumentNullException(nameof(events));
            }

            IEventToken result = new EventToken(this.Issuer, this.Header, events);
            return result;
        }
    }
}