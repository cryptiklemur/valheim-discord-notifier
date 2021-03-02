using System;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;

namespace DiscordNotifier.Patches
{
    internal class ChatPatch
    {
        public static bool IsChatMessageIgnored(string user, string text)
        {
            // Ignoring text from MapSyncMod
            if (text.StartsWith("START_") && text.EndsWith("#END")) return true;

            // Ignoring "I HAVE ARRIVED!" yell, if we are already alerting when someone joins
            if (Main.Configuration.Events[ValheimEvent.OnPlayerJoined].Value && text.ToLower() == "i have arrived!") return true;

            // Ignoring messages from Configuration.ignoredUsernames
            if (Main.Configuration.IgnoredUsernames.ToList().Contains(user)) return true;

            var regexPattern = Main.Configuration.IgnoredChatMessageRegex.Value;
            if (regexPattern.Length == 0 || !regexPattern.StartsWith("/")) return false;
            var pattern = regexPattern.Substring(1, regexPattern.LastIndexOf("/", StringComparison.Ordinal) - 1);
            var flagChars = regexPattern.Substring(regexPattern.LastIndexOf("/", StringComparison.Ordinal) + 1).ToLower().ToCharArray().ToList();

            RegexOptions opts = RegexOptions.None;
            if (flagChars.Contains('i')) opts |= RegexOptions.IgnoreCase;
            if (flagChars.Contains('m')) opts |= RegexOptions.Multiline;
            if (!flagChars.Contains('g')) opts |= RegexOptions.Singleline;

            return Regex.IsMatch(text, pattern, opts);
        }

        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        internal class OnNewChatMessage
        {
            private static bool Prefix(ref string user, ref string text, ref Talker.Type type, ref Vector3 pos)
            {
                if (!ZNet.instance.IsServer()|| IsChatMessageIgnored(user, text)) return true;

                ValheimEventHandler.OnPlayerMessage(type, user, text, pos);

                return true;
            }

        }
    }
}