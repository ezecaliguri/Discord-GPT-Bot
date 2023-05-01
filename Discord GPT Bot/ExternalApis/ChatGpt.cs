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
        // Esta propiedad fue la solucion que pense para poder trabajar el json que me devolvia la peticion directa del metodo AudioTextGenerate
        public string text { get; set; }


        // Obtencion de la api key del directorio: Discord GPT Bot\bin\Debug\config.json
        private async Task<string> GetApiKey()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            return configJson.ApiChatGPT.ToString();
        }


        // Este metodo devuelve la respuesta del chatGPT, la longitud varia dependiendo los tokens asignados
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

        // Metodo que otorga un link con la imagen generada a partir del parametro recibido
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

        // Metodo que devuelve el texto, a traves de un audio enviado desde discord

        public async Task<string> AudioTextGenerate(string urlFile,string nameFile)
        {
            try
            {
                // pathToSave obtiene el directorio en el cual se encuentra alojado el bot, utiliza el nombre, para guardarlo temporalmente con el mismo 
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), nameFile);
                
                // Comprobación de la existencia de la url
                if (String.IsNullOrEmpty(urlFile))
                {
                    throw new ArgumentNullException("La dirección URL del documento es nula o se encuentra en blanco.");
                }

                //comprobación de ruta existente

                if (String.IsNullOrEmpty(pathToSave))
                {
                    throw new ArgumentNullException("La ruta para almacenar el documento es nula o se encuentra en blanco.");
                }

                // Realiza la descarga del archivo a traves el url enviado, y la aloja en el directorio anteriormente asignado, el uso del using es para optimar memoria

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.DownloadFile(urlFile, pathToSave);
                }


                //Obtencion de la apiKey
                string apiKey = await GetApiKey();
                string response;

                // No encontre en la libreria OpenAI como poder trabajar con archivos, por lo cual use la documentacion de OpenAi y genere la peticion con los datos que tenia en curl

                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.openai.com/v1/audio/transcriptions"))
                    {
                        //envio de autorizacion con la apiKey
                        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}"); 

                        //cuerpo de la petición
                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(pathToSave)), "file", Path.GetFileName(pathToSave)); // busqueda del archivo de audio descargado anterirormente para el envio a los servidores de GPT
                        multipartContent.Add(new StringContent("whisper-1"), "model");
                        request.Content = multipartContent;

                        var respo = await httpClient.SendAsync(request); //resultado de la peticion http
                        response = await respo.Content.ReadAsStringAsync(); //Contenido de la peticion, en un string tipo json
                    }
                }

                // Eliminacion del archivo descargado anteriormente para evitar uso de disco del servidor alojado
                File.Delete(pathToSave);

                //obtencion de la informacion del Json, para que pueda funcionar, la propiedad declarada al principio tiene que tener el mismo nombre, que figura en el string
                ChatGpt obj = JsonConvert.DeserializeObject<ChatGpt>(response);


                //Devuelve el resultado del audio enviado, en texto
                return obj.text;

            } catch(Exception ex)
            {
                //devuelve el mensaje de error que pueda surgir
                return ex.Message;
            }
        }

        // metodo para comprobar que la extencion del archivo es la permitida
        public bool CheckedDocumentExtension(string nameFile)
        {
            //obtiene la extension del nombre del archivo y las compara
            string extension = Path.GetExtension(nameFile);
            if(extension == ".mp3" || extension == ".m4a" || extension == ".wav")
            {
                return true;
            }

            return false;
        }

    }
    
}