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

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), "ReturnTrue");
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), "ReturnTrue");
        }

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(prefixMethodName)));
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(postfixMethodName)));

        //todo: not working, animation handled by CharController?
        public static bool EyeHeadAnimator_LateUpdate() => false;
        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool VRVector3Axis(ref Vector3 __result, eGameInput _1A16DF67C, eGameInput _19E4D962D, eGameInput _19F48D18E)
        {
            Vector2 axis = VRRig.Instance.Input.LeftController.Thumbstick.Axis;
            __result = new Vector3(axis.x, 0, axis.y);
            return false;
        }
    }
}
