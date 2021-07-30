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
            PatchPre(typeof(T_C3DD66D9).GetMethod("UpdateBlendIdle"), "UpdateIdleAnim");
        }

        public static bool ReturnTrue(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool UpdateIdleAnim(T_C3DD66D9 __instance)
        {
            if (__instance.IsWalking)
            {
                __instance.m_currProgression = (__instance.m_targetProgression = 0f);
                if (__instance.m_targetProgression == 0f)
                {
                    float num = Mathf.Ceil(__instance.m_currProgression) * __instance.CurrentAnimSet.animSet.c_maxIdleFade * Time.deltaTime;
                    __instance.m_currProgression = Mathf.Clamp(__instance.m_currProgression - num, 0f, 3f);
                }
                else
                {
                    float num2 = Mathf.Ceil(__instance.m_currProgression) * __instance.CurrentAnimSet.animSet.c_maxFade * Time.deltaTime;
                    __instance.m_currProgression = Mathf.Clamp(__instance.m_currProgression - num2, 0f, 3f);
                }
                __instance.m_currentFade = __instance.m_currProgression - Mathf.Floor(__instance.m_currProgression);
                __instance.m_currFadeSpeed = __instance.CurrentAnimSet.animSet.c_maxIdleFade;
                __instance.m_charAnimState = T_7C97EEE2.eCharMoveAnim.kWalk;
                __instance.UpdateWalkAnims();
            }
            else
            {
                /*for (int i = 1; i < __instance.m_animStates.Length; i++)
                {
                    __instance.ResetAnimState(i, false);
                    typeof(T_C3DD66D9)
                }
                if (__instance.IsAnimValid(T_7C97EEE2.eCharMoveAnim.kIdle))
                {
                    __instance.m_animStates[0].weight = 1f;
                    if (__instance.CurrentAnimSet.HasProp())
                    {
                        __instance.m_propAnimStates[0].weight = 1f;
                    }
                }*/
            }
            return false;
        }

        #endregion
    }
}
