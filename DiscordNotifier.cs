using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace DiscordNotifier
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string MODNAME = "Discord Notifier";
        public const string AUTHOR = "CryptikLemur";
        public const string GUID = "CryptikLemur_DiscordNotifier";
        public const string VERSION = "0.0.5.0";

        internal static ManualLogSource StaticLogger;

        public static Configuration Configuration;

        private void Awake()
        {
            var harmony = new Harmony(GUID);
            var assembly = Assembly.GetExecutingAssembly();

            StaticLogger = Logger;
            Configuration = new Configuration(Config);

            if (!Configuration.Enabled.Value || Configuration.WebhookUrl.Value.Length <= 0) return;

            harmony.PatchAll(assembly);
            Logger.LogMessage($"{AUTHOR}'s {MODNAME} (v{VERSION}) has started");
        }
    }
}
