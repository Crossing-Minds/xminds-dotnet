using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllItemPropertiesAsync method.
    /// </summary>
    public sealed class ListAllItemPropertiesResult
    {
        /// <summary>
        /// All properties.
        /// </summary>
        [JsonProperty("properties")]
        public IReadOnlyList<Property> Properties { get; internal set; }
    }
}
