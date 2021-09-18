using DawnVR.Modules.VR;
using MelonLoader;

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

        public static void UnloadCurrentLevel() => VRRig.Instance.UpdateRigParent(eGameMode.kNone);
    }
}
