using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The individual account data.
    /// </summary>
    public sealed class IndividualAccount
    {
        /// <summary>
        /// The First Name.
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; internal set; }

        /// <summary>
        /// The Last Name.
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; internal set; }

        /// <summary>
        /// The Email Address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// The account role. Choices: [root, manager, backend, frontend]
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; internal set; }

        /// <summary>
        /// Account verified.
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; internal set; }
    }

}
