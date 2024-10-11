//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Threading;

namespace Microsoft.SCIM
{
    public abstract class ProtocolJsonDeserializingFactory : ProtocolJsonDeserializingFactory<Schematized>
    {
    }

    public abstract class ProtocolJsonDeserializingFactory<T> : JsonDeserializingFactory<T>
    {
        private IJsonNormalizationBehavior jsonNormalizer;

        public override IJsonNormalizationBehavior JsonNormalizer
        {
            get
            {
                IJsonNormalizationBehavior result =
                    LazyInitializer.EnsureInitialized<IJsonNormalizationBehavior>(
                        ref this.jsonNormalizer,
                        () =>
                            new ProtocolJsonNormalizer());
                return result;
            }
        }
    }
}