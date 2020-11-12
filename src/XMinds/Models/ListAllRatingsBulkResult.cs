using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllRatingsBulkAsync method.
    /// </summary>
    public sealed class ListAllRatingsBulkResult
    {
        /// <summary>
        /// Ratings data.
        /// </summary>
        [JsonProperty("ratings")]
        public IReadOnlyList<UserItemRating> Ratings { get; internal set; }

        /// <summary>
        /// Indicates whether or not there are more ratings to request
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Pagination cursor to use in next request to get more ratings.
        /// </summary>
        [JsonProperty("next_cursor")]
        public string NextCursor { get; internal set; }
    }
}
