using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of GetItemAsync method.
    /// </summary>
    public sealed class GetItemResult
    {
        /// <summary>
        /// The item data.
        /// </summary>
        [JsonProperty("item")]
        public Item Item { get; internal set; }
    }
}
