using System;
using System.Linq;
using UnityEngine;
using MelonLoader;
using System.Reflection;
using DawnVR.Modules.VR;
using UnityEngine.Rendering;
using UnityStandardAssets._1CC59503E;

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
            PatchPre(typeof(T_6FCAE66C).GetMethod("Init", HarmonyLib.AccessTools.all), "InputManagerInit");
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetAxisVector3"), "VRVector3Axis");
            // Disable Idling
            PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), "GetIdleExtraName");
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), "PostCharControllerStart");
            // Rig Parent Updating
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), "OnSetMode");
            // Highlight Manager
            //PatchPost(typeof(T_4679B25C).GetMethod("SelectObject"), "Highlights_SelectObject");
            //PatchPost(typeof(T_4679B25C).GetMethod("DeselectObject"), "Highlights_DeselectObject");
            //PatchPost(typeof(T_4679B25C).GetMethod("DeselectAll"), "Highlights_DeselectAll");
            PatchPre(typeof(T_4679B25C).GetMethod("Awake", HarmonyLib.AccessTools.all), "HighlightManagerAwake");
            PatchPre(typeof(T_4679B25C).GetMethod("_17B00A89A", HarmonyLib.AccessTools.all), "CreateStencilBuffer");
            PatchPre(typeof(T_4679B25C).GetMethod("OnDisable"), "DestroyStencilBuffer");
        }

        public static bool HighlightManagerAwake(T_4679B25C __instance)
        {
            Camera cam = new GameObject("testCam").AddComponent<Camera>();
            MelonLogger.Msg("highlight man awake");
            if (__instance.name.Contains("LISCamera") && !VRRig.Instance.Camera.GetComponent<T_FA85E78>())
            {
                MelonLogger.Msg("Beginning component copy...");

                // StenciledEdgeDetection
                try
                {
                    T_FA85E78 ogEdgeDetection = __instance.gameObject.GetComponent<T_FA85E78>();
                    T_FA85E78 edgeDet = VRRig.Instance.Camera.gameObject.AddComponent<T_FA85E78>();
                    foreach (FieldInfo i in typeof(T_FA85E78).GetFields(BindingFlags.Public | BindingFlags.Instance))
                        i.SetValue(edgeDet, i.GetValue(ogEdgeDetection));
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }

                try
                {
                    T_4679B25C highlightMan = VRRig.Instance.Camera.gameObject.AddComponent<T_4679B25C>();
                    foreach (FieldInfo i in typeof(T_4679B25C).GetFields(BindingFlags.Public | BindingFlags.Instance))
                        i.SetValue(highlightMan, i.GetValue(__instance));
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }

                MelonLogger.Msg("Succesfully copied edgeDetection!");

                return false;
            }

            return true;
        }

        public static bool CreateStencilBuffer(T_4679B25C __instance)
        {
            MelonLogger.Msg("Creating a stencil!");
            FieldInfo commandBuffer = typeof(T_4679B25C).GetField("_1743C93B6", HarmonyLib.AccessTools.all);
            if (commandBuffer.GetValue(__instance) != null)
            {
                ((CommandBuffer)commandBuffer.GetValue(__instance)).Clear();
            }
            else
            {
                commandBuffer.SetValue(__instance, new CommandBuffer());
                ((CommandBuffer)commandBuffer.GetValue(__instance)).name = "OutlineTransparent";
                VRRig.Instance.Camera.Component.AddCommandBuffer(CameraEvent.AfterGBuffer, (CommandBuffer)commandBuffer.GetValue(__instance));
            }

            return false;
        }

        public static bool DestroyStencilBuffer(T_4679B25C __instance)
        {
            MelonLogger.Msg("Destroying the stencil!");
            FieldInfo commandBuffer = typeof(T_4679B25C).GetField("_1743C93B6", HarmonyLib.AccessTools.all);
            if (commandBuffer.GetValue(__instance) != null && VRRig.Instance.Camera != null)
            {
                VRRig.Instance.Camera.Component.RemoveCommandBuffer(CameraEvent.AfterGBuffer, (CommandBuffer)commandBuffer.GetValue(__instance));
                commandBuffer.SetValue(__instance, null);
            }

            return false;
        }

        public static void InputManagerInit(T_6FCAE66C __instance)
        {
            // todo: doesnt work
            //typeof(T_6FCAE66C).GetField("m_overrideType").SetValue(__instance, eControlType.kXboxOne);
        }

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

            SkinnedMeshRenderer meshRenderer = __instance.transform.Find("CH_M_Chloe_Deluxe04/CH_M_Chloe_Deluxe04_Mesh").GetComponent<SkinnedMeshRenderer>();
            Material material = meshRenderer.sharedMaterials.Single((m) => m.name.Contains("Arms_TShirt"));
            material.hideFlags = HideFlags.DontUnloadUnusedAsset;
            VRRig.Instance.transform.Find("Controller (left)/ActuallyLeftHand").GetComponent<MeshRenderer>().sharedMaterial = material;
            VRRig.Instance.transform.Find("Controller (right)/ActuallyRightHand").GetComponent<MeshRenderer>().sharedMaterial = material;

            meshRenderer.enabled = false;

            #endregion
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
            // 
            //PatchPre(typeof(T_4679B25C).GetMethod("Awake", HarmonyLib.AccessTools.all), "HighlightManagerAwake2");
            //PatchPre(typeof(T_4679B25C).GetMethod("_17B00A89A", HarmonyLib.AccessTools.all), "CreateStencilBuffer2");
            //PatchPre(typeof(T_4679B25C).GetMethod("OnDisable"), "DestroyStencilBuffer2");
        }

        private static Camera cam;

        public static bool HighlightManagerAwake2(T_4679B25C __instance)
        {
            cam = new GameObject("testCam").AddComponent<Camera>();
            MelonLogger.Msg("highlight man awake");
            if (__instance.name.Contains("LISCamera") && !cam.GetComponent<T_FA85E78>())
            {
                MelonLogger.Msg("Beginning component copy...");

                // StenciledEdgeDetection
                try
                {
                    T_FA85E78 ogEdgeDetection = __instance.gameObject.GetComponent<T_FA85E78>();
                    T_FA85E78 edgeDet = cam.gameObject.AddComponent<T_FA85E78>();
                    foreach (FieldInfo i in typeof(T_FA85E78).GetFields(BindingFlags.Public | BindingFlags.Instance))
                        i.SetValue(edgeDet, i.GetValue(ogEdgeDetection));
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }

                try
                {
                    T_4679B25C highlightMan = cam.gameObject.AddComponent<T_4679B25C>();
                    foreach (FieldInfo i in typeof(T_4679B25C).GetFields(BindingFlags.Public | BindingFlags.Instance))
                        i.SetValue(highlightMan, i.GetValue(__instance));
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }

                MelonLogger.Msg("Succesfully copied edgeDetection!");

                return false;
            }

            return true;
        }

        public static bool CreateStencilBuffer2(T_4679B25C __instance)
        {
            MelonLogger.Msg("Creating a stencil!");
            FieldInfo commandBuffer = typeof(T_4679B25C).GetField("_1743C93B6", HarmonyLib.AccessTools.all);
            if (commandBuffer.GetValue(__instance) != null)
            {
                ((CommandBuffer)commandBuffer.GetValue(__instance)).Clear();
            }
            else
            {
                commandBuffer.SetValue(__instance, new CommandBuffer());
                ((CommandBuffer)commandBuffer.GetValue(__instance)).name = "OutlineTransparent";
                cam.AddCommandBuffer(CameraEvent.AfterGBuffer, (CommandBuffer)commandBuffer.GetValue(__instance));
            }

            return false;
        }

        public static bool DestroyStencilBuffer2(T_4679B25C __instance)
        {
            MelonLogger.Msg("Destroying the stencil!");
            FieldInfo commandBuffer = typeof(T_4679B25C).GetField("_1743C93B6", HarmonyLib.AccessTools.all);
            if (commandBuffer.GetValue(__instance) != null && cam != null)
            {
                cam.RemoveCommandBuffer(CameraEvent.AfterGBuffer, (CommandBuffer)commandBuffer.GetValue(__instance));
                commandBuffer.SetValue(__instance, null);
            }

            return false;
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
