using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The created account data.
    /// </summary>
    public sealed class CreatedAccount
    {
        /// <summary>
        /// Account ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }
    }
}
