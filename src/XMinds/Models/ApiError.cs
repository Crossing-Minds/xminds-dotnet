using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Models
{
    class ApiError
    {
        [JsonProperty("error_code")]
        public int? ErrorCode { get; set; }

        [JsonProperty("error_name")]
        public string ErrorName { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


        [JsonProperty("error_data")]
        public Dictionary<string, object> ErrorData { get; set; }
    }

}
