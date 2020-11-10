using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListUsersByIdsAsync method.
    /// </summary>
    public sealed class ListUsersByIdsResult
    {
        /// <summary>
        /// Users data.
        /// </summary>
        [JsonProperty("users")]
        public IReadOnlyList<User> Users { get; internal set; }
    }
}
