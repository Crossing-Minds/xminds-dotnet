using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace XMinds.Api
{
    sealed class ApiClient : IDisposable
    {
        private const string ServerUrl = "https://api.crossingminds.com";
        private const string ApiVersion = "v1";

        private const int DefaultHttpRequestTimeout = 30; // in seconds.

        private readonly HttpClient httpClient = new HttpClient();

        private readonly IApiHttpRequest apiHttpRequest;

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout 
        {
            get => this.httpClient.Timeout;
            set => this.httpClient.Timeout = value;
        }

        public ApiClient(IApiHttpRequest apiHttpRequest, string serverUrl = null)
        {
            this.apiHttpRequest = apiHttpRequest;

            this.httpClient.Timeout = TimeSpan.FromSeconds(DefaultHttpRequestTimeout);
            this.httpClient.BaseAddress = new Uri($"{serverUrl ?? ServerUrl}/{ ApiVersion}");
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", 
                $"CrossingMinds/{typeof(ApiClient).GetTypeInfo().Assembly.GetName().Version} (C#; JSON)");
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        #endregion

        public void SetAuthJwtToken(string authJwtToken)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = authJwtToken != null ?
                new AuthenticationHeaderValue("Bearer", authJwtToken) : null;
        }

        public bool IsAuthJwtTokenSet()
        {
            return this.httpClient.DefaultRequestHeaders.Authorization != null;
        }

        public async Task<TResponseModel> SendRequestAsync<TResponseModel>(HttpMethod httpMethod, string path,
            Dictionary<string, object> queryParams = null,
            Dictionary<string, object> bodyParams = null, 
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = path;
            if (queryParams != null)
            {
                var queryParamsBuilder = new StringBuilder(1000);
                foreach(var queryParam in queryParams)
                {
                    // if queryParam is ICollection, that means we should pass array of the values for the same key.
                    if (queryParam.Value is System.Collections.ICollection collectionValues)
                    {
                        foreach(object value in collectionValues)
                        {
                            if (queryParamsBuilder.Length != 0)
                            {
                                queryParamsBuilder.Append("&");
                            }

                            queryParamsBuilder.Append($"{queryParam.Key}={Uri.EscapeDataString(value.ToString())}");
                        }
                    }
                    else
                    {
                        if (queryParamsBuilder.Length != 0)
                        {
                            queryParamsBuilder.Append("&");
                        }

                        queryParamsBuilder.Append($"{queryParam.Key}={Uri.EscapeDataString(queryParam.Value.ToString())}");
                    }
                }

                query = $"{path}?{queryParamsBuilder}";
            }

            using (var request = new HttpRequestMessage(httpMethod, query))
            {
                this.apiHttpRequest.PrepareRequestHeadersAndBody(request, bodyParams);

                using (var responseMessage = await this.httpClient.SendAsync(request, cancellationToken)
                    .ConfigureAwait(false))
                {

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        await this.ThrowRequestExceptionAsync(responseMessage, cancellationToken).ConfigureAwait(false);
                    }

                    return await this.apiHttpRequest.ParseResponseAsync<TResponseModel>(
                        responseMessage, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task ThrowRequestExceptionAsync(HttpResponseMessage httpResponseMessage, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var apiError = await this.apiHttpRequest.ParseResponseAsync<ApiError>(
                httpResponseMessage, cancellationToken)
                .ConfigureAwait(false); 

            var exception = CreateRequestException((int)httpResponseMessage.StatusCode, apiError);

            cancellationToken.ThrowIfCancellationRequested();

            throw exception;
        }

        private XMindsErrorException CreateRequestException(int httpStatusCode, ApiError apiError)
        {
            XMindsErrorException exception;
            if (httpStatusCode >= 500)
            {
                if (httpStatusCode == (int) HttpStatusCode.ServiceUnavailable)
                {
                    exception = new ServiceUnavailableException(apiError);
                }
                else
                {
                    exception = new ServerErrorException(httpStatusCode, apiError);
                }
            }
            else if (httpStatusCode >= 400)
            {
                if (httpStatusCode == (int) HttpStatusCode.Unauthorized 
                    && apiError?.ErrorCode == ErrorCode.AuthError)
                {
                    exception = new AuthErrorException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.Unauthorized 
                    && apiError?.ErrorCode == ErrorCode.JwtTokenExpired)
                {
                    exception = new JwtTokenExpiredException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.Unauthorized 
                    && apiError?.ErrorCode == ErrorCode.RefreshTokenExpired)
                {
                    exception = new RefreshTokenExpiredException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.BadRequest
                    && apiError?.ErrorCode == ErrorCode.WrongData)
                {
                    exception = new WrongDataException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.BadRequest
                    && apiError?.ErrorCode == ErrorCode.DuplicatedError)
                {
                    exception = new DuplicatedErrorException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.Forbidden
                    && apiError?.ErrorCode == ErrorCode.ForbiddenError)
                {
                    exception = new ForbiddenErrorException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.NotFound
                    && apiError?.ErrorCode == ErrorCode.NotFoundError)
                {
                    exception = new NotFoundErrorException(apiError);
                }
                else if (httpStatusCode == (int)HttpStatusCode.MethodNotAllowed
                    && apiError?.ErrorCode == ErrorCode.MethodNotAllowed)
                {
                    exception = new MethodNotAllowedException(apiError);
                }
                else if (httpStatusCode == (int)TooManyRequestsException.TooManyRequestsHttpStatusCode
                    && apiError?.ErrorCode == ErrorCode.TooManyRequests)
                {
                    exception = new TooManyRequestsException(apiError);
                }
                else
                {
                    exception = new ServerErrorException(httpStatusCode, apiError);
                }
            }
            else
            {
                exception = new ServerErrorException(httpStatusCode, apiError);
            }

            return exception;
        }
    }
}
