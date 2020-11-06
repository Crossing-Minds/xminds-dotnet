using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of List All Databases API endpoint.
    /// </summary>
    public class ListAllDatabasesResult
    {
        /// <summary>
        /// Indicates whether or not there are more pages to request.
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Next page to request.
        /// </summary>
        [JsonProperty("next_page")]
        public int NextPage { get; internal set; }

        /// <summary>
        /// Databases page.
        /// </summary>
        [JsonProperty("databases")]
        public IReadOnlyCollection<Database> Databases { get; internal set; }
    }
}
