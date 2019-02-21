using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GL.Multipart.Poc.Batching
{
    public static class HttpBatchingStringExtensions
    {
        public static HttpRequestMessage ReadAsHttpRequestMessage(this string rawRequest)
        {
            var requestLines = rawRequest.Split(Environment.NewLine);

            // Verb and location
            var requestInfo = requestLines[0].Split(" ");
            var method = requestInfo[0];
            var location = requestInfo[1];

            // Body index
            int bodyIndex = 0;

            // Headers
            var headerDictionary = new Dictionary<string, string>();
            for (int i = 1; i < requestLines.Length; i++)
            {
                var headerLine = requestLines[i];

                if (string.IsNullOrEmpty(headerLine))
                {
                    bodyIndex = i + 1;
                    break;
                }

                var columnIndex = headerLine.IndexOf(':');
                var headerName = headerLine.Substring(0, columnIndex);
                var headerValue = headerLine.Substring((columnIndex + 1)).Trim();
                headerDictionary.Add(headerName, headerValue);
            }

            // Body set
            string requestBody = null;
            if (method.Equals("POST") || method.Equals("PUT"))
            {
                if (requestLines.Length > bodyIndex)
                {
                    requestBody = requestLines[bodyIndex];
                }
            }

            var request = new HttpRequestMessage(new HttpMethod(method), location);
            foreach (var header in headerDictionary)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Content = new StringContent(requestBody);
            }

            return request;
        }
    }
}
