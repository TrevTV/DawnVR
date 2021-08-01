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
            // Input Handling
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetAxisVector3"), "VRVector3Axis");
            // Disable Idling
            PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), "GetIdleExtraName");
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), "PostCharControllerStart");
        }

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(prefixMethodName)));
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(postfixMethodName)));

        public static bool GetIdleExtraName(ref string __result)
        {
            // todo: this causes an error to be spat out every frame, might want to replace at some point
            // CharAnimSet.GetIdleExtraName
            __result = "";
            return false;
        }

        public static void PostCharControllerStart(T_C3DD66D9 __instance)
        {
            foreach (AnimationState state in __instance.m_animStates)
            {
                if (state.name.ToLower().Contains("idle"))
                {
                    state.weight = 0;
                    state.speed = 0;
                    state.time = 0;
                }
            }
        }

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
        }

        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool DontRunMe()
        {
            return false;
        }

        #endregion
    }
}
