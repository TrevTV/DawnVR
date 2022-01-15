using DawnVR.Modules.VR;
using MelonLoader;
using System.Linq;
using UnityEngine;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void OnSetMode(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
                if (_1C57B7248 == eGameMode.kFreeRoam && (VRRig.Instance.ChloeComponent == null || VRRig.Instance.ChloeComponent.enabled == false))
                    VRRig.Instance.UpdateCachedChloe(T_A6E913D1.Instance.m_mainCharacter);
                VRRig.Instance?.UpdateRigParent(_1C57B7248);
            }
        }

        public static void OnFreeroamObjectActivate()
        {
            if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam)
            {
                if (T_A6E913D1.Instance.m_followCamera.m_isFreelook && T_A6E913D1.Instance.m_dawnUI.currentViewCookie == T_A7F99C25.eCookieChoices.kNone)
                {
                    Vector3 pos = VRRig.Instance.ChloeComponent.transform.Find("Reference/Hips").position;
                    pos.y = -0.3f;
                    VRRig.Instance.transform.position = pos;
                }
            }
        }

        public static void UnloadCurrentLevel() => VRRig.Instance.SetParent(null);
    }
}
