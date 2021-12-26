using UnityEngine;
using MelonLoader;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void OnSetMode2(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
        }

        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
