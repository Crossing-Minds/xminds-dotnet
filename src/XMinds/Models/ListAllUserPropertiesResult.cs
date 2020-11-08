using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of List All User-Properties API endpoint.
    /// </summary>
    public class ListAllUserPropertiesResult
    {
        /// <summary>
        /// All properties.
        /// </summary>
        [JsonProperty("properties")]
        public IReadOnlyCollection<Property> Properties { get; internal set; }
    }
}
