// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System;
using System.Net.Http;

namespace Microsoft.SCIM
{
    internal class HttpStringResponseMessageFactory : HttpResponseMessageFactory<string>
    {
        private const string ArgumentNameContent = "content";

        public override HttpContent ProvideContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(HttpStringResponseMessageFactory.ArgumentNameContent);
            }

            HttpContent result = null;
            try
            {
                result = new StringContent(content);
                return result;
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
                    result = null;
                }

                throw;
            }
        }
    }
}
