using HarmonyLib;

namespace DiscordNotifier.Patches
{
    internal class PlayerPatch
    {
        private static bool hasSpawned = false;

        private static ZNet.PlayerInfo getBasicPlayerInfoFromPlayer(Player player) => new ZNet.PlayerInfo
            {
                m_characterID = player.GetZDOID(),
                m_name = player.GetPlayerName(),
                m_position = player.transform.position
            };

        [HarmonyPatch(typeof(Player), "OnSpawned")]
        internal class OnSpawned
        {
            private static void Prefix(ref Player __instance)
            {
                if (hasSpawned || !ZNet.instance.IsServer() || ZNet.instance.IsDedicated()) return;
                hasSpawned = true;

                ValheimEventHandler.OnPlayerJoined(getBasicPlayerInfoFromPlayer(__instance));
            }
        }

        [HarmonyPatch(typeof(Player), "OnDeath")]
        internal class OnDeath
        {
            private static void Postfix(ref Player __instance)
            {
                if (!ZNet.instance.IsServer() || ZNet.instance.IsDedicated()) return;

                ValheimEventHandler.OnPlayerDeath(getBasicPlayerInfoFromPlayer(__instance));
            }
        }
    }
}