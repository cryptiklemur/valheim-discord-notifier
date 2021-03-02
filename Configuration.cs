using System;
using System.Collections.Generic;
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
        public readonly ConfigEntry<string> WebhookUrl;
        public readonly ConfigEntry<bool> Enabled;
        public readonly ConfigEntry<bool> FetchAndShowIp;
        public readonly ConfigEntry<string> RawIgnoredUsernames;
        public readonly ConfigEntry<string> IgnoredChatMessageRegex;
        
        public readonly Dictionary<ValheimEvent, ConfigEntry<bool>> Events = new Dictionary<ValheimEvent, ConfigEntry<bool>>();
        public readonly Dictionary<ValheimEvent, ConfigEntry<string>> EventMessages = new Dictionary<ValheimEvent, ConfigEntry<string>>();
        private string[] _ignoredUsernames;

        public string[] IgnoredUsernames => _ignoredUsernames ??= JsonMapper.ToObject<string[]>(RawIgnoredUsernames.Value);

        internal Configuration(ConfigFile config)
        {
            Enabled = config.Bind("General", "Enabled", true, "Is the plugin enabled?");
            FetchAndShowIp = config.Bind("General", "FetchAndShowIp", false, "Should the plugin attempt to get the server IP and post to the webhook");
            WebhookUrl = config.Bind("General", "WebhookUrl", "", "Enter the Webhook URL from discord here.");
            RawIgnoredUsernames = config.Bind("Chat", "IgnoredUsernames", "[]", "Array of ignored usernames. Use the format: [\"Coralle\", \"Steve\"]");
            IgnoredChatMessageRegex = config.Bind("Chat", "IgnoredChatMessageRegex", "", "Specify a regex used to ignore chat messages.\nSyntax: /(^START)|(#END$)/\nThis would ignore text from the MapSync mod");

            foreach (var eventName in Enum.GetNames(typeof(ValheimEvent)))
            {
                if (!Enum.TryParse(eventName, false, out ValheimEvent evt))
                {
                    Main.StaticLogger.LogError("Bad event name, somehow: " + eventName);
                    continue;
                }

                Events[evt] = config.Bind("Events", eventName, true);
            }

            EventMessages[ValheimEvent.OnPlayerDeath] = config.Bind("Events", "OnPlayerDeathMessage", "{{username}} has died!",
                "Message to send when a player dies.\nAvailable variables: {{username}}, {{userId}}");
            EventMessages[ValheimEvent.OnPlayerDisconnected] = config.Bind("Events", "OnPlayerDisconnectedMessage", "{{username}} has disconnected!",
                "Message to send when a player disconnects from the server.\nAvailable variables: {{username}}, {{userId}}");
            EventMessages[ValheimEvent.OnPlayerJoined] = config.Bind("Events", "OnPlayerJoinedMessage", "{{username}} has joined!",
                "Message to send when a player joins the server.\nAvailable variables: {{username}}, {{userId}}");
        }
    }
}
