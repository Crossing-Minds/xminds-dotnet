using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XMinds.Api;

namespace XMinds
{
    /// <summary>
    /// The class is a .NET wrapper around Crossing Minds API. The class implements all the API endpoints.
    /// It also handles the logic to automatically refresh a new JWT token using the refresh token.
    /// </summary>
    public sealed class CrossingMindsApiClient : IDisposable
    {
        private ApiClient apiClient = new ApiClient(new JsonApiHttpRequest());

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get => this.apiClient.Timeout;
            set => this.apiClient.Timeout = value;
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            this.apiClient.Dispose();
        }

        #endregion

        #region Login methods

        /// <summary>
        /// Logins with the root account, without selecting any database. 
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result with JWT Token.</returns>
        /// <exception cref="ArgumentException">Email or password is not specified.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name INCORRECT_PASSWORD if the password is incorrect.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<LoginRootResult> LoginRootAsync(string email, string password,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (password == null)
            {
                throw new ArgumentException(nameof(password));
            }

            var result = await this.SendRequestAsync<LoginRootResult>(HttpMethod.Post, "login/root/",
                bodyParams: new Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password }
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(result.Token);

            return result;
        }

        #endregion

        #region Accounts API endpoints

        /// <summary>
        /// Gets all the accounts that belong to the organization of the token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The JWT Token.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllAccountsResult> ListAllAccountsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<ListAllAccountsResult>(HttpMethod.Get, "organizations/accounts/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        private async Task<TResponseModel> SendRequestAsync<TResponseModel>(HttpMethod httpMethod, string path,
            Dictionary<string, object> bodyParams = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.apiClient.SendRequestAsync<TResponseModel>(httpMethod, path, bodyParams, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
