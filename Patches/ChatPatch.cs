using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace DiscordNotifier.Patches
{
    internal class ChatPatch
    {
        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        internal class OnNewChatMessage
        {
            private static bool Prefix(ref long senderID, ref string user, ref string text, ref Talker.Type type, ref Vector3 pos)
            {
                Main.StaticLogger.LogInfo($"Message from {user} ({senderID}): {text}");

                // Ignoring text from MapSyncMod
                if (text.StartsWith("START_") && text.EndsWith("#END")) return true;

                // Ignoring "I HAVE ARRIVED!" yell, if we are already alerting when someone joins
                if (Main.configEvents[ValheimEvent.OnPlayerJoined].Value && text.ToLower() == "i have arrived!") return true;

                if (Main.zNet.IsServer() || Main.configTrackAllUsers.Value)
                {
                    ValheimEventHandler.OnPlayerMessage(type, user, text, pos);
                }

                return true;
            }
        }
    }
}