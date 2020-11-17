using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The base class for XMinds specific exceptions.
    /// </summary>
    public class XMindsErrorException : Exception
    {
        /// <summary>
        /// HTTP Status Code.
        /// </summary>
        public int HttpStatusCode { get; private set; }

        /// <summary>
        /// Error code supported values in <c>ErrorCode</c> class.
        /// </summary>
        public int? ErrorCode { get; private set; }

        /// <summary>
        /// Error name.
        /// </summary>
        public string ErrorName { get; private set; }

        /// <summary>
        /// Optional. If specified the number of seconds to retry the request to resolve the error.
        /// </summary>
        public int? RetryAfter { get; private set; }

        /// <summary>
        /// Additional error data if available.
        /// </summary>
        public IDictionary<object, object> ErrorData
        {
            get
            {
                var dict = new Dictionary<object, object>(this.Data.Count);
                foreach(System.Collections.DictionaryEntry dictionaryEntry in this.Data)
                {
                    dict.Add(dictionaryEntry.Key, dictionaryEntry.Value);
                }

                return dict;
            }
        }

        internal XMindsErrorException(int httpStatusCode, ApiError apiError)
            : base(apiError?.Message)
        {
            this.HttpStatusCode = httpStatusCode;
            this.ErrorCode = apiError?.ErrorCode;
            this.ErrorName = apiError?.ErrorName;

            if (apiError?.ErrorData != null)
            {
                foreach(var dataItem in apiError?.ErrorData)
                {
                    this.Data.Add(dataItem.Key, dataItem.Value?.ToString());
                }

                if (apiError.ErrorData.TryGetValue("retry_after", out object retryAfter) && retryAfter != null)
                {
                    this.RetryAfter = (int)retryAfter;
                }
            }
        }

    }
}
