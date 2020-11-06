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

    }
}
