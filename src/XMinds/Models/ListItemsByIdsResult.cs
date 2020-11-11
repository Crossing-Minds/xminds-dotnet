using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListItemsByIdsAsync method.
    /// </summary>
    public sealed class ListItemsByIdsResult
    {
        /// <summary>
        /// Items data.
        /// </summary>
        [JsonProperty("items")]
        public IReadOnlyList<Item> Items { get; internal set; }
    }
}
