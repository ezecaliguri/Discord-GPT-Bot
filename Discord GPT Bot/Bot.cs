using Discord_GPT_Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_GPT_Bot
{
    public class Bot
    {
        // propiedades requeridas por la libreria, el private set, es para evitar que se pueda sobreescribir en algun momento
        public DiscordClient Client { get; private set; }

        public InteractivityExtension Interactivity { get; private set; }

        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            // obtención de los datos del archivo json
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync(); 

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            // Configuración requerida por discord
            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            // Tiempo de espera máximo del bot para la lectura de comandos 
            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });


            // Configuracíón del prefijo del bot de discord

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            // Declaración de la clase que se va a utilizar como comando
            Commands.RegisterCommands<GptCommand>();
            
            // Conexión del bot
            await Client.ConnectAsync();

            // El delay es para evitar algún problema con el tiempo de conexión del cliente 
            await Task.Delay(-1);

        }
    }
}
