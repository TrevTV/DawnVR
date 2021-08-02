using System;
using UnityEngine;
using MelonLoader;
using System.Reflection;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static class HarmonyPatches
    {
        private static HarmonyLib.Harmony HarmonyInstance;

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(prefixMethodName)));
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(postfixMethodName)));

        public static void Init(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            // Debug Stuff
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), "DisableSplashScreen");
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), "CutsceneSkipPressed");
            // Input Handling
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetAxisVector3"), "VRVector3Axis");
            // Disable Idling
            PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), "GetIdleExtraName");
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), "PostCharControllerStart");
            // Rig Parent Updating
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), "OnSetMode");
            // Highlight Manager
            PatchPost(typeof(T_4679B25C).GetMethod("SelectObject"), "Highlights_SelectObject");
            PatchPost(typeof(T_4679B25C).GetMethod("DeselectObject"), "Highlights_DeselectObject");
            PatchPost(typeof(T_4679B25C).GetMethod("DeselectAll"), "Highlights_DeselectAll");
        }

        public static void Highlights_SelectObject(T_A8FCC1F5 _13F8ADFD4) => VRHighlightManager.Instance?.SelectObject(_13F8ADFD4);
        public static void Highlights_DeselectObject(T_A8FCC1F5 _13F8ADFD4) => VRHighlightManager.Instance?.DeselectObject(_13F8ADFD4);
        public static void Highlights_DeselectAll() => VRHighlightManager.Instance?.DeselectAll();

        public static bool GetIdleExtraName(ref string __result)
        {
            __result = "empty_anim";
            return false;
        }

        public static bool CutsceneSkipPressed(T_BF5A5EEC __instance)
        {
            _15C6DD6D9.T_58A5E6E2 currentMode = __instance.GetCurrentMode<_15C6DD6D9.T_58A5E6E2>();
            if (currentMode != null)
            {
                T_156BDACC timeline = T_14474339.GetTimeline(currentMode);
                if (timeline != null)
                {
                    float sequenceEndTime = currentMode.sequenceEndTime;
                    float timeS = sequenceEndTime - timeline.CurrentTime;
                    T_E8819104.Singleton.AdvanceAllSounds(timeS);
                    timeline.SetTime(sequenceEndTime);
                    _169E4A3E.T_4B84CB26.s_forceFullEvaluate = true;
                    T_14474339.UpdateCurrentTimelinesForFrame();
                }
            }

            if (T_A6E913D1.Instance.m_rumbleManager != null)
                T_A6E913D1.Instance.m_rumbleManager.ClearAllRumbles(0f);

            return false;
        }

        public static void DisableSplashScreen(T_EDB11480 __instance) => __instance.m_splashList.Clear();

        public static void PostCharControllerStart(T_C3DD66D9 __instance)
        {
            AnimationClip clip = new AnimationClip();
            clip.name = "empty_anim";
            clip.legacy = true;
            __instance.m_animation.AddClip(clip, "empty_anim");
            VRRig.Instance?.UpdateCachedChloe(__instance);

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

        public static void OnSetMode(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
                VRRig.Instance?.UpdateRigParent(_1C57B7248);
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
            // Debug Stuff
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), "ReturnTrue");
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), "ReturnTrue");
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), "DisableSplashScreen");
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), "CutsceneSkipPressed");
            // Disable Idling
            PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), "GetIdleExtraName");
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), "PostCharControllerStart");
            // Testing
            PatchPost(typeof(T_A6E913D1).GetMethod("Awake"), "GameManagerAwake");
        }

        public static void GameManagerAwake(T_A6E913D1 __instance)
        {
            //__instance.SetBuildOptions(T_A6E913D1.eBuildOptions.kVR, true);
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
