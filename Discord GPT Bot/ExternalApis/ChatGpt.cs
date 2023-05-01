using Newtonsoft.Json;
using OpenAI_API.Completions;
using OpenAI_API;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API.Images;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Discord_GPT_Bot.ExternalApis
{
    public class ChatGpt
    {
        public string text { get; set; }

        private async Task<string> GetApiKey()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            return configJson.ApiChatGPT.ToString();
        }

        public async Task<string> ResponseGenerateChatGpt(string Question)
        {
            try
            {
                string apiKey = await GetApiKey();
                string resultQuestion = string.Empty;
                var openIA = new OpenAIAPI(apiKey);
                CompletionRequest completionRequest = new CompletionRequest
                {
                    Prompt = Question,
                    MaxTokens = 4000
                };

                var result = await openIA.Completions.CreateCompletionsAsync(completionRequest);
                if (result != null)
                {
                    foreach (var item in result.Completions)
                    {
                        resultQuestion = item.Text;
                    }
                }

                return resultQuestion;

            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ImageGenerateChatGpt(string Question)
        {
            try
            {
                string apiKey = await GetApiKey();
                var openIA = new OpenAIAPI(apiKey);

                var result = await openIA.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(Question, 1, ImageSize._512));

                return result.Data[0].Url;
                
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> AudioTextGenerate(string urlFile,string nameFile)
        {
            try
            {
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), nameFile);
                if (String.IsNullOrEmpty(urlFile))
                {
                    throw new ArgumentNullException("La dirección URL del documento es nula o se encuentra en blanco.");
                }
                if (String.IsNullOrEmpty(pathToSave))
                {
                    throw new ArgumentNullException("La ruta para almacenar el documento es nula o se encuentra en blanco.");
                }
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.DownloadFile(urlFile, pathToSave);
                }


                string apiKey = await GetApiKey();
                string response;
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.openai.com/v1/audio/transcriptions"))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}");

                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(pathToSave)), "file", Path.GetFileName(pathToSave));
                        multipartContent.Add(new StringContent("whisper-1"), "model");
                        request.Content = multipartContent;

                        var respo = await httpClient.SendAsync(request);
                        response = await respo.Content.ReadAsStringAsync();
                    }
                }

                File.Delete(pathToSave);

                ChatGpt obj = JsonConvert.DeserializeObject<ChatGpt>(response);

                return obj.text;

            } catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public bool CheckedDocumentExtension(string nameFile)
        {
            string extension = Path.GetExtension(nameFile);
            if(extension == ".mp3" || extension == ".m4a" || extension == ".wav")
            {
                return true;
            }

            return false;
        }

    }
    
}


//var templateFile = Path.Combine(Directory.GetCurrentDirectory(), "Templates\\SendGridTemplate.html");