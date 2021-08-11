using System;
using System.Linq;
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
            PatchPre(typeof(T_C3DD66D9).GetMethod("CalculateAngle"), "CalculateCharAngle");
            //PatchPre(typeof(T_6FCAE66C).GetMethod("Init", HarmonyLib.AccessTools.all), "InputManagerInit");
            PatchPre(typeof(T_6FCAE66C).GetMethod("_1B350D183", HarmonyLib.AccessTools.all), "InputManagerInit");
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetAxisVector3"), "VRVector3Axis");

            // Disable Idling
            PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), "GetIdleExtraName");
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), "PostCharControllerStart");

            // Rig Parent Updating
            PatchPre(typeof(T_91FF9D92).GetMethod("UnloadCurrentLevel"), "UnloadCurrentLevel");
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), "OnSetMode");

            // Objective Manager
            PatchPost(typeof(T_81803C2C).GetMethod("SetReminder"), "SetReminderTexture");

            // Highlight Manager
            PatchPre(typeof(T_1C1609D7).GetMethod("Update"), "CUICameraRelativeUpdate");
            PatchPre(typeof(T_2D9F19A8).GetMethod("UpdatePosition"), "CUIAnchorUpdatePosition");
            PatchPre(typeof(T_8F74F848).GetMethod("CheckOnScreen"), "IsHotspotOnScreen"); // HotSpotUI
            PatchPre(typeof(T_572A4969).GetMethod("CheckOnScreen"), "IsInteractOnScreen"); // InteractUI
            // todo: i have no clue what "HoverObjectUI" is but it also has a CheckOnScreen function
        }

        private static readonly FieldInfo CharControl_WorldAngle = typeof(T_C3DD66D9).GetField("_15B7EF7A4", HarmonyLib.AccessTools.all);

        #region Debug Stuff

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

        #endregion

        #region Input Handling

        public static void InputManagerInit(T_6FCAE66C __instance)
        {
            // this was me testing a mono harmony bug, ignore
            /*MelonLogger.Msg("Force setting input manager to Xbox One (try catch)");
            try
            {
                typeof(T_6FCAE66C).GetField("_1C6FBAE09").SetValue(__instance, eControlType.kXboxOne);
            }
            catch (Exception e) { MelonLogger.Msg(e); }
            MelonLogger.Msg("ForceSet attempt 2 (notrycatch)");
            typeof(T_6FCAE66C).GetField("_1C6FBAE09").SetValue(__instance, eControlType.kXboxOne);
            MelonLogger.Msg("done");*/
            typeof(T_6FCAE66C).GetField("_1C6FBAE09", HarmonyLib.AccessTools.all).SetValue(__instance, eControlType.kXboxOne);
        }

        public static bool VRVector3Axis(ref Vector3 __result, eGameInput _1A16DF67C, eGameInput _19E4D962D, eGameInput _19F48D18E)
        {
            Vector2 axis = VRRig.Instance.Input.LeftController.Thumbstick.Axis;
            Vector3 controlDirection = new Vector3(axis.x, 0, axis.y);
            //__result = VRRig.Instance.Camera.transform.TransformDirection(controlDirection);
            __result = controlDirection;
            return false;
        }

        public static bool CalculateCharAngle(T_C3DD66D9 __instance, Vector3 _13F806F29)
        {
            // todo: somehow implement using the Camera to change direction like in other locomotion vr games
            /*if (_13F806F29 != __instance.m_moveDirection)
            {
                __instance.m_moveDirection = (__instance.m_nonNormalMoveDirection = _13F806F29);
                __instance.m_moveDirection.Normalize();
                CharControl_WorldAngle.SetValue(__instance, Vector3.Angle(Vector3.forward, __instance.m_moveDirection));
                if (_13F806F29.x < 0f)
                {
                    //__instance._15B7EF7A4 = 360f - __instance._15B7EF7A4;
                    CharControl_WorldAngle.SetValue(__instance, 360f - (float)CharControl_WorldAngle.GetValue(__instance));
                }
            }

            return false;*/

            return true;
        }

        #endregion

        #region Disable Idling

        public static bool GetIdleExtraName(ref string __result)
        {
            __result = "empty_anim";
            return false;
        }

        public static void PostCharControllerStart(T_C3DD66D9 __instance)
        {
            #region Disable Idling

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

            #endregion

            #region Add Hand Material

            Material material = null;
            foreach (SkinnedMeshRenderer sMesh in __instance.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Material possibleMat = sMesh.sharedMaterials?.SingleOrDefault((m) => m.name.Contains("Arms_TShirt"));
                if (possibleMat != null)
                    material = possibleMat;

                sMesh.sharedMesh = null;
            }
            material.hideFlags = HideFlags.DontUnloadUnusedAsset;
            VRRig.Instance.transform.Find("Controller (left)/ActuallyLeftHand").GetComponent<MeshRenderer>().sharedMaterial = material;
            VRRig.Instance.transform.Find("Controller (right)/ActuallyRightHand").GetComponent<MeshRenderer>().sharedMaterial = material;

            #endregion
        }

        #endregion

        #region Rig Parent Updating

        public static void OnSetMode(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
                VRRig.Instance?.UpdateRigParent(_1C57B7248);
            }
        }

        public static void UnloadCurrentLevel() => VRRig.Instance.UpdateRigParent(eGameMode.kNone);

        #endregion

        #region Objective Manager

        public static void SetReminderTexture(T_81803C2C __instance)
            => VRRig.Instance.transform.Find("Controller (left)/ActuallyLeftHand/handpad").GetComponent<MeshRenderer>().sharedMaterial = __instance.m_reminderRenderer.material;

        #endregion

        #region Highlight Manager

        private static readonly FieldInfo HotSpotUI_ScreenAlpha = typeof(T_8F74F848).GetField("_14888EF3", HarmonyLib.AccessTools.all);
        private static readonly FieldInfo InteractUI_HotSpotObj = typeof(T_572A4969).GetField("_133075675", HarmonyLib.AccessTools.all);

        public static bool CUICameraRelativeUpdate(T_1C1609D7 __instance)
        {
            __instance.transform.rotation = VRRig.Instance.Camera.transform.rotation;
            return false;
        }

        public static bool CUIAnchorUpdatePosition(T_2D9F19A8 __instance)
        {
            if (__instance.m_anchorObj != null)
            {
                Transform parent = __instance.transform.parent;
                __instance.transform.localPosition = ((!(parent != null)) ? __instance.m_anchorObj.transform.position : parent.InverseTransformPoint(__instance.m_anchorObj.transform.position)) + __instance.m_offset;
            }
            return false;
        }

        public static bool IsHotspotOnScreen(T_8F74F848 __instance, ref bool __result)
        {
            if (__instance.m_anchor == null || __instance.m_anchor.m_anchorObj == null)
            {
                __result = false;
                return false;
            }
            Vector3 position = VRRig.Instance.Camera.Component.WorldToScreenPoint(__instance.m_anchor.m_anchorObj.transform.position);
            Vector3 vector = VRRig.Instance.Camera.Component.ScreenToViewportPoint(position);
            if (vector.x > 0f && vector.y > 0f && vector.x < 1f && vector.y < 1f)
            {
                float num = 0.1f; // never changed, so its hardcoded here

                if (vector.x < num)
                    HotSpotUI_ScreenAlpha.SetValue(__instance, Mathf.Lerp(0f, 1f, vector.x / num));
                else if (vector.x > 1f - num)
                    HotSpotUI_ScreenAlpha.SetValue(__instance, Mathf.Lerp(0f, 1f, (1f - vector.x) / num));
                else
                    HotSpotUI_ScreenAlpha.SetValue(__instance, 1f);

                if (vector.y < num)
                    HotSpotUI_ScreenAlpha.SetValue(__instance, ((float)HotSpotUI_ScreenAlpha.GetValue(__instance)) * Mathf.Lerp(0f, 1f, vector.y / num));
                else if (vector.y > 1f - num)
                    HotSpotUI_ScreenAlpha.SetValue(__instance, ((float)HotSpotUI_ScreenAlpha.GetValue(__instance)) * Mathf.Lerp(0f, 1f, (1f - vector.y) / num));

                __result = true;
                return false;
            }
            float output = 0;
            HotSpotUI_ScreenAlpha.SetValue(__instance, Mathf.SmoothDamp((float)HotSpotUI_ScreenAlpha.GetValue(__instance), 0f, ref output, 0.2f));
            return false;
        }

        public static bool IsInteractOnScreen(T_572A4969 __instance, ref bool __result)
        {
            if (__instance.m_anchor != null && __instance.m_anchor.m_anchorObj != null)
            {
                T_6FD30C1C hotspotObj = (T_6FD30C1C)InteractUI_HotSpotObj.GetValue(__instance);
                float num = Vector3.Angle(VRRig.Instance.Camera.transform.forward, __instance.m_anchor.m_anchorObj.transform.position - VRRig.Instance.Camera.transform.position);
                if (num < 180f)
                {
                    Vector3 position = VRRig.Instance.Camera.Component.WorldToScreenPoint(__instance.m_anchor.m_anchorObj.transform.position);
                    Vector3 vector = VRRig.Instance.Camera.Component.ScreenToViewportPoint(position);
                    if (vector.x > 0f && vector.y > 0f && vector.x < 1f && vector.y < 1f)
                    {
                        __instance.m_arrow.gameObject.SetActive(true);
                        __instance.m_choiceUI.gameObject.SetActive(true);
                        if (hotspotObj != null)
                            hotspotObj.Select(true, true);
                        __result = true;
                        return false;
                    }
                }
                __instance.m_arrow.gameObject.SetActive(false);
                __instance.m_choiceUI.gameObject.SetActive(false);
                if (hotspotObj != null)
                    hotspotObj.Select(false, false);
            }
            __result = false;
            return false;
        }

        #endregion

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
            //
            PatchPost(typeof(T_8F74F848).GetMethod("CheckOnScreen"), "IsHotspotOnScreen2");
            PatchPre(typeof(T_5A79B056).GetMethod("_183B63F81", HarmonyLib.AccessTools.all), "TweenAlphaCache");
            //PatchPre(typeof(T_4679B25C).GetMethod("Awake", HarmonyLib.AccessTools.all), "HighlightManagerAwake2");
            //PatchPre(typeof(T_4679B25C).GetMethod("_17B00A89A", HarmonyLib.AccessTools.all), "CreateStencilBuffer2");
            //PatchPre(typeof(T_4679B25C).GetMethod("OnDisable"), "DestroyStencilBuffer2");
        }

        private static Camera cam;

        public static bool TweenAlphaCache(T_5A79B056 __instance)
        {
            __instance.enabled = false;
            return false;
        }

        public static void GameManagerAwake(T_A6E913D1 __instance)
        {
            __instance.SetBuildOptions(T_A6E913D1.eBuildOptions.kVR, true);
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
