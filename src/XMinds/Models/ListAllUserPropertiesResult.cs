using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllUserPropertiesAsync method.
    /// </summary>
    public sealed class ListAllUserPropertiesResult
    {
        /// <summary>
        /// All properties.
        /// </summary>
        [JsonProperty("properties")]
        public IReadOnlyList<Property> Properties { get; internal set; }
    }
}
