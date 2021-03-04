using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiscordNotifier.Patches;
using HarmonyLib;
using LitJson;
using UnityEngine;

namespace DiscordNotifier
{
    public class Configuration
    {
        public class MessageStruct
        {
            public readonly string __comment = "Below is the configuration for the available messages you can send when the corresponding events happen in game.";
            public readonly string[] __availableVariables = {"{{username}}", "{{userId}}"};

            public string[] OnPlayerDeath = { "{{username}} has died!" };

            public string[] OnPlayerDisconnect = { "{{username}} has disconnected!" };

            public string[] OnPlayerJoined = { "{{username}} has joined!" };
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

        public string[] IgnoredUsernames => _ignoredUsernames ??= JsonMapper.ToObject<string[]>(RawIgnoredUsernames.Value);

        internal Configuration(ConfigFile config)
        {
            messagesJsonPath = config.ConfigFilePath.Substring(0, config.ConfigFilePath.Length - 3) + "messages.json";
            messages = ReadMessagesConfig();
            Main.StaticLogger.LogError(messages);

            Enabled = config.Bind("General", "Enabled", true, "Is the plugin enabled?");
            FetchAndShowIp = config.Bind("General", "FetchAndShowIp", false, "Should the plugin attempt to get the server IP and post to the webhook");
            WebhookUrl = config.Bind("General", "WebhookUrl", "", "Enter the Webhook URL from discord here.");
            RawIgnoredUsernames = config.Bind("Chat", "IgnoredUsernames", "[]", "Array of ignored usernames. Use the format: [\"Coralle\", \"Steve\"]");
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
            if (!File.Exists(messagesJsonPath))
            {
                using (var sw = File.CreateText(messagesJsonPath))
                {
                    JsonMapper.ToJson(new MessageStruct(), new JsonWriter(sw) { PrettyPrint = true, IndentValue = 4 });
                }
            }

            var content = File.ReadAllText(messagesJsonPath);
            Main.StaticLogger.LogError("Content: " + content);

            try
            {
                return JsonMapper.ToObject<MessageStruct>(content);
            }
            catch (Exception e)
            {
                throw new Exception("Issue parsing JSON. Validate your JSON: https://jsonlint.com/", e);
            }
        }
    }
}
