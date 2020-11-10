using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The current database counters.
    /// </summary>
    public sealed class DatabaseCounters
    {
        /// <summary>
        /// Amount of ratings.
        /// </summary>
        [JsonProperty("rating")]
        public int Rating { get; internal set; }

        /// <summary>
        /// Amount of users.
        /// </summary>
        [JsonProperty("user")]
        public int User { get; internal set; }

        /// <summary>
        /// Amount of items.
        /// </summary>
        [JsonProperty("item")]
        public int Item { get; internal set; }
    }
}
