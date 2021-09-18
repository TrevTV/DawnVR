using UnityEngine;
using MelonLoader;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static bool wasLastDistZero;

        public static void SetCameraPosition2(Camera _13A97A3A2, Vector3 _1ACF98885)
        {
            if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
            {
                float f = Vector3.Distance(_13A97A3A2.transform.position, _1ACF98885);
                if (f != 0)
                {
                    if (wasLastDistZero)
                        MelonLogger.Msg("finished 0s");
                    MelonLogger.Msg("Distance between Camera positions: " + f.ToString());
                }
                else if (!wasLastDistZero)
                {
                    MelonLogger.Msg("Distance between Camera positions: 0");
                    wasLastDistZero = true;
                }
            }
        }

        public static void OnPPEnable2(UnityEngine._1F1547F66.T_190FC323 __instance)
        {
            if (__instance.GetComponent<VR.VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = __instance.gameObject.AddComponent<VR.VRPostProcessing>();
            vpp.profile = __instance.profile;
        }

        public static void OnSetMode2(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
            }
        }

        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
