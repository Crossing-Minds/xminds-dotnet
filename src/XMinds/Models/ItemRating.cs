using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The user-item rating object. 
    /// </summary>
    public sealed class ItemRating 
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

        /// <summary>
        /// Rating timestamp.
        /// </summary>
        [JsonIgnore]
        public DateTime Timestamp
        {
            get => this.UnixTimestamp.ToDateTimeFromUnixDateTime();
            set => this.UnixTimestamp = value.ToUnixDateTime();
        }

        /// <summary>
        /// Rating timestamp in Unix date/time.
        /// </summary>
        [JsonProperty("timestamp")]
        public double UnixTimestamp { get; set; }

        public ItemRating() { }

        public ItemRating(object itemId, float rating, DateTime timestamp) 
        {
            this.ItemId = itemId;
            this.Rating = rating;
            this.Timestamp = timestamp;
        }

        public ItemRating(object itemId, float rating, double unixTimestamp)
        {
            this.ItemId = itemId;
            this.Rating = rating;
            this.UnixTimestamp = unixTimestamp;
        }
    }
}
