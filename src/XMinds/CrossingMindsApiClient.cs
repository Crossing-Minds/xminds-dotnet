using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XMinds.Api;

namespace XMinds
{
    public sealed class CrossingMindsApiClient : IDisposable
    {
        private ApiClient apiClient = new ApiClient(new JsonApiHttpRequest());

        public void Dispose()
        {
            this.apiClient.Dispose();
        }

        public async Task<LoginRootToken> LoginRootAsync(string email, string password,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException();
            }

            if (password == null)
            {
                throw new ArgumentException();
            }

            var result = await this.SendRequestAsync<LoginRootToken>(HttpMethod.Post, "login/root/",
                bodyParams: new Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password }
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(result.Token);

            return result;
        }

        private async Task<TResponseModel> SendRequestAsync<TResponseModel>(HttpMethod httpMethod, string path,
            Dictionary<string, object> bodyParams = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.apiClient.SendRequestAsync<TResponseModel>(httpMethod, path, bodyParams, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
