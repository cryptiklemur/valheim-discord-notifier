namespace DiscordNotifier
{
    public enum ValheimEvent
    {
        OnServerStarted,
        OnServerStopped,
        OnPlayerJoined,
        OnPlayerDisconnected,
        OnPlayerDeath,
        OnPlayerRespawn,
    }

    public class ValheimEventHandler
    {
        private static bool IsTrackingAll() => Main.zNet.IsServer() || Main.configTrackAllUsers.Value;
        private static bool IsTrackingUserId(ZDOID zdoId) => !Main.zNet.IsServer() && !Main.configTrackAllUsers.Value && Player.m_localPlayer.GetZDOID().userID == zdoId.userID;

        public static void OnServerStarted()
        {
            if (!Main.configEvents[ValheimEvent.OnServerStarted].Value) return;

            Utils.PostMessage("Server has started");
        }


        public static void OnServerStopped()
        {
            if (!Main.configEvents[ValheimEvent.OnServerStopped].Value) return;

            Utils.PostMessage("Server has stopped");
        }


        public static void OnPlayerJoined(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerJoined].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Utils.PostMessage($"{playerInfo.m_name} has joined the server!");
        }

        public static void OnPlayerDisconnected(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerDisconnected].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Utils.PostMessage($"{playerInfo.m_name} has left the server!");
        }

        public static void OnPlayerDeath(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerDeath].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Utils.PostMessage($"{playerInfo.m_name} has died!");
        }
        public static void OnPlayerRespawn(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerRespawn].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Utils.PostMessage($"{playerInfo.m_name} has respawned!");
        }
    }
}