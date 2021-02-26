using System.IO;
using System.Net;

namespace DiscordNotifier
{
    public class Utils
    {
        public static void PostMessage(string message)
        {
            Main.StaticLogger.LogMessage($"Posting message to webhook: {message}");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Main.configWebhookUrl.Value);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = $"{{\"content\":\"{message}\"}}";

                streamWriter.Write(json);
            }

            httpWebRequest.GetResponseAsync();
        }
    }
}