using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_GPT_Bot
{
    // Configuracion de las propiedades para el archibo config.json
    internal struct ConfigJSON
    {
        [JsonProperty("TokenD")] // api key del bot de discord
        public string Token { get; private set; }

        [JsonProperty("prefix")] // Prefijo que se quiere utilizar para la lectura del bot 

        public string Prefix { get; private set; }

        [JsonProperty("ApiChatGPT")] // api key de GPT
        public string ApiChatGPT { get; private set; }

    }
}
