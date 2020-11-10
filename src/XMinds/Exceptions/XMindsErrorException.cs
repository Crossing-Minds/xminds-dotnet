using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace XMinds
{
    public class XMindsErrorException : Exception
    {
        public int HttpStatusCode { get; private set; }
        public int? ErrorCode { get; private set; }
        public string ErrorName { get; private set; }
        public int? RetryAfter { get; private set; }

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

                object retryAfter = null;
                if (apiError.ErrorData.TryGetValue("retry_after", out retryAfter) && retryAfter != null)
                {
                    this.RetryAfter = (int)retryAfter;
                }
            }
        }

    }
}
