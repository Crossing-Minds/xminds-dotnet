using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The rating object. 
    /// </summary>
    public sealed class UserItemRating 
    {
        /// <summary>
        /// The user id property. 
        /// </summary>
        [JsonProperty ("user_id")]
        public object UserId { get; set; }

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

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserItemRating() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="itemId">Item ID.</param>
        /// <param name="rating">Rating value. [min: 1 max: 10].</param>
        /// <param name="timestamp">Rating timestamp.</param>
        public UserItemRating(object userId, object itemId, float rating, DateTime timestamp)
        {
            this.UserId = userId;
            this.ItemId = itemId;
            this.Rating = rating;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="itemId">Item ID.</param>
        /// <param name="rating">Rating value. [min: 1 max: 10].</param>
        /// <param name="unixTimestamp">Rating timestamp in Unix date/time.</param>
        public UserItemRating(object userId, object itemId, float rating, double unixTimestamp)
        {
            this.UserId = userId;
            this.ItemId = itemId;
            this.Rating = rating;
            this.UnixTimestamp = unixTimestamp;
        }
    }
}
