using DawnVR.Modules.VR;
using MelonLoader;
using UnityEngine;
#if !REMASTER
using GameMaster = T_A6E913D1;
using OverlayCookie = T_A7F99C25;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void OnSetMode(bool __result, eGameMode __0)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + __0);
                if (__0 == eGameMode.kFreeRoam && (VRRig.Instance.ChloeComponent == null || VRRig.Instance.ChloeComponent.enabled == false))
                    VRRig.Instance.UpdateCachedChloe(GameMaster.Instance.m_mainCharacter);
                VRRig.Instance?.UpdateRigParent(__0);
            }
        }

        public static void OnFreeroamObjectActivate()
        {
            if (GameMaster.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam)
            {
                if (GameMaster.Instance.m_followCamera.m_isFreelook && GameMaster.Instance.m_dawnUI.currentViewCookie == OverlayCookie.eCookieChoices.kNone)
                {
                    Vector3 pos = Vector3.zero;
                    pos.y += VRRig.Instance.Calibrator.HeightOffset;
                    VRRig.Instance.transform.localPosition = pos;
                }
            }
        }

        public static void UnloadCurrentLevel() => VRRig.Instance.SetParent(null);
    }
}
