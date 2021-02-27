using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace DiscordNotifier
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [HarmonyPatch]
    public class Main : BaseUnityPlugin
    {
        public const string MODNAME = "Discord Notifier";
        public const string AUTHOR = "CryptikLemur";
        public const string GUID = "CryptikLemur_DiscordNotifier";
        public const string VERSION = "0.0.1.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        public static ConfigEntry<string> configWebhookUrl;
        public static ConfigEntry<bool> configEnabled;
        public static ConfigEntry<bool> configFetchAndShowIp;
        public static ConfigEntry<bool> configTrackAllUsers;
        public static Dictionary<ValheimEvent, ConfigEntry<bool>> configEvents = new Dictionary<ValheimEvent, ConfigEntry<bool>>();

        internal static ManualLogSource StaticLogger;
        internal static ZNet zNet;
        internal static ZDOMan zdoMan;
        internal static Game game;

        public Main()
        {
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
            StaticLogger = Logger;

            configEnabled = Config.Bind("General", "Enabled", true, "Is the plugin enabled?");
            configFetchAndShowIp = Config.Bind("General", "FetchAndShowIp", false, "Should the plugin attempt to get the server IP and post to the webhook");
            configWebhookUrl = Config.Bind("General", "WebhookUrl", "", "Enter the Webhook URL from discord here.");
            configTrackAllUsers = Config.Bind("General", "TrackAllUsers", false, "Should the plugin track all the users on the server, or just you. If running as the server, this is ignored.");

            foreach (var eventName in Enum.GetNames(typeof(ValheimEvent)))
            {
                if (!Enum.TryParse(eventName, false, out ValheimEvent evt))
                {
                    Logger.LogError("Bad event name, somehow: " + eventName);
                    continue;
                }

                configEvents[evt] = Config.Bind("Events", eventName, true);
            }
        }

        private void Awake()
        {
            if (configEnabled.Value && configWebhookUrl.Value.Length > 0)
            {
                harmony.PatchAll(assembly);
                Logger.LogMessage($"{AUTHOR}'s {MODNAME} (v{VERSION}) has started");
            }
        }
        private void OnDestroy()
        {
            if (configEnabled.Value) harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(ZNet), "Awake")]
        private class ZNetPatch
        {
            static void Prefix(ref ZNet __instance)
            {
                zNet = __instance;
            }
        }

        [HarmonyPatch(typeof(ZDOMan), "ResetSectorArray")]
        private class ZDOManPatch
        {
            static void Postfix(ref ZDOMan __instance)
            {
                zdoMan = __instance;
            }
        }

        [HarmonyPatch(typeof(Game), "Awake")]
        private class GamePatch
        {
            static void Prefix(ref Game __instance)
            {
                game = __instance;
            }
        }
    }
}
