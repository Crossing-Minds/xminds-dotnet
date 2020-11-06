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

        private string refreshToken = null;

        private Database database = null;

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
        /// Endpoint: POST login/root/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result with JWT Token.</returns>
        /// <exception cref="ArgumentException">Email or password is not specified.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name INCORRECT_PASSWORD 
        /// if the password is incorrect.</exception>
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
            this.refreshToken = null;
            this.database = null;

            return result;
        }

        /// <summary>
        /// Logins on a database with your account, using your email and password combination. 
        /// Endpoint: POST login/individual/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="dbId">Database ID.</param>
        /// <param name="frontendUserId">Optional. Frontend user ID, for accounts with frontend role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result login information.</returns>
        /// <exception cref="ArgumentException">Email, password, or dbId is not specified.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name INCORRECT_PASSWORD 
        /// if the password is incorrect.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name ACCOUNT_NOT_VERIFIED 
        /// if the email has not been verified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<LoginResult> LoginIndividualAsync(string email, string password, string dbId, 
            object frontendUserId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (password == null)
            {
                throw new ArgumentException(nameof(password));
            }

            if (string.IsNullOrEmpty(dbId))
            {
                throw new ArgumentException(nameof(dbId));
            }

            var bodyParams = new Dictionary<string, object>
            {
                { "email", email },
                { "password", password },
                { "db_id", dbId },
            };

            if (frontendUserId != null)
            {
                bodyParams.Add("frontend_user_id", frontendUserId);
            }

            var result = await this.SendRequestAsync<LoginResult>(HttpMethod.Post, "login/individual/",
                bodyParams: bodyParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(result.Token);
            this.refreshToken = result.RefreshToken;
            this.database = result.Database;

            return result;
        }

        /// <summary>
        /// Logins on a database with a service account, using a service name and password combination. 
        /// Endpoint: POST login/service/
        /// </summary>
        /// <param name="name">Service name.</param>
        /// <param name="password">Password.</param>
        /// <param name="dbId">Database ID.</param>
        /// <param name="frontendUserId">Optional. Frontend user ID, for accounts with frontend role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result login information.</returns>
        /// <exception cref="ArgumentException">Email, password, or dbId is not specified.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name INCORRECT_PASSWORD 
        /// if the password is incorrect.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<LoginResult> LoginServiceAsync(string name, string password, string dbId,
            object frontendUserId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (password == null)
            {
                throw new ArgumentException(nameof(password));
            }

            if (string.IsNullOrEmpty(dbId))
            {
                throw new ArgumentException(nameof(dbId));
            }

            var bodyParams = new Dictionary<string, object>
            {
                { "name", name },
                { "password", password },
                { "db_id", dbId },
            };

            if (frontendUserId != null)
            {
                bodyParams.Add("frontend_user_id", frontendUserId);
            }

            var result = await this.SendRequestAsync<LoginResult>(HttpMethod.Post, "login/service/",
                bodyParams: bodyParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(result.Token);
            this.refreshToken = result.RefreshToken;
            this.database = result.Database;

            return result;
        }

        /// <summary>
        /// Logins on a database with your account, using a refresh token. 
        /// Endpoint: POST login/refresh-token/
        /// </summary>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="password">Password.</param>
        /// <param name="dbId">Database ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result login information.</returns>
        /// <exception cref="ArgumentException">The refresh token is not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ACCOUNT_NOT_FOUND 
        /// if the account has been deleted.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name INCORRECT_REFRESH_TOKEN 
        /// if the refresh token is invalid.</exception>
        /// <exception cref="RefreshTokenExpiredException">RefreshTokenExpired with error name REFRESH_TOKEN_EXPIRED
        /// if the refresh token is expired.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<LoginResult> LoginRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException(nameof(refreshToken));
            }

            // We call directly ApiClient in this case to avoid loops.

            var result = await this.apiClient.SendRequestAsync<LoginResult>(HttpMethod.Post, "login/refresh-token/",
                bodyParams: new Dictionary<string, object>
                {
                    { "refresh_token", refreshToken },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(result.Token);
            this.refreshToken = result.RefreshToken;
            this.database = result.Database;

            return result;
        }

        #endregion

        #region Accounts methods

        /// <summary>
        /// Gets all the accounts that belong to the organization of the token.
        /// Endpoint: GET organizations/current/accounts/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllAccountsResult> ListAllAccountsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<ListAllAccountsResult>(HttpMethod.Get, "organizations/current/accounts/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Сreates a new account for an individual, identified by an email. 
        /// Endpoint: POST accounts/individual/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="role">Role, choices: [manager, backend, frontend].</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created account information.</returns>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_ACCOUNT
        /// if an individual account with the same email already exists.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<CreatedAccount> CreateIndividualAccountAsync(string email, string password, string role,
            string firstName, string lastName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(nameof(password));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException(nameof(role));
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException(nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException(nameof(lastName));
            }

            var result = await this.SendRequestAsync<CreatedAccount>(HttpMethod.Post, "accounts/individual/",
                bodyParams: new Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password },
                    { "role", role },
                    { "first_name", firstName },
                    { "last_name", lastName },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Deletes another individual account by email address that belong to the organization of the token. 
        /// Endpoint: DELETE accounts/individual/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ACCOUNT_NOT_FOUND 
        /// if the account does not exist.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteIndividualAccountAsync(string email,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete, "accounts/individual/",
                bodyParams: new Dictionary<string, object>
                {
                    { "email", email },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Сreates a new service account, identified by a service name. 
        /// Endpoint: POST accounts/service/
        /// </summary>
        /// <param name="name">Service name.</param>
        /// <param name="password">Password.</param>
        /// <param name="role">Role, choices: [manager, backend, frontend].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created account information.</returns>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_ACCOUNT
        /// if a service account with the same name already exists.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<CreatedAccount> CreateServiceAccountAsync(string name, string password, string role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(nameof(password));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException(nameof(role));
            }

            var result = await this.SendRequestAsync<CreatedAccount>(HttpMethod.Post, "accounts/service/",
                bodyParams: new Dictionary<string, object>
                {
                    { "name", name },
                    { "password", password },
                    { "role", role },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Deletes another individual account by email address that belong to the organization of the token. 
        /// Endpoint: DELETE accounts/service/
        /// </summary>
        /// <param name="name">Service name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ACCOUNT_NOT_FOUND 
        /// if the account does not exist.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteServiceAccountAsync(string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete, "accounts/service/",
                bodyParams: new Dictionary<string, object>
                {
                    { "name", name },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a new verification code to the email address of an individual account. 
        /// Endpoint: PUT accounts/resend-verification-code/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ACCOUNT_NOT_FOUND 
        /// if the email does not exist.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name ACCOUNT_ALREADY_VERIFIED 
        /// if the email has already been verified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task ResendVerificationCodeAsync(string email,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, "accounts/resend-verification-code/",
                bodyParams: new Dictionary<string, object>
                {
                    { "email", email },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the email of an individual account. 
        /// Endpoint: GET accounts/verify/
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <param name="code">Verification code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ACCOUNT_NOT_FOUND 
        /// if the email does not exist.</exception>
        /// <exception cref="AuthErrorException">AuthError with error name ACTIVATION_CODE_DOES_NOT_MATCH
        /// if the code is incorret.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task VerifyAsync(string email, string code,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException(nameof(code));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Get, "accounts/verify/",
                queryParams: new Dictionary<string, object>
                {
                    { "code", code },
                    { "email", email },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the account you’re logged to with your current token. 
        /// Endpoint: DELETE accounts/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If no current account logged in.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteCurrentAccountAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!this.apiClient.IsAuthJwtTokenSet())
            {
                throw new InvalidOperationException(
                    "No current account logged in. Execute one of login methods to set the current account.");
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete, "accounts/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.apiClient.SetAuthJwtToken(null);
            this.refreshToken = null;
            this.database = null;
        }

        #endregion

        #region Database methods

        /// <summary>
        /// Сreates a new database. 
        /// Endpoint: POST databases/
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="description">Database long description.</param>
        /// <param name="item_id_type">Item ID type. See https://docs.api.crossingminds.com/flexible-id.html#concept-flexible-id for details.</param>
        /// <param name="user_id_type">User ID type. See https://docs.api.crossingminds.com/flexible-id.html#concept-flexible-id for details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created account information.</returns>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_DB_NAME
        /// if a database with the same name already exists.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<CreatedDatabase> CreateDatabaseAsync(string name, string description, string item_id_type,
            string user_id_type, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException(nameof(description));
            }

            if (string.IsNullOrEmpty(item_id_type))
            {
                throw new ArgumentException(nameof(item_id_type));
            }

            if (string.IsNullOrEmpty(user_id_type))
            {
                throw new ArgumentException(nameof(user_id_type));
            }

            var result = await this.SendRequestAsync<CreatedDatabase>(HttpMethod.Post, "databases/",
                bodyParams: new Dictionary<string, object>
                {
                    { "name", name },
                    { "description", description },
                    { "item_id_type", item_id_type },
                    { "user_id_type", user_id_type },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Gets all databases for the organization you’re logged to with your current token. The result is paginated.
        /// Endpoint: GET databases/
        /// </summary>
        /// <param name="page">Optional. [min: 1] Page to be listed.</param>
        /// <param name="amt">Optional. [min: 1 max: 64] Amount of databases to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The database list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllDatabasesResult> ListAllDatabasesAsync(int? page = null, int? amt = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Dictionary<string, object> queryParams = null;
            if (page != null || amt != null)
            {
                queryParams = new Dictionary<string, object>();
                if (page != null)
                {
                    queryParams.Add("page", page);
                }

                if (amt != null)
                {
                    queryParams.Add("amt", amt);
                }
            }

            var result = await this.SendRequestAsync<ListAllDatabasesResult>(HttpMethod.Get, "databases/",
                queryParams: queryParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Gets the metadata for the database you’re logged to with your current token.
        /// Endpoint: GET databases/current
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The current database data.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<CurrentDatabase> GetCurrentDatabaseAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<CurrentDatabase>(HttpMethod.Get, "databases/current/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Deletes the database you’re logged to with your current token. 
        /// Endpoint: DELETE databases/current/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">If no current database.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteCurrentDatabaseAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.database == null)
            {
                throw new InvalidOperationException(
                    "No current database. Execute one of login methods that sets current database.");
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete, "databases/current/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            this.database = null;
        }

        /// <summary>
        /// Get status of database. Initially the database will be in “pending” status. Until the status
        /// switch to “ready” you will not be able to get recommendations.
        /// Endpoint: GET databases/current/status/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The current database status.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<CurrentDatabaseStatus> GetCurrentDatabaseStatusAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<CurrentDatabaseStatus>(HttpMethod.Get, "databases/current/status/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        #endregion 

        #region General code 

        private async Task<TResponseModel> SendRequestAsync<TResponseModel>(HttpMethod httpMethod, string path,
            Dictionary<string, object> queryParams = null,
            Dictionary<string, object> bodyParams = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await this.apiClient.SendRequestAsync<TResponseModel>(httpMethod, path, queryParams, bodyParams, 
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (JwtTokenExpiredException)
            {
                if (this.refreshToken == null)
                {
                    throw;
                }

                await this.LoginRefreshTokenAsync(this.refreshToken, cancellationToken)
                    .ConfigureAwait(false);

                return await this.apiClient.SendRequestAsync<TResponseModel>(httpMethod, path, queryParams, bodyParams,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        #endregion
    }
}
