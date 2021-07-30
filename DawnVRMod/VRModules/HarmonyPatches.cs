using System;
using UnityEngine;
using MelonLoader;
using System.Reflection;
using System.Diagnostics;

namespace DawnVR
{
    internal static class HarmonyPatches
    {
        private static HarmonyLib.Harmony HarmonyInstance;

        public static void Init(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPre(typeof(_1EB728BCC.T_A7E3390E).GetMethod("LateUpdate"), "EyeHeadAnimator_LateUpdate");
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetAxisVector3"), "VRVector3Axis");
        }

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(prefixMethodName)));
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(postfixMethodName)));

        //todo: not working, animation handled by CharController?
        public static bool EyeHeadAnimator_LateUpdate() => false;

        public static bool VRVector3Axis(ref Vector3 __result, eGameInput _1A16DF67C, eGameInput _19E4D962D, eGameInput _19F48D18E)
        {
            Vector2 axis = VRRig.Instance.Input.LeftController.Thumbstick.Axis;
            __result = new Vector3(axis.x, 0, axis.y);
            return false;
        }

        #region NoVR Patches

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), "ReturnTrue");
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), "ReturnTrue");
            // manual patches for these :P
            // same method, different types (and the obfuscation only got one of em)
            HarmonyInstance.Patch(typeof(T_C3DD66D9).GetMethod("ChangeAnimSet"), new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod("ChangeAnimSetP", new Type[] { typeof(T_59B7B63D), typeof(_19480244B.T_E1C2C083), typeof(Transform) })));
            HarmonyInstance.Patch(typeof(T_C3DD66D9).GetMethod("_17E7DCA76", HarmonyLib.AccessTools.all), new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod("ChangeAnimSetP2", new Type[] { typeof(GameObject), typeof(_19480244B.T_E1C2C083), typeof(Transform) })));
        }

        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }

        // public void ChangeAnimSet(STCharAnimSet newAnimSet, PrefabData attachObj = null, Transform attachSocket = null)
        public static bool ChangeAnimSetP(T_59B7B63D _12B92D0EA, _19480244B.T_E1C2C083 _1DC5545CD = null, Transform _167BD9A61 = null)
        {
            MelonLogger.Msg("---- PUBLIC ANIM CHANGE ----");
            MelonLogger.Msg("new anim set: " + _12B92D0EA.FullPathName);
            MelonLogger.Msg("attachObj: " + _1DC5545CD?.FullPathName ?? "NULL");
            MelonLogger.Msg("attachSocket: " + _167BD9A61?.name ?? "NULL");
            return true;
        }

        // private void ChangeAnimSet(GameObject animSet, PrefabData attachObj = null, Transform attachSocket = null)
        public static bool ChangeAnimSetP2(GameObject _14408D88A, _19480244B.T_E1C2C083 _1DC5545CD = null, Transform _167BD9A61 = null)
        {
            MelonLogger.Msg("---- PRIVATE ANIM CHANGE ----");
            MelonLogger.Msg("anim set: " + _14408D88A.name);
            MelonLogger.Msg("attachObj: " + _1DC5545CD?.FullPathName ?? "NULL");
            MelonLogger.Msg("attachSocket: " + _167BD9A61?.name ?? "NULL");
            return true;
        }

        #endregion
    }
}
