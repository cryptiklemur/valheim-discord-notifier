using System.Collections.Generic;
using System.IO;
using System.Net;
using LitJson;

namespace DiscordNotifier
{
    public class Utils
    {
        public static void PostMessage(string message, string username = null)
        {
            Main.StaticLogger.LogMessage($"Posting message to webhook: {message}");
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(Main.Configuration.WebhookUrl.Value);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var body = new Dictionary<string, string> {{"content", message}};
                if (username != null) body.Add("username", username);

                streamWriter.Write(JsonMapper.ToJson(body));
            }

            httpWebRequest.GetResponseAsync();
        }

        public static string FetchIPAddress()
        {
            string ipAddress;
            const string url = @"https://api.ipify.org/";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            ipAddress = reader.ReadToEnd();

            return ipAddress;
        }
    }
}