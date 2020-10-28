using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XMinds.Api
{
    sealed class ApiClient : IDisposable
    {
        private const string ServerUrl = "https://staging-api.crossingminds.com";

        private readonly HttpClient httpClient = new HttpClient();

        private readonly IApiHttpRequest apiHttpRequest;

        public ApiClient(IApiHttpRequest apiHttpRequest)
        {
            this.apiHttpRequest = apiHttpRequest;
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        public void SetAuthJwtToken(string authJwtToken)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authJwtToken);
        }

        public async Task<TResponseModel> SendRequestAsync<TResponseModel>(HttpMethod httpMethod, string path,
            Dictionary<string, object> bodyParams = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var url = $"{ServerUrl}/{path}";

            var request = new HttpRequestMessage(httpMethod, url);

            this.apiHttpRequest.PrepareRequestHeadersAndBody(request, bodyParams);

            var responseMessage = await this.httpClient.SendAsync(request, cancellationToken);

            // TODO: Review this.
            responseMessage.EnsureSuccessStatusCode();

            if (responseMessage.Content == null)
            {
                // TODO: Review this.
                return default(TResponseModel);
            }

            return await this.apiHttpRequest.ParseResponseContentAsync<TResponseModel>(
                responseMessage.Content, cancellationToken);
        }
    }
}
