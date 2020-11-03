﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of Login as Individual, Login as Service and Login with Refresh Token API endpoint.
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// The JWT Token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; internal set; }

        /// <summary>
        /// The Refresh token.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; internal set; }

        /// <summary>
        /// The database object.
        /// </summary>
        [JsonProperty("database")]
        public Database Database { get; internal set; }

    }
}
