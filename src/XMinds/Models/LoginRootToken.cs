using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Models
{
    public class LoginRootToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
