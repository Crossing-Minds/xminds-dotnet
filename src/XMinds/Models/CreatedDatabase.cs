using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The created database data.
    /// </summary>
    public class CreatedDatabase
    {
        /// <summary>
        /// Database ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }
    }
}
