using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllItemsBulkAsync method.
    /// </summary>
    public sealed class ListAllItemsBulkResult
    {
        /// <summary>
        /// Items data.
        /// </summary>
        [JsonProperty("items")]
        public IReadOnlyList<Item> Items { get; internal set; }

        /// <summary>
        /// Indicates whether or not there are more items to request
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Pagination cursor to use in next request to get more items.
        /// </summary>
        [JsonProperty("next_cursor")]
        public string NextCursor { get; internal set; }
    }
}
