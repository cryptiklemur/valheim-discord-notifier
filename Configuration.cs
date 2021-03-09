using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using LitJson;

namespace DiscordNotifier
{
    public class Configuration
    {
        public static readonly string MessageStructComment = "Below is the configuration for the available messages you can send when the corresponding events happen in game.";
        public static readonly Dictionary<string, string[]> AvailableVariables = new Dictionary<string, string[]>
        {
            {"OnPlayerDeath", new[] {"{{username}}", "{{userId}}"}},
            {"OnPlayerDisconnect", new[] {"{{username}}", "{{userId}}"}},
            {"OnPlayerJoined", new[] {"{{username}}", "{{userId}}"}},
            {"OnServerStart", new[] {"{{serverAddress}}"}},
            {"OnServerStop", new string[]{}},
        };

        public class MessageStruct
        {
            public string __comment = MessageStructComment;
            public Dictionary<string, string[]> __availableVariables = AvailableVariables;

            public string[] OnPlayerDeath = { "{{username}} has died!" };
            public string[] OnPlayerDisconnect = { "{{username}} has disconnected!" };
            public string[] OnPlayerJoined = { "{{username}} has joined!" };

            public string[] OnServerStart = { "Server has started!" };
            public string[] OnServerStop = { "Server has stopped!" };
        }

        public readonly string messagesJsonPath;
        public readonly MessageStruct messages;

        public readonly ConfigEntry<string> WebhookUrl;
        public readonly ConfigEntry<bool> Enabled;
        public readonly ConfigEntry<bool> FetchAndShowIp;
        public readonly ConfigEntry<string> RawIgnoredUsernames;
        public readonly ConfigEntry<string> IgnoredChatMessageRegex;
        public readonly ConfigEntry<bool> UpperCaseShout;
        
        public readonly Dictionary<ValheimEvent, ConfigEntry<bool>> Events = new Dictionary<ValheimEvent, ConfigEntry<bool>>();
        private string[] _ignoredUsernames;

        public string[] IgnoredUsernames => _ignoredUsernames ??= RawIgnoredUsernames.Value.Split(',').Select((x) => x.Trim()).ToArray();

        internal Configuration(ConfigFile config)
        {
            messagesJsonPath = config.ConfigFilePath.Substring(0, config.ConfigFilePath.Length - 3) + "messages.json";
            messages = ReadMessagesConfig();

            Enabled = config.Bind("General", "Enabled", true, "Is the plugin enabled?");
            FetchAndShowIp = config.Bind("General", "FetchAndShowIp", false, "Should the plugin attempt to get the server IP and post to the webhook");
            WebhookUrl = config.Bind("General", "WebhookUrl", "", "Enter the Webhook URL from discord here.");
            RawIgnoredUsernames = config.Bind("Chat", "IgnoredUsernames", "[]", "Array of ignored usernames. Comma separated. Use the format: Coralle, Steve");
            IgnoredChatMessageRegex = config.Bind("Chat", "IgnoredChatMessageRegex", "", "Specify a regex used to ignore chat messages.\nSyntax: /(^START)|(#END$)/\nThis would ignore text from the MapSync mod");
            UpperCaseShout = config.Bind("Chat", "UpperCaseShout", true, "Should /s messages be uppercased to match the in-game experience?");

            foreach (var eventName in Enum.GetNames(typeof(ValheimEvent)))
            {
                if (!Enum.TryParse(eventName, false, out ValheimEvent evt))
                {
                    Main.StaticLogger.LogError("Bad event name, somehow: " + eventName);
                    continue;
                }

                Events[evt] = config.Bind("Events", eventName, evt != ValheimEvent.OnPlayerWhisper);
            }
        }

        private MessageStruct ReadMessagesConfig()
        {
            try
            {
                EnsureMessageConfigExists();
                var parsedJson = JsonMapper.ToObject<MessageStruct>(GetMessageConfigText()) ?? new MessageStruct();
                var json = EnsureMessageConfigUpToDate(parsedJson);

                return json;
            }
            catch (Exception e)
            {
                throw new Exception("Issue parsing JSON. Validate your JSON: https://jsonlint.com/", e);
            }
        }

        private void EnsureMessageConfigExists()
        {
            if (File.Exists(messagesJsonPath)) return;
            
            using var sw = File.CreateText(messagesJsonPath);
            JsonMapper.ToJson(new MessageStruct(), new JsonWriter(sw) { PrettyPrint = true, IndentValue = 4 });
        }

        private string GetMessageConfigText() => File.ReadAllText(messagesJsonPath);

        private MessageStruct EnsureMessageConfigUpToDate(MessageStruct messageStruct)
        {
            messageStruct.__comment = MessageStructComment;
            messageStruct.__availableVariables = AvailableVariables;

            var sb = new StringBuilder();
            JsonMapper.ToJson(messageStruct, new JsonWriter(sb) { PrettyPrint = true, IndentValue = 4 });

            File.WriteAllText(messagesJsonPath, sb.ToString());

            return messageStruct;
        }
    }
}
