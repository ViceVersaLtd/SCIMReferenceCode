// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Microsoft.SCIM
{
    public static class RequestExtensions
    {
        private const string SegmentInterface =
            RequestExtensions.SegmentSeparator +
            SchemaConstants.PathInterface +
            RequestExtensions.SegmentSeparator;
        private const string SegmentSeparator = "/";

        private readonly static Lazy<char[]> SegmentSeparators =
            new Lazy<char[]>(
                () =>
                    SegmentSeparator.ToArray());

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of the 'this' parameter of an extension method")]
        public static Uri GetBaseResourceIdentifier(this HttpRequest request)
        {
            if (null == request.GetUri())
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidRequest);
            }

            string lastSegment =
                request.GetUri().AbsolutePath.Split(
                    RequestExtensions.SegmentSeparators.Value,
                    StringSplitOptions.RemoveEmptyEntries)
                .Last();
            if (string.Equals(lastSegment, SchemaConstants.PathInterface, StringComparison.OrdinalIgnoreCase))
            {
                return request.GetUri();
            }

            string resourceIdentifier = request.GetUri().AbsoluteUri;

            int indexInterface =
                resourceIdentifier
                .LastIndexOf(
                    RequestExtensions.SegmentInterface,
                    StringComparison.OrdinalIgnoreCase);

            if (indexInterface < 0)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidRequest);
            }

            string baseResource = resourceIdentifier.Substring(0, indexInterface);
            Uri result = new Uri(baseResource, UriKind.Absolute);
            return result;
        }

        public static bool TryGetRequestIdentifier(this HttpRequest request, out string requestIdentifier)
        {
            request.Headers.TryGetValue("client-id", out var _);
            requestIdentifier = Guid.NewGuid().ToString();
            return true;
        }
        public static Uri GetUri(this HttpRequest request)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Value,
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };
            return builder.Uri;
        }
        public class HttpResponseException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
            /// </summary>
            /// <param name="statusCode">The status code of the response.</param>
            public HttpResponseException(HttpStatusCode statusCode) : this(new HttpResponseMessage(statusCode))
            {
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
            /// </summary>
            /// <param name="response">The response message.</param>
            public HttpResponseException(HttpResponseMessage response)
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                Response = response;
            }

            /// <summary>
            /// Gets the <see cref="HttpResponseMessage"/> to return to the client.
            /// </summary>
            public HttpResponseMessage Response { get; private set; }
        }

        private static void Relate(
            this IBulkUpdateOperationContext context,
            IEnumerable<IBulkCreationOperationContext> creations)
        {
            if (null == creations)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (null == context.Method)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidContext);
            }

            if (null == context.Operation)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidContext);
            }

            try
            {
                dynamic operationDataJson = JsonConvert.DeserializeObject(context.Operation.Data.ToString());
                IReadOnlyCollection<PatchOperation2Combined> patchOperations = operationDataJson.Operations.ToObject<List<PatchOperation2Combined>>();
                PatchRequest2 patchRequest = new PatchRequest2(patchOperations);

                foreach (IBulkCreationOperationContext creation in creations)
                {
                    if (null == creation.Operation)
                    {
                        throw new InvalidOperationException(
                            SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
                    }

                    if (string.IsNullOrWhiteSpace(creation.Operation.Identifier))
                    {
                        throw new InvalidOperationException(
                            SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
                    }

                    if (patchRequest.References(creation.Operation.Identifier))
                    {
                        creation.AddDependent(context);
                        context.AddDependency(creation);
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }


        private static void Enlist(
            this IRequest<BulkRequest2> request,
            BulkRequestOperation operation,
            List<IBulkOperationContext> operations,
            List<IBulkCreationOperationContext> creations,
            List<IBulkUpdateOperationContext> updates)
        {
            if (null == operation)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (null == operations)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            if (null == creations)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (null == updates)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (null == operation.Method)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (HttpMethod.Post == operation.Method)
            {
                IBulkCreationOperationContext context = new BulkCreationOperationContext(request, operation);
                context.Relate(updates);

                (IBulkOperationContext item, int index) firstDependent =
                    operations
                    .Select(
                        (IBulkOperationContext item, int index) => (item, index))
                    .Where(
                        (candidateItem) =>
                            context
                            .Dependents
                            .Any(
                                (IBulkOperationContext dependentItem) =>
                                    dependentItem == candidateItem.item))
                    .OrderBy(
                        (item) =>
                            item.index)
                    .FirstOrDefault();

                if (firstDependent != default)
                {
                    operations.Insert(firstDependent.index, context);
                }
                else
                {
                    operations.Add(context);
                }
                creations.Add(context);
                operations.AddRange(context.Subordinates);
                updates.AddRange(context.Subordinates);
                return;
            }

            if (HttpMethod.Delete == operation.Method)
            {
                IBulkOperationContext context = new BulkDeletionOperationContext(request, operation);
                operations.Add(context);
                return;
            }

            if (ProtocolExtensions.PatchMethod == operation.Method)
            {
                IBulkUpdateOperationContext context = new BulkUpdateOperationContext(request, operation);
                context.Relate(creations);
                operations.Add(context);
                updates.Add(context);
                return;
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        public static Queue<IBulkOperationContext> EnqueueOperations(this IRequest<BulkRequest2> request)
        {
            if (null == request)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (null == request.Payload)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidRequest);
            }

            List<IBulkCreationOperationContext> creations = new List<IBulkCreationOperationContext>();
            List<IBulkUpdateOperationContext> updates = new List<IBulkUpdateOperationContext>();
            List<IBulkOperationContext> operations = new List<IBulkOperationContext>();

            foreach (BulkRequestOperation operation in request.Payload.Operations)
            {
                request.Enlist(operation, operations, creations, updates);
            }

            Queue<IBulkOperationContext> result = new Queue<IBulkOperationContext>(operations.Count);
            foreach (IBulkOperationContext operation in operations)
            {
                result.Enqueue(operation);
            }
            return result;
        }

        private static void Relate(
            this IBulkCreationOperationContext context,
            IEnumerable<IBulkUpdateOperationContext> updates)
        {
            if (null == updates)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (null == context.Method)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidContext);
            }

            if (null == context.Operation)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidContext);
            }

            if (string.IsNullOrWhiteSpace(context.Operation.Identifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            foreach (IBulkUpdateOperationContext update in updates)
            {
                switch (update.Operation.Data)
                {
                    case PatchRequest2 patchRequest:
                        if (patchRequest.References(context.Operation.Identifier))
                        {
                            context.AddDependent(update);
                            update.AddDependency(context);
                        }
                        break;
                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}
