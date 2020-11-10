using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The service account data.
    /// </summary>
    public sealed class ServiceAccount
    {
        /// <summary>
        /// The Service Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The account role. Choices: [root, manager, backend, frontend]
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; internal set; }
    }

}
