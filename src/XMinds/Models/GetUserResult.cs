﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of GetUserAsync method.
    /// </summary>
    public sealed class GetUserResult
    {
        /// <summary>
        /// The user data.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; internal set; }
    }
}
