using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The status of the current database.
    /// </summary>
    public sealed class CurrentDatabaseStatus
    {
        /// <summary>
        /// Either “pending” or “ready”.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; internal set; }
    }
}
