using System;
using Valve.VR;
using System.Linq;
using UnityEngine;
using MelonLoader;
using System.Reflection;
using DawnVR.Modules.VR;
using UnityEngine._1F1547F66;
using System.Collections.Generic;

namespace DawnVR.Modules
{
    // todo: possibly separate patches by function (some moved to VRRig, VRCamera, etc)
    internal static class HarmonyPatches
    {
        private static HarmonyLib.Harmony HarmonyInstance;

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(prefixMethodName)));
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(typeof(HarmonyPatches).GetMethod(postfixMethodName)));

        public static void Init(HarmonyLib.Harmony hInstance)
        {
            // todo: find cause of input enabling delay, im sure its from one of these patches
            HarmonyInstance = hInstance;
            // Debug Stuff
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), nameof(DisableSplashScreen));
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed));

            // Input Handling
            PatchPre(typeof(T_6FCAE66C).GetMethod("_1B350D183", HarmonyLib.AccessTools.all), nameof(InputManagerInit));
            PatchPre(typeof(T_C3DD66D9).GetMethod("CalculateAngle"), nameof(CalculateCharAngle));
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetInputState", new Type[] { typeof(eGameInput), typeof(bool), typeof(bool), typeof(bool) }), nameof(GetInputState_Enum));
            PatchPre(typeof(T_D9E8342E).GetMethod("GetButtonState"), nameof(GetButtonState));
            PatchPre(typeof(T_D9E8342E).GetMethod("GetAxis"), nameof(GetAxis));

            // Disable Idling
            //PatchPre(typeof(T_7C97EEE2).GetMethod("GetIdleExtraName"), nameof(GetIdleExtraName));

            // Rig Parent Updating
            PatchPre(typeof(T_91FF9D92).GetMethod("UnloadCurrentLevel"), nameof(UnloadCurrentLevel));
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), nameof(OnSetMode));

            // Objective Manager
            PatchPost(typeof(T_81803C2C).GetMethod("SetReminder"), nameof(SetReminderTexture));
            PatchPre(typeof(T_1928221C).GetMethod("Update", HarmonyLib.AccessTools.all), nameof(DontRunMe));

            // Highlight Manager
            PatchPre(typeof(T_244D769F).GetMethod("Interact"), nameof(HotspotObjectInteract));
            PatchPre(typeof(T_1C1609D7).GetMethod("Update"), nameof(CUICameraRelativeUpdate));
            PatchPre(typeof(T_2D9F19A8).GetMethod("UpdatePosition"), nameof(CUIAnchorUpdatePosition));
            PatchPre(typeof(T_F8FE3E1C).GetMethod("Update"), nameof(DontRunMe));
            PatchPre(typeof(T_8F74F848).GetMethod("CheckOnScreen"), nameof(IsHotspotOnScreen)); // HotSpotUI
            PatchPre(typeof(T_572A4969).GetMethod("CheckOnScreen"), nameof(IsInteractOnScreen)); // InteractUI
            PatchPre(typeof(T_A0A6EA62).GetMethod("CheckOnScreen"), nameof(IsHoverObjectOnScreen)); // HoverObjectUI

            // Tutorial Fixes
            PatchPre(typeof(T_F6DEE320).GetMethod("Do"), nameof(UpdateTutorialUI));

            // Misc
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), nameof(PostCharControllerStart));
            PatchPre(typeof(T_96E81635).GetProperty("ScrollingText").GetGetMethod(), nameof(ReplaceScrollingText));
            PatchPost(typeof(T_421B9CDF).GetMethod("SetCameraPosition"), nameof(SetCameraPosition));
            PatchPre(typeof(T_3BE79CFB).GetMethod("Start", HarmonyLib.AccessTools.all), nameof(BoundaryStart));
            PatchPre(typeof(T_3BE79CFB).GetMethod("OnTriggerEnter", HarmonyLib.AccessTools.all), nameof(DontRunMe));
            // post processing doesnt seem to render correctly in vr, so this is gonna stay disabled
            //PatchPre(typeof(T_190FC323).GetMethod("OnEnable", HarmonyLib.AccessTools.all), nameof(OnPPEnable));
        }

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
            => __instance._1C6FBAE09 = eControlType.kXboxOne;

        public static bool CalculateCharAngle(T_C3DD66D9 __instance, Vector3 _13F806F29)
        {
            __instance._11C77E995 = Quaternion.Euler(0, VRRig.Instance.Camera.transform.eulerAngles.y, 0);
            if (_13F806F29 != __instance.m_moveDirection)
            {
                __instance.m_moveDirection = (__instance.m_nonNormalMoveDirection = _13F806F29);
                __instance.m_moveDirection.Normalize();
                __instance._15B7EF7A4 = Vector3.Angle(Vector3.forward, __instance.m_moveDirection);
                if (_13F806F29.x < 0f)
                    __instance._15B7EF7A4 = 360f - __instance._15B7EF7A4;
            }
            return false;
        }

        public static eInputState GetInputState_Binding(T_6FCAE66C inputManInstance, T_9005A419 binding)
        {
            if (inputManInstance.InputBlocked)
                return eInputState.kNone;

            for (int i = 0; i < binding.m_joystick.Count; i++)
            {
                eInputState buttonState = T_D9E8342E.Singleton.GetButtonState(binding.m_joystick[i]);
                if (buttonState != eInputState.kNone)
                    return buttonState;
            }

            return eInputState.kNone;
        }

        public static bool GetInputState_Enum(T_6FCAE66C __instance, ref eInputState __result, eGameInput _1561EDFFF)
        {
            if (__instance.InputBlocked)
            {
                __result = eInputState.kNone;
                return false;
            }

            if (_1561EDFFF == eGameInput.kAny)
            {
                if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.stateDown))
                {
                    __result = eInputState.kDown;
                    return false;
                }
                if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.state))
                {
                    __result = eInputState.kHeld;
                    return false;
                }
            }
            else if (__instance.m_keyBindings.ContainsKey((int)_1561EDFFF))
            {
                T_9005A419 keybinding = __instance.m_keyBindings[(int)_1561EDFFF];
                __result = GetInputState_Binding(__instance, keybinding);
                return false;
            }

            __result = eInputState.kNone;
            return false;
        }

        public static bool GetButtonState(ref eInputState __result, eJoystickKey _13A42C455)
        {
            __result = eInputState.kNone;

            switch (_13A42C455)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kStart:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kSelect:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                    break;
                /*case eJoystickKey.kDPadUp:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetThumbstickUp(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kDPadRight:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetThumbstickRight(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kDPadLeft:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetThumbstickLeft(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kDPadDown:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetThumbstickDown(VRInput.Hand.Left));
                    break;*/
                case eJoystickKey.kR1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kR2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kR3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kL1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kL2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kL3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kAction1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonX());
                    break;
                case eJoystickKey.kAction2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonY());
                    break;
                case eJoystickKey.kAction3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonB());
                    break;
                case eJoystickKey.kAction4:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonA());
                    break;
                // are these used here?
                case eJoystickKey.kPlatform:
                    break;
                case eJoystickKey.kLeftStickX:
                    break;
                case eJoystickKey.kLeftStickY:
                    break;
                case eJoystickKey.kRightStickX:
                    break;
                case eJoystickKey.kRightStickY:
                    break;
                default:
                    break;
            }

            return false;
        }

        public static bool GetAxis(ref float __result, eJoystickKey _1BBA85C4E)
        {
            __result = 0;

            switch (_1BBA85C4E)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kLeftStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Left).axis.x;
                    break;
                case eJoystickKey.kLeftStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Left).axis.y;
                    break;
                case eJoystickKey.kRightStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Right).axis.x;
                    break;
                case eJoystickKey.kRightStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Right).axis.y;
                    break;
                // are these used here?
                case eJoystickKey.kR1:
                    break;
                case eJoystickKey.kR2:
                    break;
                case eJoystickKey.kL1:
                    break;
                case eJoystickKey.kL2:
                    break;
                default:
                    break;
            }

            return false;
        }

        #endregion

        #region Disable Idling

        public static bool GetIdleExtraName(ref string __result)
        {
            __result = "empty_anim";
            return false;
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
        {
            __instance.SetAlpha(1);
            VRRig.Instance.transform.Find("Controller (left)/ActuallyLeftHand/handpad").GetComponent<MeshRenderer>().sharedMaterial = __instance.m_reminderRenderer.material;
        }

        #endregion

        #region Highlight Manager

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

        public static void HotspotObjectInteract(T_6FD30C1C _1BAF664A9) => _1BAF664A9.m_lookAt = null;

        public static bool IsHotspotOnScreen(T_8F74F848 __instance, ref bool __result)
        {
            if (__instance.m_anchor == null || __instance.m_anchor.m_anchorObj == null)
            {
                __result = false;
                return false;
            }

            float distance = Vector3.Distance(VRRig.Instance.Camera.transform.position, __instance.m_anchor.m_anchorObj.transform.position);
            if (distance < 20)
            {
                __instance._14888EF3 = 1f;
                __result = true;
                return false;
            }
            else
                __instance._14888EF3 = 0f;

            __result = false;
            return false;
        }

        public static bool IsInteractOnScreen(T_572A4969 __instance, ref bool __result)
        {
            if (__instance.m_anchor != null && __instance.m_anchor.m_anchorObj != null)
            {
                T_6FD30C1C hotspotObj = __instance._133075675;
                float num = Vector3.Angle(VRRig.Instance.Camera.transform.forward, __instance.m_anchor.m_anchorObj.transform.position - VRRig.Instance.Camera.transform.position);
                if (num < 90f)
                {
                    float distance = Vector3.Distance(VRRig.Instance.Camera.transform.position, __instance.m_anchor.m_anchorObj.transform.position);
                    if (distance < 3)
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

        public static bool IsHoverObjectOnScreen(T_A0A6EA62 __instance, ref bool __result)
        {
            Vector3 vector = VRRig.Instance.Camera.Component.WorldToScreenPoint(__instance.m_anchor.m_anchorObj.transform.position);
            if (vector.x > 0f && vector.y > 0f && vector.x < VRRig.Instance.Camera.Component.pixelWidth && vector.y < VRRig.Instance.Camera.Component.pixelHeight)
            {
                float num = VRRig.Instance.Camera.Component.pixelWidth * T_A0A6EA62._1D66D99B4;

                if (vector.x < num)
                    __instance._14888EF3 = Mathf.Lerp(0f, 1f, vector.x / num);
                else if (vector.x > VRRig.Instance.Camera.Component.pixelWidth - num)
                    __instance._14888EF3 = Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelWidth - vector.x) / num);
                else
                    __instance._14888EF3 = 1f;

                num = VRRig.Instance.Camera.Component.pixelHeight * T_A0A6EA62._1D66D99B4;

                if (vector.y < num)
                    __instance._14888EF3 *= Mathf.Lerp(0f, 1f, vector.y / num);
                else if (vector.y > VRRig.Instance.Camera.Component.pixelHeight - num)
                    __instance._14888EF3 *= Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelHeight - vector.y) / num);

                __instance._1649E566F();
                __result = true;
                return false;
            }
            __instance._1649E566F();
            __result = false;
            return false;
        }

        #endregion

        #region Tutorial Fixes

        public static bool UpdateTutorialUI(T_F6DEE320 __instance)
        {
            if (__instance.flags.Contains("tutorial2") // objective reminders
                || __instance.flags.Contains("tutorial5")
                || __instance.flags.Contains("tutorial7"))
                return false;

            return true;
        }

        #endregion

        #region Misc

        public static void PostCharControllerStart(T_C3DD66D9 __instance)
        {
            VRRig.Instance?.UpdateCachedChloe(__instance);

            #region Disable Idling

            /*AnimationClip clip = new AnimationClip();
            clip.name = "empty_anim";
            clip.legacy = true;
            __instance.m_animation.AddClip(clip, "empty_anim");

            foreach (AnimationState state in __instance.m_animStates)
            {
                if (state.name.ToLower().Contains("idle"))
                {
                    state.weight = 0;
                    state.speed = 0;
                    state.time = 0;
                }
            }*/

            #endregion

            #region Add Hand Material

            Material material = null;
            foreach (SkinnedMeshRenderer sMesh in __instance.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Material possibleMat = sMesh.sharedMaterials?.SingleOrDefault((m) => m.name.Contains("Arms"));
                if (possibleMat != null)
                    material = possibleMat;
            }
            material.hideFlags = HideFlags.DontUnloadUnusedAsset;
            foreach (MeshRenderer renderer in VRRig.Instance.HandMeshRenderers)
                renderer.sharedMaterial = material;
            VRRig.Instance.ChloeMaterial = material;

            #endregion
        }

        public static void SetCameraPosition(Camera _13A97A3A2, Vector3 _1ACF98885)
        {
            // todo: fade between large jumps
            if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
            {
                if (Vector3.Distance(VRRig.Instance.transform.position, _1ACF98885) > 20)
                {
                    // todo: test
                    SteamVR_Fade.Start(Color.clear, 0);
                    SteamVR_Fade.Start(Color.black, 0.1f);
                }

                //MelonLogger.Msg("Distance between Camera positions: " + Vector3.Distance(VRRig.Instance.transform.position, _1ACF98885));
                VRRig.Instance.transform.position = _1ACF98885 - new Vector3(0, 1, 0);
                Vector3 rot = _13A97A3A2.transform.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                VRRig.Instance.transform.eulerAngles = rot;
            }
        }

        private static readonly string[] scrollingTextOptions =
        {
            "Join the Flat2VR Discord (https://flat2vr.com) for more Flatscreen To VR mods!",
            "Support me on Ko-fi! https://ko-fi.com/trevtv",
            "I hope Deck Nine approves of this mod...",
            "Thank you for trying my VR mod!"
        };

        public static bool ReplaceScrollingText(ref string __result)
        {
            __result = scrollingTextOptions[UnityEngine.Random.Range(0, scrollingTextOptions.Length)];
            return false;
        }

        public static bool BoundaryStart(T_3BE79CFB __instance)
        {
            __instance.GetComponent<Collider>().isTrigger = false;
            return false;
        }

        public static void OnPPEnable(T_190FC323 __instance)
        {
            if (VRRig.Instance.Camera.GetComponent<DawnVRMod.Modules.VR.VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = VRRig.Instance.Camera.gameObject.AddComponent<DawnVRMod.Modules.VR.VRPostProcessing>();
            vpp.profile = __instance.profile;
        }

        #endregion

        #region NoVR Patches

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            // Debug Stuff
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), nameof(ReturnTrue));
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), nameof(ReturnTrue));
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), nameof(DisableSplashScreen));
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed));
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), nameof(OnSetMode2));
            PatchPre(typeof(T_421B9CDF).GetMethod("SetCameraPosition"), nameof(SetCameraPosition2));
        }

        public static void SetCameraPosition2(Camera _13A97A3A2, Vector3 _1ACF98885)
        {
            if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
            {
                float f = Vector3.Distance(_13A97A3A2.transform.position, _1ACF98885);
                MelonLogger.Msg("Distance between Camera positions: " + f.ToString());
            }
        }

        public static void OnPPEnable2(T_190FC323 __instance)
        {
            if (__instance.GetComponent<DawnVRMod.Modules.VR.VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = __instance.gameObject.AddComponent<DawnVRMod.Modules.VR.VRPostProcessing>();
            vpp.profile = __instance.profile;
        }

        public static void OnSetMode2(bool __result, eGameMode _1C57B7248)
        {
            if (__result)
            {
                MelonLogger.Msg("Game successfully updated to mode " + _1C57B7248);
            }
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