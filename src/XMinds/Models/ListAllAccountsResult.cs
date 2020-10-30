using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The response of List All Accounts API endpoint.
    /// </summary>
    public class ListAllAccountsResult
    {
        /// <summary>
        /// The individual accounts.
        /// </summary>
        [JsonProperty("individual_accounts")]
        public IReadOnlyCollection<IndividualAccount> IndividualAccounts { get; internal set; }

        /// <summary>
        /// The service accounts.
        /// </summary>
        [JsonProperty("service_accounts")]
        public IReadOnlyCollection<ServiceAccount> ServiceAccounts { get; internal set; }

        public class IndividualAccount
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

        public class ServiceAccount
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
}
