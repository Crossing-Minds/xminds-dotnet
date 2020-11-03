using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The database object.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Database ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Database name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Database long description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// Item ID type. See https://docs.api.crossingminds.com/flexible-id.html#concept-flexible-id for details.
        /// </summary>
        [JsonProperty("item_id_type")]
        public string ItemIdType { get; internal set; }

        /// <summary>
        /// User ID type. See https://docs.api.crossingminds.com/flexible-id.html#concept-flexible-id for details.
        /// </summary>
        [JsonProperty("user_id_type")]
        public string UserIdType { get; internal set; }
    }
}
