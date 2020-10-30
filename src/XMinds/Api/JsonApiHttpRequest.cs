using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
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
            // TODO: define User-Agent header.
            //'User-Agent': f'CrossingMinds/{__version__} (Python/{PYV}; JSON)',
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
                httpRequestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
        }

        public async Task<TResponseModel> ParseResponseAsync<TResponseModel>(
            HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return default(TResponseModel);
            }

            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false); 
            if (responseJson == null)
            {
                return default(TResponseModel);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return JsonConvert.DeserializeObject<TResponseModel>(responseJson);
        }
    }
}
