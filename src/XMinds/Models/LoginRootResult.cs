using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of Login as Root API endpoint.
    /// </summary>
    public class LoginRootResult
    {
        /// <summary>
        /// The JWT Token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; internal set; }
    }
}
