using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace DiscordNotifier.Patches
{
    internal class ZNetPatch
    {
        internal static Dictionary<long, ZNet.PlayerInfo> players = new Dictionary<long, ZNet.PlayerInfo>();

        [HarmonyPatch(typeof(ZNet), "SendPlayerList")]
        internal class SendPlayerList
        {
            private static void Postfix(ref ZNet __instance)
            {
                var onServer = new List<long>();
                foreach (var player in __instance.GetPlayerList())
                {
                    // If the player is new, trigger an OnPlayerJoined event
                    if (!players.ContainsKey(player.m_characterID.userID)) ValheimEventHandler.OnPlayerJoined(player);
                    onServer.Add(player.m_characterID.userID);
                    players[player.m_characterID.userID] = player;
                }

                // If the player is no longer on the server, remove them from the list, and trigger an OnPlayerDisconnected event
                var toRemove = players.Values.Where(player => !onServer.Contains(player.m_characterID.userID)).ToList();
                toRemove.ForEach(player =>
                {
                    ValheimEventHandler.OnPlayerDisconnected(player);
                    players.Remove(player.m_characterID.userID);
                });
            }
        }

        [HarmonyPatch(typeof(ZNet), "LoadWorld")]
        internal class LoadWorld
        {
            private static void Postfix()
            {
                ValheimEventHandler.OnServerStarted();
            }
        }

        [HarmonyPatch(typeof(ZNet), "Shutdown")]
        internal class Shutdown
        {
            private static void Prefix(ref ZNet __instance)
            {
                if (__instance.IsServer())
                {
                    ValheimEventHandler.OnServerStopped();
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), "SendPeriodicData")]
        internal class SendPeriodicData
        {
            private static List<long> deadPlayers = new List<long>();

            private static ZNet.PlayerInfo getPlayerInfo(ZNet __instance, ZDOID zdoId)
            {
                return __instance.GetPlayerList().Find(player => player.m_characterID.userID == zdoId.userID);
            }

            private static void process(ZNet __instance, ZDOID zdoID)
            {
                if (zdoID.IsNone()) return;

                ZDO zdo = Main.zdoMan.GetZDO(zdoID);
                if (zdo == null) return;

                bool dead = zdo.GetBool("dead", false);

                // If dead, and not in deadPlayers, add to deadPlayers and create event
                // If dead, and in deadPlayers, do nothing
                // If not dead, and in deadPlayers, remove, and create event
                // If not dead, and not in deadPlayers, do nothing
                if (dead)
                {
                    if (deadPlayers.Contains(zdoID.userID)) return;
                    Main.StaticLogger.LogInfo($"{zdoID} died");
                    deadPlayers.Add(zdoID.userID);
                    ValheimEventHandler.OnPlayerDeath(getPlayerInfo(__instance, zdoID));
                }
                else
                {
                    if (!deadPlayers.Contains(zdoID.userID)) return;
                    Main.StaticLogger.LogInfo($"{zdoID} respawned");
                    deadPlayers.Remove(zdoID.userID);
                    ValheimEventHandler.OnPlayerRespawn(getPlayerInfo(__instance, zdoID));
                }
            }

            private static void Prefix(ref ZNet __instance)
            {
                if (!__instance.IsServer()) return;

                if (Player.m_localPlayer != null)
                {
                    process(__instance, Player.m_localPlayer.GetZDOID());
                }
                
                foreach (var cur in __instance.GetPeers())
                {
                    if (!cur.IsReady()) continue;
                    process(__instance, cur.m_characterID);
                }
            }
        }
    }
}