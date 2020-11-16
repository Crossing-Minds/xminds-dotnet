using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The background task object. 
    /// </summary>
    public sealed class BackgroundTask 
    {
        /// <summary>
        /// The task id. 
        /// </summary>
        [JsonProperty ("task_id")]
        public string TaskId { get; internal set; }

        /// <summary>
        /// The task name. 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The task start time.
        /// </summary>
        [JsonIgnore]
        public DateTime StartTime
        {
            get => this.UnixStartTime.ToDateTimeFromUnixDateTime();
            internal set => this.UnixStartTime = value.ToUnixDateTime();
        }

        /// <summary>
        /// Start time in Unix date/time.
        /// </summary>
        [JsonProperty("start_time")]
        public double UnixStartTime { get; internal set; }

        /// <summary>
        /// Execution status. The supported values are in <c>BackgroundTaskStatus</c> class. 
        /// </summary>
        [JsonProperty("status")] 
        public string Status { get; internal set; }

        /// <summary>
        /// Execution progress message.
        /// </summary>
        [JsonProperty("progress")]
        public string Progress { get; internal set; }
    }
}
