using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XMinds.Api;
using XMinds.Utils;

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
        private string loginName = null;

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
            this.loginName = email;

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
            this.loginName = email;

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
            this.loginName = name;

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
            this.loginName = null;

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

            if (email.Equals(this.loginName, StringComparison.OrdinalIgnoreCase))
            {
                this.ResetLoginData();
            }    
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

            if (name.Equals(this.loginName, StringComparison.OrdinalIgnoreCase))
            {
                this.ResetLoginData();
            }
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

            this.ResetLoginData();
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

            this.ResetLoginData();
        }

        /// <summary>
        /// Gets status of database. Initially the database will be in “pending” status. Until the status
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

        #region Users Data and Properties

        /// <summary>
        /// Gets all user-properties for the current database.
        /// Endpoint: GET users-properties/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user-properties list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllUserPropertiesResult> ListAllUserPropertiesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<ListAllUserPropertiesResult>(HttpMethod.Get, "users-properties/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Creates a new user-property, identified by property_name (case-insensitive).
        /// Endpoint: POST users-properties/
        /// </summary>
        /// <param name="propertyName">Property name, [max-length: 64]. Only alphanumeric characters, dots, underscores 
        /// or hyphens are allowed. The names ‘item_id’ and ‘user_id’ are reserved.</param>
        /// <param name="valueType">Property type, [max-length: 10]. 
        /// See https://docs.api.crossingminds.com/properties.html#concept-properties-types</param>
        /// <param name="repeated">Optional. Whether the property has many values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_USER_PROPERTY
        /// if a user property with the same name already exists.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE 
        /// if value_type is invalid.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task CreateUserPropertyAsync(string propertyName, 
            string valueType, bool repeated = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            if (string.IsNullOrEmpty(valueType))
            {
                throw new ArgumentException(nameof(valueType));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Post, "users-properties/",
                bodyParams: new Dictionary<string, object>
                {
                    { "property_name", propertyName },
                    { "value_type", valueType },
                    { "repeated", repeated },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one user-property given its property name.
        /// Endpoint: GET users-properties/{str:property_name}/
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user-property data.</returns>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name USER_PROPERTY_NOT_FOUND
        /// if the property name cannot be found.</exception>
        /// /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<Property> GetUserPropertyAsync(string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            var result = await this.SendRequestAsync<Property>(HttpMethod.Get, 
                $"users-properties/{Uri.EscapeDataString(propertyName)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Deletes an user-property given by its name.
        /// Endpoint: DELETE users-properties/{str:property_name}/
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name USER_PROPERTY_NOT_FOUND
        /// if the property name cannot be found.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteUserPropertyAsync(string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete, 
                $"users-properties/{Uri.EscapeDataString(propertyName)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one user given its ID.
        /// Endpoint: GET users/{str:user_id}/
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user data.</returns>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name USER_NOT_FOUND
        /// if no user with the given user id can be found.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<GetUserResult> GetUserAsync(object userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            var result = await this.SendRequestAsync<GetUserResult>(HttpMethod.Get,
                $"users/{this.IdToUrlParam(userId)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Creates a new user, or update it if the user id already exists. All properties need to be defined beforehand.
        /// Endpoint: PUT users/{str:user_id}/
        /// </summary>
        /// <param name="user">User data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE 
        /// if the properties are invalid.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task CreateOrUpdateUserAsync(User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            if (user.UserId == null)
            {
                throw new ArgumentException($"{User.UserIdPropName} property");
            }

            // Removes user_id as API endpoint accepts user id in url.
            var userProps = new Dictionary<string, object>(user);
            userProps.Remove(User.UserIdPropName);

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, 
                $"users/{this.IdToUrlParam(user.UserId)}/",
                bodyParams: new Dictionary<string, object>
                {
                    { "user", userProps },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates many users in bulk, or update the ones for which the user_id already exist. 
        /// All properties need to be defined beforehand. The users data are sent to the server in chunks.
        /// Endpoint: PUT users-bulk/
        /// </summary>
        /// <param name="users">Users data. For each user object at least "user_id" property should be specified.</param>
        /// <param name="chunkSize">Optional. The chunk size (the number of users included in the chunk), users data
        /// are sent to the server in chunks of this size (default: 1K).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE 
        /// if the properties are invalid.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_USER_ID 
        /// if a user_id occurs multiple times in the same request</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        /// <remarks>In case of exception, the exception contains "last_processed_index" item in 
        /// Exception.Data dictionary. The item is the index of last successfuly sent user from the list. 
        /// The client can use the index to repeat the request starting from "last_processed_index" + 1 user. </remarks>
        public async Task CreateOrUpdateUsersBulkAsync(List<User> users, int chunkSize = 1024,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var chunkIndex = 0;

            try
            {
                if (users == null)
                {
                    throw new ArgumentException(nameof(users));
                }

                while (chunkIndex < users.Count)
                {
                    var actualChunkSize = Math.Min(chunkSize, users.Count - chunkIndex);
                    var usersChunk = new List<IDictionary<string, object>>(actualChunkSize);
                    for (var i = chunkIndex; i < chunkIndex + actualChunkSize; ++i)
                    {
                        usersChunk.Add(users[i]);
                    }

                    await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, $"users-bulk/",
                        bodyParams: new Dictionary<string, object>
                        {
                            { "users", usersChunk },
                        }, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    chunkIndex += actualChunkSize;
                }
            }
            catch (Exception ex)
            {
                // Adding the index of the last successfuly sent user from the list.
                // The client can use the index to repeat the request starting from index + 1 user.
                ex.Data.Add("last_processed_index", chunkIndex - 1);

                throw;
            }
        }

        /// <summary>
        /// Gets multiple users by page. The response is paginated, you can control the response amount and offset 
        /// using the query parameters amt and cursor.
        /// Endpoint: GET users-bulk/
        /// </summary>
        /// <param name="amt">Optional. [max: 500] Maximum amount of users returned, by default is 300.</param>
        /// <param name="page">Optional. Pagination cursor, typically from the NextCursor value from the previous response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The users list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllUsersBulkResult> ListAllUsersBulkAsync(int? amt = null, string cursor = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Dictionary<string, object> queryParams = null;
            if (amt != null || cursor != null)
            {
                queryParams = new Dictionary<string, object>();
                if (amt != null)
                {
                    queryParams.Add("amt", amt);
                }

                if (cursor != null)
                {
                    queryParams.Add("cursor", cursor);
                }
            }

            var result = await this.SendRequestAsync<ListAllUsersBulkResult>(HttpMethod.Get, "users-bulk/",
                queryParams: queryParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }


        /// <summary>
        /// Get multiple users given their IDs. The users in the response are not aligned with the input, 
        /// the missing users are simply not present in the result.
        /// Endpoint: POST users-bulk/list/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The users list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListUsersByIdsResult> ListUsersByIdsAsync(List<object> usersIds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (usersIds == null)
            {
                throw new ArgumentException(nameof(usersIds));
            }

            var result = await this.SendRequestAsync<ListUsersByIdsResult>(HttpMethod.Post, "users-bulk/list/",
                bodyParams: new Dictionary<string, object>
                {
                    { "users_id", usersIds },
                },
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Items Data and Properties

        /// <summary>
        /// Getы all item-properties for the current database.
        /// Endpoint: GET items-properties/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The item-properties list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllItemPropertiesResult> ListAllItemPropertiesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.SendRequestAsync<ListAllItemPropertiesResult>(HttpMethod.Get, "items-properties/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Creates a new item-property, identified by property_name (case-insensitive).
        /// Endpoint: POST items-properties/
        /// </summary>
        /// <param name="propertyName">Property name, [max-length: 64]. Only alphanumeric characters, dots, underscores 
        /// or hyphens are allowed. The names ‘item_id’ and ‘user_id’ are reserved.</param>
        /// <param name="valueType">Property type, [max-length: 10]. 
        /// See https://docs.api.crossingminds.com/properties.html#concept-properties-types</param>
        /// <param name="repeated">Optional. Whether the property has many values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_ITEM_PROPERTY
        /// if an item property with the same name already exists.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE
        /// if value_type is invalid.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task CreateItemPropertyAsync(string propertyName,
            string valueType, bool repeated = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            if (string.IsNullOrEmpty(valueType))
            {
                throw new ArgumentException(nameof(valueType));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Post, "items-properties/",
                bodyParams: new Dictionary<string, object>
                {
                    { "property_name", propertyName },
                    { "value_type", valueType },
                    { "repeated", repeated },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one item-property given its property name.
        /// Endpoint: GET items-properties/{str:property_name}/
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The item-property data.</returns>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ITEM_PROPERTY_NOT_FOUND
        /// if the property name cannot be found.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<Property> GetItemPropertyAsync(string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            var result = await this.SendRequestAsync<Property>(HttpMethod.Get,
                $"items-properties/{Uri.EscapeDataString(propertyName)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Deletes an item-property given by its name.
        /// Endpoint: DELETE items-properties/{str:property_name}/
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ITEM_PROPERTY_NOT_FOUND
        /// if the property name cannot be found.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteItemPropertyAsync(string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete,
                $"items-properties/{Uri.EscapeDataString(propertyName)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one item properties given its ID.
        /// Endpoint: GET items/{str:item_id}/
        /// </summary>
        /// <param name="itemId">Item Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user data.</returns>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name ITEM_NOT_FOUND 
        /// if no item with the given item_id can be found.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<GetItemResult> GetItemAsync(object itemId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (itemId == null)
            {
                throw new ArgumentException(nameof(itemId));
            }

            var result = await this.SendRequestAsync<GetItemResult>(HttpMethod.Get,
                $"items/{this.IdToUrlParam(itemId)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Creates a new item, or update it if the item id already exists. All properties need to be defined beforehand.
        /// Endpoint: PUT items/{str:item_id}/
        /// </summary>
        /// <param name="item">Item data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE 
        /// if the properties are invalid.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task CreateOrUpdateItemAsync(Item item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (item == null)
            {
                throw new ArgumentException(nameof(item));
            }

            if (item.ItemId == null)
            {
                throw new ArgumentException($"{Item.ItemIdPropName} property");
            }

            // Removes item_id as API endpoint accepts item id in url.
            var itemProps = new Dictionary<string, object>(item);
            itemProps.Remove(Item.ItemIdPropName);

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Put,
                $"items/{this.IdToUrlParam(item.ItemId)}/",
                bodyParams: new Dictionary<string, object>
                {
                    { "item", itemProps },
                }, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates many items in bulk, or update the ones for which the item_id already exist. 
        /// All properties need to be defined beforehand. The items data are sent to the server in chunks.
        /// Endpoint: PUT items-bulk/
        /// </summary>
        /// <param name="items">Items data. For each item object at least "item_id" property should be specified.</param>
        /// <param name="chunkSize">Optional. The chunk size (the number of items included in the chunk), items data
        /// are sent to the server in chunks of this size (default: 1K).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="WrongDataException">WrongData with error name WRONG_DATA_TYPE 
        /// if the properties are invalid.</exception>
        /// <exception cref="DuplicatedErrorException">DuplicatedError with error name DUPLICATED_ITEM_ID 
        /// if an item_id occurs multiple times in the same request.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        /// <remarks>In case of exception, the exception contains "last_processed_index" item in 
        /// Exception.Data dictionary. The item is the index of last successfuly sent item from the list. 
        /// The client can use the index to repeat the request starting from "last_processed_index" + 1 item. </remarks>
        public async Task CreateOrUpdateItemsBulkAsync(List<Item> items, int chunkSize = 1024,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var chunkIndex = 0;

            try
            {
                if (items == null)
                {
                    throw new ArgumentException(nameof(items));
                }

                while (chunkIndex < items.Count)
                {
                    var actualChunkSize = Math.Min(chunkSize, items.Count - chunkIndex);
                    var itemsChunk = new List<IDictionary<string, object>>(actualChunkSize);
                    for (var i = chunkIndex; i < chunkIndex + actualChunkSize; ++i)
                    {
                        itemsChunk.Add(items[i]);
                    }

                    await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, $"items-bulk/",
                        bodyParams: new Dictionary<string, object>
                        {
                            { "items", itemsChunk },
                        }, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    chunkIndex += actualChunkSize;
                }
            }
            catch (Exception ex)
            {
                // Adding the index of the last successfuly sent item from the list.
                // The client can use the index to repeat the request starting from index + 1 item.
                ex.Data.Add("last_processed_index", chunkIndex - 1);

                throw;
            }
        }

        /// <summary>
        /// Gets multiple items by page. The response is paginated, you can control the response amount and offset 
        /// using the query parameters amt and cursor.
        /// Endpoint: GET items-bulk/
        /// </summary>
        /// <param name="amt">Optional. [max: 500] Maximum amount of items returned, by default is 300.</param>
        /// <param name="page">Optional. Pagination cursor, typically from the NextCursor value from the previous response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The items list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllItemsBulkResult> ListAllItemsBulkAsync(int? amt = null, string cursor = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Dictionary<string, object> queryParams = null;
            if (amt != null || cursor != null)
            {
                queryParams = new Dictionary<string, object>();
                if (amt != null)
                {
                    queryParams.Add("amt", amt);
                }

                if (cursor != null)
                {
                    queryParams.Add("cursor", cursor);
                }
            }

            var result = await this.SendRequestAsync<ListAllItemsBulkResult>(HttpMethod.Get, "items-bulk/",
                queryParams: queryParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Get multiple items given their IDs. The items in the response are not aligned with the input, 
        /// the missing items are simply not present in the result.
        /// Endpoint: POST items-bulk/list/
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The items list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListItemsByIdsResult> ListItemsByIdsAsync(List<object> itemsIds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (itemsIds == null)
            {
                throw new ArgumentException(nameof(itemsIds));
            }

            var result = await this.SendRequestAsync<ListItemsByIdsResult>(HttpMethod.Post, "items-bulk/list/",
                bodyParams: new Dictionary<string, object>
                {
                    { "items_id", itemsIds },
                },
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region User Ratings

        /// <summary>
        /// Creates or updates a rating for a user and an item. If the rating exists for the tuple (user id, item id) 
        /// then it is updated, otherwise it is created.
        /// Endpoint: PUT users/{str:user_id}/ratings/{str:item_id}/
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="itemId">Item id.</param>
        /// <param name="rating">Rating value. [min: 1 max: 10].</param>
        /// <param name="timestamp">Optional. Rating timestamp (default: now).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task CreateOrUpdateRatingAsync(object userId, object itemId, float rating, 
            DateTime? timestamp = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            if (itemId == null)
            {
                throw new ArgumentException(nameof(itemId));
            }

            var ratingProps = new Dictionary<string, object>()
            {
                { "rating", rating }
            };

            if (timestamp != null)
            {
                ratingProps.Add("timestamp", timestamp.Value.ToUnixDateTime());
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Put,
                $"users/{this.IdToUrlParam(userId)}/ratings/{this.IdToUrlParam(itemId)}/",
                bodyParams: ratingProps, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a single rating for a given user.
        /// Endpoint: DELETE users/{str:user_id}/ratings/{str:item_id}/
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="itemId">Item Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="NotFoundErrorException">NotFoundError with error name USER_RATING_NOT_FOUND 
        /// if the user does not have a rating for this item.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteRatingAsync(object userId, object itemId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }
            
            if (itemId == null)
            {
                throw new ArgumentException(nameof(itemId));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete,
                $"users/{this.IdToUrlParam(userId)}/ratings/{this.IdToUrlParam(itemId)}/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets multiple items by page. The response is paginated, you can control the response amount and offset 
        /// using the query parameters amt and cursor.
        /// Endpoint: GET users/{str:user_id}/ratings/
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="amt">Optional. [min: 1 max: 64] Amount of ratings to return.</param>
        /// <param name="page">Optional. [min: 1] Page to be listed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The items list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListUserRatingsResult> ListUserRatingsAsync(object userId, 
            int? amt = null, int? page = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            Dictionary<string, object> queryParams = null;
            if (amt != null || page != null)
            {
                queryParams = new Dictionary<string, object>();
                if (amt != null)
                {
                    queryParams.Add("amt", amt);
                }

                if (page != null)
                {
                    queryParams.Add("page", page);
                }
            }

            var result = await this.SendRequestAsync<ListUserRatingsResult>(HttpMethod.Get, 
                $"users/{this.IdToUrlParam(userId)}/ratings/",
                queryParams: queryParams, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Creates or updates bulks of ratings for a single user and many items. 
        /// Endpoint: PUT users/{str:user_id}/ratings/
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="ratings">Ratings data.</param>
        /// <param name="chunkSize">Optional. The chunk size (the number of items included in the chunk), items data
        /// are sent to the server in chunks of this size (default: 1K).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        /// <remarks>In case of exception, the exception contains "last_processed_index" item in 
        /// Exception.Data dictionary. The item is the index of last successfuly sent rating from the list. 
        /// The client can use the index to repeat the request starting from "last_processed_index" + 1 rating. </remarks>
        public async Task CreateOrUpdateUserRatingsBulkAsync(object userId, List<ItemRating> ratings, int chunkSize = 1024,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var chunkIndex = 0;

            try
            {
                if (userId == null)
                {
                    throw new ArgumentException(nameof(userId));
                }

                if (ratings == null)
                {
                    throw new ArgumentException(nameof(ratings));
                }

                while (chunkIndex < ratings.Count)
                {
                    var actualChunkSize = Math.Min(chunkSize, ratings.Count - chunkIndex);
                    var itemsChunk = new List<ItemRating>(actualChunkSize);
                    for (var i = chunkIndex; i < chunkIndex + actualChunkSize; ++i)
                    {
                        itemsChunk.Add(ratings[i]);
                    }

                    await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, 
                        $"users/{this.IdToUrlParam(userId)}/ratings/",
                        bodyParams: new Dictionary<string, object>
                        {
                            { "ratings", itemsChunk },
                        }, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    chunkIndex += actualChunkSize;
                }
            }
            catch (Exception ex)
            {
                // Adding the index of the last successfuly sent item from the list.
                // The client can use the index to repeat the request starting from index + 1 ratings.
                ex.Data.Add("last_processed_index", chunkIndex - 1);

                throw;
            }
        }

        /// <summary>
        /// Deletes all ratings of a given user.
        /// Endpoint: DELETE users/{str:user_id}/ratings/
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task DeleteRatingAsync(object userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            await this.SendRequestAsync<VoidEntity>(HttpMethod.Delete,
                $"users/{this.IdToUrlParam(userId)}/ratings/",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates or updates large bulks of ratings for many users and many items. 
        /// Endpoint: PUT ratings-bulk/
        /// </summary>
        /// <param name="ratings">Ratings data.</param>
        /// <param name="chunkSize">Optional. The chunk size (the number of ratings included in the chunk), ratings data
        /// are sent to the server in chunks of this size (default: 1K).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">If input parameters are not specified.</exception>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        /// <remarks>In case of exception, the exception contains "last_processed_index" item in 
        /// Exception.Data dictionary. The item is the index of last successfuly sent rating from the list. 
        /// The client can use the index to repeat the request starting from "last_processed_index" + 1 rating. </remarks>
        public async Task CreateOrUpdateRatingsBulkAsync(List<UserItemRating> ratings, int chunkSize = 1024,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var chunkIndex = 0;

            try
            {
                if (ratings == null)
                {
                    throw new ArgumentException(nameof(ratings));
                }

                while (chunkIndex < ratings.Count)
                {
                    var actualChunkSize = Math.Min(chunkSize, ratings.Count - chunkIndex);
                    var itemsChunk = new List<UserItemRating>(actualChunkSize);
                    for (var i = chunkIndex; i < chunkIndex + actualChunkSize; ++i)
                    {
                        itemsChunk.Add(ratings[i]);
                    }

                    await this.SendRequestAsync<VoidEntity>(HttpMethod.Put, $"ratings-bulk/",
                        bodyParams: new Dictionary<string, object>
                        {
                            { "ratings", itemsChunk },
                        }, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    chunkIndex += actualChunkSize;
                }
            }
            catch (Exception ex)
            {
                // Adding the index of the last successfuly sent item from the list.
                // The client can use the index to repeat the request starting from index + 1 item.
                ex.Data.Add("last_processed_index", chunkIndex - 1);

                throw;
            }
        }

        /// <summary>
        /// Gets multiple items by page. The response is paginated, you can control the response amount and offset 
        /// using the query parameters amt and cursor.
        /// Endpoint: GET items-bulk/
        /// </summary>
        /// <param name="amt">Optional. [max: 500] Maximum amount of items returned, by default is 300.</param>
        /// <param name="page">Optional. Pagination cursor, typically from the NextCursor value from the previous response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The items list.</returns>
        /// <exception cref="XMindsErrorException">Other Crossing Minds API exceptions.</exception>
        /// <exception cref="HttpRequestException">A network error occurs.</exception>
        /// <exception cref="TaskCanceledException">The call was cancelled or timeout occurs.</exception>
        public async Task<ListAllRatingsBulkResult> ListAllRatingsBulkAsync(int? amt = null, string cursor = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Dictionary<string, object> queryParams = null;
            if (amt != null || cursor != null)
            {
                queryParams = new Dictionary<string, object>();
                if (amt != null)
                {
                    queryParams.Add("amt", amt);
                }

                if (cursor != null)
                {
                    queryParams.Add("cursor", cursor);
                }
            }

            var result = await this.SendRequestAsync<ListAllRatingsBulkResult>(HttpMethod.Get, "ratings-bulk/",
                queryParams: queryParams, cancellationToken: cancellationToken)
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

        private string IdToUrlParam(object id)
        {
            var userIdString = Convert.ToString(id, CultureInfo.InvariantCulture);

            return Uri.EscapeDataString(userIdString);
        }

        private void ResetLoginData()
        {
            this.apiClient.SetAuthJwtToken(null);
            this.refreshToken = null;
            this.database = null;
            this.loginName = null;
        }

        #endregion
    }
}
