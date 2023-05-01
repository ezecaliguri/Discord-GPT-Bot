using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_GPT_Bot
{
    internal struct ConfigJSON
    {
        [JsonProperty("TokenD")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]

        public string Prefix { get; private set; }

        [JsonProperty("ApiChatGPT")]
        public string ApiChatGPT { get; private set; }

    }
}
