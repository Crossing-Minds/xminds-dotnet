using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XMinds.Api
{
    class JsonApiHttpRequest : IApiHttpRequest
    {
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>()
        {
            //'User-Agent': f'CrossingMinds/{__version__} (Python/{PYV}; JSON)',
            { "Content-Type", "application/json" },
            { "Accept", "application/json" }
        };

        public void PrepareRequestHeadersAndBody(HttpRequestMessage httpRequestMessage, 
            Dictionary<string, object> bodyParams = null)
        {
            foreach (var header in headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            if (bodyParams != null)
            {
                var jsonBody = JsonConvert.SerializeObject(bodyParams);
                httpRequestMessage.Content = new StringContent(jsonBody, Encoding.UTF8);
            }
        }

        public async Task<TResponseModel> ParseResponseAsync<TResponseModel>(
            HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
            if (responseJson == null)
            {
                // TODO: Review this.
                return default(TResponseModel);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return JsonConvert.DeserializeObject<TResponseModel>(responseJson);
        }
    }
}
