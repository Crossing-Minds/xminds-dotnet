using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of LoginRootAsync method.
    /// </summary>
    public sealed class LoginRootResult
    {
        /// <summary>
        /// JWT Token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; internal set; }
    }
}
