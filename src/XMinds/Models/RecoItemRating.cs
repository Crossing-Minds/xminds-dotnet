using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The recommendation item rating object. Used as a param to GetRecoSessionToItemsResultAsync method.
    /// </summary>
    public sealed class RecoItemRating 
    {
        /// <summary>
        /// The item id property. 
        /// </summary>
        [JsonProperty("item_id")]
        public object ItemId { get; set; }

        /// <summary>
        /// Rating value. [min: 1 max: 10].
        /// </summary>
        [JsonProperty("rating")] 
        public float Rating { get; set; }

        public RecoItemRating() { }

        public RecoItemRating(object itemId, float rating) 
        {
            this.ItemId = itemId;
            this.Rating = rating;
        }
    }
}
