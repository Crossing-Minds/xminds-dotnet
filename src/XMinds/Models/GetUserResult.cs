using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of Get User API endpoint.
    /// </summary>
    public class GetUserResult
    {
        /// <summary>
        /// The user data. The dictionary key is property name, dictionary value is property value.
        /// </summary>
        [JsonProperty("user")]
        public IDictionary<string, object> User { get; internal set; }
    }
}
