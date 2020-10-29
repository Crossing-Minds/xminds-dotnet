using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XMinds.Exceptions;
using XMinds.Models;

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
            if (!responseMessage.IsSuccessStatusCode)
            {
                await this.ThrowRequestExceptionAsync(responseMessage, cancellationToken);
            }

            return await this.apiHttpRequest.ParseResponseAsync<TResponseModel>(
                responseMessage, cancellationToken);
        }

        private async Task ThrowRequestExceptionAsync(HttpResponseMessage httpResponseMessage, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var apiError = await this.apiHttpRequest.ParseResponseAsync<ApiError>(
                httpResponseMessage, cancellationToken);

            var exception = CreateRequestException((int)httpResponseMessage.StatusCode, apiError);

            cancellationToken.ThrowIfCancellationRequested();

            throw exception;
        }

        private XMindsErrorException CreateRequestException(int httpStatusCode, ApiError apiError)
        {
            XMindsErrorException exception = null;
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
                    exception = new RefreshTokenExpired(apiError);
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
                else
                {
                    //TODO: review this.
                    exception = new ServerErrorException(httpStatusCode, apiError);
                }
            }
            else
            {
                //TODO: review this.
                exception = new ServerErrorException(httpStatusCode, apiError);
            }

            return exception;
        }
    }
}
