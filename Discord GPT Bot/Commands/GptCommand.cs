using Discord_GPT_Bot.ExternalApis;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Discord_GPT_Bot.Commands
{
    public class GptCommand : BaseCommandModule
    {

        // Este comando informa  todo lo que se puede realizar a través del bot

        [Command("help")]
        public async Task AyudaGptCommand(CommandContext ctx)
        {
            string commands = "\n" + "**!chatgpt**: Permite realizar una pregunta al chatGpt" + "\n" + "\n" +
                              "**!imageGpt**: Permite generar una imagen de 512*512 pixeles en base a un texto otorgado " + "\n" + "\n" +
                              "**!audioGpt**: Permite realizar una transcripción. Extensiones validas: .mp3, .wav, .m4a" + "\n" + "\n" +
                              "**!audioGptLenguajes**: Obtiene la lista de lenguajes soportados para la transcripción" ;


            var help = new DiscordEmbedBuilder()
            {
                Title = ":clipboard: **Lista de Comandos** :clipboard:",
                Color = DiscordColor.Green,
                Description = commands,
                
            };

            await ctx.Channel.SendMessageAsync(embed: help);
        }

        // Comando para enviar consultas al chatGPT
        
        [Command("chatGpt")]
        public async Task ChatGptCommand(CommandContext ctx, params string[] Question) // Params string[] lo utilice simplemente para evitar el uso de comillas en el envío del parametro
        {
            // Instancia la clase
            var chatgpt = new ChatGpt();

            // Realiza la solicitud
            string response = await chatgpt.ResponseGenerateChatGpt(string.Join(" ", Question));

            // El bot envía el resultado, en el canal que el usuario realizo el uso del comando
            await ctx.Channel.SendMessageAsync(response);

        }


        // Comando para solicitar una imagen a través de un texto

        [Command("imageGpt")]
        public async Task ImageGptCommand(CommandContext ctx, params string[] Question)
        {
            
            var chatgpt = new ChatGpt();
            string linkImage = await chatgpt.ImageGenerateChatGpt(string.Join(" ", Question));

            // Envía un link directamente como respuesta, discord automáticamente lo adapta
            await ctx.Channel.SendMessageAsync(linkImage);

        }

        // Comando para solicitar el texto que hay en un audio

        [Command("AudioGpt")]
        public async Task AudioGptCommand(CommandContext ctx)
        {     

            var chatGpt = new ChatGpt();

            // Obtención del nombre del archivo 
            string nombreArchivo =  ctx.Message.Attachments[0].FileName.ToString();


            // Verifica que la extensión del archivo sea la correcta (.wav,.mp3 o .m4a)
            if (chatGpt.CheckedDocumentExtension(nombreArchivo))
            {
                //Obtiene el link en el cual discord tiene alojado el archivo 
                string url = ctx.Message.Attachments[0].Url.ToString();


                string response = await chatGpt.AudioTextGenerate(url, nombreArchivo);

                await ctx.Channel.SendMessageAsync(response);

            } else
            {
                await ctx.Channel.SendMessageAsync("La extension valida para utilizar este comando es .wav, .mp3 o .m4a");

            }


            
        }

        // Comando que informa los lenguajes disponibles para que el GPT pueda transcribir 

        [Command("audioGptLenguajes")]

        public async Task AGLCommander(CommandContext ctx)
        {
            var lenguajes = new DiscordEmbedBuilder()
            {
                Title = "**Lenguajes Disposibles para Transcripciones**",
                Description = "Afrikaans, Arabic, Armenian, Azerbaijani, Belarusian, Bosnian, Bulgarian, Catalan, Chinese, " +
                              "Croatian, Czech, Danish, Dutch, English, Estonian, Finnish, French, Galician, German, Greek, " +
                              "Hebrew, Hindi, Hungarian, Icelandic, Indonesian, Italian, Japanese, Kannada, Kazakh, Korean, " +
                              "Latvian, Lithuanian, Macedonian, Malay, Marathi, Maori, Nepali, Norwegian, Persian, Polish, " +
                              "Portuguese, Romanian, Russian, Serbian, Slovak, Slovenian, Spanish, Swahili, Swedish, Tagalog, " +
                              "Tamil, Thai, Turkish, Ukrainian, Urdu, Vietnamese, and Welsh.",
                Color = DiscordColor.Red,

            };

            await ctx.Channel.SendMessageAsync(embed: lenguajes);
        }
    }
}
