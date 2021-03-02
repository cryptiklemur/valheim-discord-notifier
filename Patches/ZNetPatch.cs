using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace DiscordNotifier.Patches
{
    internal class ZNetPatch
    {
        private static readonly List<long> ConnectedPlayers = new List<long>();

        public static ZNet.PlayerInfo GetPlayerInfoFromPeer(ZNetPeer peer)
        {
            ZNet.PlayerInfo player = new ZNet.PlayerInfo();
            player.m_characterID = peer.m_characterID;
            player.m_name = peer.m_playerName;
            player.m_host = peer.m_socket.GetHostName();
            player.m_publicPosition = peer.m_publicRefPos;
            if (player.m_publicPosition)
                player.m_position = peer.m_refPos;

            return player;
        }

        [HarmonyPatch(typeof(ZNet), "Awake")]
        internal class Awake
        {
            private static void Postfix(ref ZNet __instance)
            {

                ChatPatch.IsChatMessageIgnored("", "Testing message");
                if (!__instance.IsServer()) Main.StaticLogger.LogError("This mod does not work on the client!");
            }
        }

        [HarmonyPatch(typeof(ZNet), "RPC_CharacterID")]
        internal class RPC_CharacterID
        {
            private static void Postfix(ref ZNet __instance, ref ZRpc rpc)
            {
                ZNetPeer peer = __instance.GetPeer(rpc);
                if (peer.m_characterID.IsNone() || !ZNet.instance.IsServer() || !__instance.IsConnected(peer.m_uid) || ConnectedPlayers.Contains(peer.m_characterID.userID)) return;
                ConnectedPlayers.Add(peer.m_characterID.userID);

                ValheimEventHandler.OnPlayerJoined(GetPlayerInfoFromPeer(peer));
            }
        }

        [HarmonyPatch(typeof(ZNet), "RPC_Disconnect")]
        internal class RPC_Disconnect
        {
            private static void Prefix(ref ZNet __instance, ref ZRpc rpc)
            {
                ZNetPeer peer = __instance.GetPeer(rpc);
                if (peer.m_characterID.IsNone() || !ZNet.instance.IsServer() || !__instance.IsConnected(peer.m_uid)) return;
                ConnectedPlayers.Remove(peer.m_uid);

                ValheimEventHandler.OnPlayerDisconnected(GetPlayerInfoFromPeer(peer));
            }
        }

        [HarmonyPatch(typeof(ZNet), "LoadWorld")]
        internal class LoadWorld
        {
            private static void Postfix(ref ZNet __instance)
            {
                if (!__instance.IsServer()) return;
                
                ValheimEventHandler.OnServerStarted(Main.Configuration.FetchAndShowIp.Value ? Utils.FetchIPAddress() : null);
            }
        }

        [HarmonyPatch(typeof(ZNet), "Shutdown")]
        internal class Shutdown
        {
            private static void Prefix(ref ZNet __instance)
            {
                if (!__instance.IsServer()) return;

                ValheimEventHandler.OnServerStopped();
            }
        }

        [HarmonyPatch(typeof(ZNet), "SendPeriodicData")]
        internal class SendPeriodicData
        {
            private static readonly List<long> DeadPlayers = new List<long>();

            private static ZNet.PlayerInfo GetPlayerInfo(ZNet __instance, ZDOID zdoId)
            {
                return __instance.GetPlayerList().Find(player => player.m_characterID.userID == zdoId.userID);
            }

            private static void Process(ZNet __instance, ZDOID zdoID)
            {
                if (zdoID.IsNone()) return;

                var zdo = ZDOMan.instance.GetZDO(zdoID);
                if (zdo == null) return;

                var dead = zdo.GetBool("dead", false);

                // If dead, and not in deadPlayers, add to deadPlayers and create event
                // If dead, and in deadPlayers, do nothing
                // If not dead, and in deadPlayers, remove
                // If not dead, and not in deadPlayers, do nothing
                if (dead)
                {
                    if (DeadPlayers.Contains(zdoID.userID)) return;
                    DeadPlayers.Add(zdoID.userID);
                    ValheimEventHandler.OnPlayerDeath(GetPlayerInfo(__instance, zdoID));
                }
                else if (DeadPlayers.Contains(zdoID.userID)) DeadPlayers.Remove(zdoID.userID);
            }

            private static void Prefix(ref ZNet __instance)
            {
                if (!__instance.IsServer()) return;

                foreach (var cur in __instance.m_peers.Where(cur => cur.IsReady())) Process(__instance, cur.m_characterID);
            }
        }
    }
}