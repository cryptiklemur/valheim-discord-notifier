using HarmonyLib;

namespace DiscordNotifier.Patches
{
    internal class FejdStartupPatch
    {
        [HarmonyPatch(typeof(FejdStartup), "SetupGui")]
        internal class SetupGui
        {
            static void Postfix(ref FejdStartup __instance)
            {
                __instance.m_versionLabel.text += $"\n<size=6><color=#ADB2FFFF>{Main.AUTHOR}'s {Main.MODNAME} v{Main.VERSION}</color></size>";
            }
        }
    }
}