using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XMinds.Utils;

namespace XMinds.Api
{
    class JsonApiHttpRequest : IApiHttpRequest
    {
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>()
        {
            { "User-Agent", $"CrossingMinds/{typeof(ApiClient).GetTypeInfo().Assembly.GetName().Version} (C#; JSON)" },
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
                return default;
            }

            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false); 
            if (responseJson == null)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return JsonConvert.DeserializeObject<TResponseModel>(responseJson, new UntypedJsonConverter());
        }
    }
}
