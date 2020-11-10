using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The property object.
    /// </summary>
    public sealed class Property
    {
        /// <summary>
        /// Property name [max-length: 64]. Only alphanumeric characters, dots, underscores or hyphens are allowed.
        /// The names ‘item_id’ and ‘user_id’ are reserved.
        /// </summary>
        [JsonProperty("property_name")]
        public string PropertyName { get; internal set; }

        /// <summary>
        /// Property type, [max-length: 10]. See https://docs.api.crossingminds.com/properties.html#concept-properties-types.
        /// </summary>
        [JsonProperty("value_type")]
        public string ValueType { get; internal set; }

        /// <summary>
        /// Whether the property has many values.
        /// </summary>
        [JsonProperty("repeated")]
        public bool Repeated { get; internal set; }
    }
}
