using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The current database data.
    /// </summary>
    public sealed class CurrentDatabase
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

        /// <summary>
        /// Counters data.
        /// </summary>
        [JsonProperty("counters")]
        public DatabaseCounters Counters { get; internal set; }

        /// <summary>
        /// Extra meta data, as unvalidated JSON.
        /// </summary>
        [JsonProperty("metadata")]
        public IReadOnlyDictionary<string, object> Metadata { get; internal set; }

    }
}
