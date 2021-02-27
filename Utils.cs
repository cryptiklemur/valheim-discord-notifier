using System.CodeDom;
using System.IO;
using System.Net;

namespace DiscordNotifier
{
    public class Utils
    {
        public static void PostMessage(string message, string username = null)
        {
            Main.StaticLogger.LogMessage($"Posting message to webhook: {message}");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Main.configWebhookUrl.Value);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = $"{{\"content\":\"{message}\"";
                if (username != null)
                {
                    json += $", \"username\": \"{username}\"";
                }

                json += "}";

                streamWriter.Write(json);
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