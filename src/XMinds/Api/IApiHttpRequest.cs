using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XMinds.Api
{
    interface IApiHttpRequest
    {
        void PrepareRequestHeadersAndBody(HttpRequestMessage httpRequestMessage, 
            Dictionary<string, object> bodyParams = null);

        Task<TResponseModel> ParseResponseAsync<TResponseModel>(
            HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);
    }
}
