using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllUsersBulkAsync method.
    /// </summary>
    public sealed class ListAllUsersBulkResult
    {
        /// <summary>
        /// Users data.
        /// </summary>
        [JsonProperty("users")]
        public IReadOnlyList<User> Users { get; internal set; }

        /// <summary>
        /// Indicates whether or not there are more users to request
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Pagination cursor to use in next request to get more users.
        /// </summary>
        [JsonProperty("next_cursor")]
        public string NextCursor { get; internal set; }
    }
}
