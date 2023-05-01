using Discord_GPT_Bot.ExternalApis;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Discord_GPT_Bot.Commands
{
    public class GptCommand : BaseCommandModule
    {

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
        
        [Command("chatGpt")]
        public async Task ChatGptCommand(CommandContext ctx, params string[] Question)
        {
            var chatgpt = new ChatGpt();
            string response = await chatgpt.ResponseGenerateChatGpt(string.Join(" ", Question));
            await ctx.Channel.SendMessageAsync(response);

        }
        [Command("imageGpt")]

        public async Task ImageGptCommand(CommandContext ctx, params string[] Question)
        {
            var chatgpt = new ChatGpt();
            string linkImage = await chatgpt.ImageGenerateChatGpt(string.Join(" ", Question));
            await ctx.Channel.SendMessageAsync(linkImage);

        }

        [Command("AudioGpt")]
        public async Task AudioGptCommand(CommandContext ctx)
        {     

            var chatGpt = new ChatGpt();
            string nombreArchivo =  ctx.Message.Attachments[0].FileName.ToString();


            // Verifica que al extension del archivo sea la correcta (.wav,.mp3 o .m4a)
            if (chatGpt.CheckedDocumentExtension(nombreArchivo))
            {
                string url = ctx.Message.Attachments[0].Url.ToString();

                string response = await chatGpt.AudioTextGenerate(url, nombreArchivo);

                await ctx.Channel.SendMessageAsync(response);

            } else
            {
                await ctx.Channel.SendMessageAsync("La extension valida para utilizar este comando es .wav, .mp3 o .m4a");

            }


            
        }


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
