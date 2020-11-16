using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The result of ListRecentBackgroundTasksAsync method.
    /// </summary>
    public sealed class ListRecentBackgroundTasksResult
    {
        /// <summary>
        /// Background tasks data.
        /// </summary>
        [JsonProperty("tasks")]
        public IReadOnlyList<BackgroundTask> Tasks { get; internal set; }
    }
}
