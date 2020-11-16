using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of GetRecoItemToItemsResultAsync and GetRecoUserToItemsResultAsync method.
    /// </summary>
    public sealed class RecoItemsResult
    {
        /// <summary>
        ///  Items IDs.
        /// </summary>
        [JsonProperty("items_id")]
        public IReadOnlyList<string> ItemsId { get; internal set; }

        /// <summary>
        /// Pagination cursor to use in next request to get more items.
        /// </summary>
        [JsonProperty("next_cursor")]
        public string NextCursor { get; internal set; }

        /// <summary>
        /// Optional. List of warnings.
        /// </summary>
        [JsonProperty("warnings")]
        public IReadOnlyList<string> Warnings { get; internal set; }
    }
}
