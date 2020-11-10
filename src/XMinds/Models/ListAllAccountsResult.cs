using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListAllAccountsAsync method.
    /// </summary>
    public sealed class ListAllAccountsResult
    {
        /// <summary>
        /// The individual accounts.
        /// </summary>
        [JsonProperty("individual_accounts")]
        public IReadOnlyList<IndividualAccount> IndividualAccounts { get; internal set; }

        /// <summary>
        /// The service accounts.
        /// </summary>
        [JsonProperty("service_accounts")]
        public IReadOnlyList<ServiceAccount> ServiceAccounts { get; internal set; }

    }
}
