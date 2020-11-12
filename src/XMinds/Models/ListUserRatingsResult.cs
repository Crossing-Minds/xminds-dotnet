using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListUserRatingsAsync method.
    /// </summary>
    public sealed class ListUserRatingsResult
    {
        /// <summary>
        /// User rating.
        /// </summary>
        [JsonProperty("user_ratings")]
        public IReadOnlyList<ItemRating> UserRatings { get; internal set; }

        /// <summary>
        /// Indicates whether or not there are more pages to request.
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Next page to request.
        /// </summary>
        [JsonProperty("next_page")]
        public int NextPage { get; internal set; }
    }
}
