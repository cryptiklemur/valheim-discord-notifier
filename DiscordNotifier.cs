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
        public const string VERSION = "0.0.4.0";

        internal Harmony harmony;
        internal Assembly assembly;

        internal static ManualLogSource StaticLogger;

        public static Configuration Configuration;

        private void Awake()
        {
            StaticLogger = Logger;
            Configuration = new Configuration(Config);

            if (!Configuration.Enabled.Value || Configuration.WebhookUrl.Value.Length <= 0) return;

            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();

            harmony.PatchAll(assembly);
            Logger.LogMessage($"{AUTHOR}'s {MODNAME} (v{VERSION}) has started");
        }
        private void OnDestroy()
        {
            if (Configuration.Enabled.Value) harmony.UnpatchSelf();
        }
    }
}
