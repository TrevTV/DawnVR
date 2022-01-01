using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using MelonLoader;
using System.Reflection;
using DawnVR.Modules.VR;
using UnityEngine._1F1547F66;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static HarmonyLib.Harmony HarmonyInstance;

        private static void PatchPre(MethodInfo original, string prefixMethodName) => HarmonyInstance.Patch(original, typeof(HarmonyPatches).GetMethod(prefixMethodName).ToNewHarmonyMethod());
        private static void PatchPost(MethodInfo original, string postfixMethodName) => HarmonyInstance.Patch(original, null, typeof(HarmonyPatches).GetMethod(postfixMethodName).ToNewHarmonyMethod());

        public static void Init(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            // Debug
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), nameof(DisableSplashScreen)); // skips most of the splash screens
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed)); // allows skipping any cutscene

            // InputOverrides
            PatchPre(typeof(T_6FCAE66C).GetMethod("_1B350D183", AccessTools.all), nameof(ManagerInit)); // makes the game think we're using an xbox controller
            PatchPre(typeof(T_6FCAE66C).GetMethod("GetInputState",
                new Type[] { typeof(eGameInput), typeof(bool), typeof(bool), typeof(bool) }), nameof(GetInputState_Enum)); // redirect input to vr controllers
            PatchPre(typeof(T_D9E8342E).GetMethod("GetButtonState"), nameof(GetButtonState)); // redirect input to vr controllers
            PatchPre(typeof(T_D9E8342E).GetMethod("GetAxis"), nameof(GetAxis)); // redirect input to vr controllers

            // RigMovement
            //PatchPost(typeof(T_421B9CDF).GetMethod("SetCameraPosition"), nameof(SetCameraPosition)); // moves VRRig to follow the camera during a cutscene
            PatchPre(typeof(T_C3DD66D9).GetMethod("CalculateAngle"), nameof(CalculateCharAngle)); // overrides it so it doesnt actually calculate the angle, as VRRig and CharControllerMove handles that
            PatchPre(typeof(T_C3DD66D9).GetMethod("Move"), nameof(CharControllerMove)); // improves the game's movement controller to better fit vr
            PatchPre(typeof(T_C3DD66D9).GetMethod("Rotate"), nameof(CharControllerRotate)); // slightly improves roomscale collision
            PatchPre(typeof(T_C3DD66D9).GetMethod("Update"), nameof(CharControllerUpdate)); // copy-paste of CharController::Update to fix a small bug, jank but i dont care
            PatchPre(typeof(T_884A92DB).GetProperty("isFreeroamStart").GetSetMethod(), nameof(DontRunMe)); // fixes some movement issues
            PatchPre(typeof(T_51AF6A60).GetMethod("Start"), nameof(AddVRCalibrationButton)); // adds the VR Calibration button to the main menu

            // RigParentModifer
            PatchPre(typeof(T_91FF9D92).GetMethod("UnloadCurrentLevel"), nameof(UnloadCurrentLevel)); // prevents the vrrig from getting destroyed after unloading a scene
            PatchPre(typeof(T_A6E913D1).GetMethod("Restart"), nameof(UnloadCurrentLevel)); // prevents the vrrig from getting destroyed after unloading a scene
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), nameof(OnSetMode)); // lets VRRig know when the game's mode changes

            // ObjectiveManager
            PatchPost(typeof(T_81803C2C).GetMethod("SetReminder"), nameof(SetReminderTexture)); // adds the reminder to the vr hands
            PatchPre(typeof(T_1928221C).GetMethod("Update", AccessTools.all), nameof(DontRunMe)); // makes it so the objective view button doesnt work since it's useless in vr

            // HighlightManager
            PatchPre(typeof(T_244D769F).GetMethod("Interact"), nameof(HotspotObjectInteract)); // prevents some weird bug
            PatchPre(typeof(T_1C1609D7).GetMethod("Update"), nameof(CUICameraRelativeUpdate)); // overrides the ui3d camera with the vrcamera rotation
            PatchPre(typeof(T_2D9F19A8).GetMethod("UpdatePosition"), nameof(CUIAnchorUpdatePosition)); // makes it so OverlayPosition isnt needed
            PatchPre(typeof(T_F8FE3E1C).GetMethod("Update"), nameof(FreeroamWindowUpdate)); // overrides some references to use the vr camera
            PatchPre(typeof(T_8F74F848).GetMethod("CheckOnScreen"), nameof(IsHotspotOnScreen)); // HotSpotUI, makes it use the vr camera for calculations
            PatchPre(typeof(T_572A4969).GetMethod("CheckOnScreen"), nameof(IsInteractOnScreen)); // InteractUI, makes it use the vr camera for calculations
            PatchPre(typeof(T_A0A6EA62).GetMethod("CheckOnScreen"), nameof(IsHoverObjectOnScreen)); // HoverObjectUI, makes it use the vr camera for calculations

            // UIFixes
            //PatchPre(typeof(T_34182F31).GetProperty("MainUICamera").GetGetMethod(), nameof(GetMainUICamera)); // fixes nullrefs after get_MainUICamera() tries to use Camera.main
            PatchPre(typeof(T_421B9CDF).GetMethod("SetCameraCullingMask"), nameof(SetCameraCullingMask)); // fix random nullrefs part 1
            PatchPre(typeof(T_D4EA31BB).GetMethod("Reset"), nameof(DontRunMe)); // fix random nullrefs part 2
            PatchPre(typeof(T_408CFC35).GetMethod("UpdateFade"), nameof(UpdateUIFade)); // makes fades use SteamVR_Fade instead of a transition window
            PatchPre(typeof(T_64B68373).GetMethod("SetTutorial"), nameof(SetTutorialInfo)); // fixes the issue after disabling the objective reminder button
            PatchPost(typeof(T_632CCBA1).GetMethod("_158268DAA", AccessTools.all), nameof(CreateDrawCallMat)); // overrides the interaction ui's shader to force it to always stay over everything
            PatchPost(typeof(T_E29491C9).GetMethod("Start", AccessTools.all), nameof(FixFakeFogQueue)); // hooks scene root's start, there is probably a better way i dont know of

            // InteractionFixes
            PatchPre(typeof(T_3BE79CFB).GetMethod("Start", AccessTools.all), nameof(BoundaryStart)); // prevents a bug with the boundaries
            PatchPre(typeof(T_3BE79CFB).GetMethod("OnTriggerEnter", AccessTools.all), nameof(DontRunMe)); // part 2 of the boundary issue fix
            //PatchPost(typeof(T_884A92DB).GetMethod("Start"), nameof(FollowCamStart)); // prevents bug with FollowCamera disabling interaction
            //PatchPre(typeof(T_884A92DB).GetMethod("LateUpdate"), nameof(FollowCamLateUpdate)); // part 2 of FollowCamera fix
            PatchPre(typeof(T_884A92DB).GetMethod("_15EB64374", AccessTools.all), nameof(FollowCamUpdateInputVars)); // makes camera drives use the left thumbstick
            PatchPost(typeof(T_6876113C).GetMethod("ButtonPressed"), nameof(ChoiceButtonSelection)); // fixes some weird interaction bugs

            // Misc
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), nameof(PostCharControllerStart)); // mainly updates VRRig's chloe and material
            PatchPre(typeof(T_96E81635).GetProperty("ScrollingText").GetGetMethod(), nameof(ReplaceScrollingText)); // adds a personal touch lol
            PatchPre(typeof(T_55EA835B).GetMethod("Awake", AccessTools.all), nameof(MirrorReflectionAwake)); // overrides the mirror component with a modified one made for vr
            PatchPost(typeof(T_408CFC35).GetProperty("currentViewCookie").GetSetMethod(), nameof(SetCurrentViewCookie)); // sets overlays for mainly binocular scenes
            PatchPre(typeof(T_884A92DB).GetMethod("_1430D6986", AccessTools.all), nameof(SetupFollowCameraMatrix)); // fixes a null ref
            // post processing doesnt seem to render correctly in vr, so this is gonna stay disabled
            //PatchPre(typeof(T_190FC323).GetMethod("OnEnable", AccessTools.all), nameof(OnPPEnable));
        }

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), nameof(ReturnTrue));
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), nameof(ReturnTrue));
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), nameof(DisableSplashScreen));
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed));
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), nameof(OnSetMode2));
        }

        public static bool DontRunMe() => false;

        #region Misc

        public static void PostCharControllerStart(T_C3DD66D9 __instance) 
            => VRRig.Instance?.UpdateCachedChloe(__instance);

        public static bool MirrorReflectionAwake(T_55EA835B __instance)
        {
            __instance.enabled = false;
            __instance.GetComponent<MeshRenderer>().sharedMaterial.shader = Resources.MirrorShader;
            VRMirrorReflection reflection = __instance.gameObject.AddComponent<VRMirrorReflection>();
            __instance.gameObject.layer = 16; // sets layer to "UI3D" to cull it from other mirrors
            reflection.m_TextureSize = __instance.m_TextureSize;
            reflection.m_ClipPlaneOffset = __instance.m_ClipPlaneOffset;
            reflection.m_ReflectLayers = __instance.m_ReflectLayers;
            return false;
        }

        public static void SetCurrentViewCookie(T_408CFC35 __instance)
        {
            T_A7F99C25.eCookieChoices cookie = __instance.currentViewCookie;
            //MelonLogger.Msg("View cookie change to " + cookie.ToString());

            switch (cookie)
            {
                case T_A7F99C25.eCookieChoices.kNone:
                    VRRig.Instance.CutsceneHandler.EndCutscene();
                    if (VRRig.Instance?.ChloeComponent?.Camera != null)
                        VRRig.Instance.ChloeComponent.Camera.enabled = false;
                    break;
                case T_A7F99C25.eCookieChoices.kBinoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case T_A7F99C25.eCookieChoices.kE3Binoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case T_A7F99C25.eCookieChoices.kE4Binoculars:
                    // todo: implement cookies here
                    break;
            }
        }

        public static bool SetupFollowCameraMatrix(T_884A92DB __instance, Vector4 _1DD947C88, Vector4 _15E19D274)
        {
            if (__instance.m_isLineLocked)
            {
                if (__instance._19186BCE3)
                {
                    __instance._15479B337 *= -1f;
                }
                if (__instance._12E8B1EAF)
                {
                    __instance._15479B329 *= 0f;
                }
                if (__instance._15479B337 >= 0f && __instance._15479B329 >= 0f)
                {
                    __instance._1C0908541 -= Mathf.Max(__instance._15479B337, __instance._15479B329);
                }
                else if (__instance._15479B337 <= 0f && __instance._15479B329 <= 0f)
                {
                    __instance._1C0908541 -= Mathf.Min(__instance._15479B337, __instance._15479B329);
                }
                __instance._1C0908541 = Mathf.Clamp01(__instance._1C0908541);
                _1DD947C88 = Vector3.Lerp(__instance._18166372C, __instance._1854BFA4C, __instance._1C0908541);
                //_1DD947C88 += __instance._111890643;
                __instance._111890643 -= __instance._111890643 * ((Mathf.Abs(__instance._15479B337) + Mathf.Abs(__instance._15479B329)) * Time.deltaTime / 2f);
            }
            else if (__instance.IsLocked)
            {
                float num = -__instance._15479B337 * 57.29578f;
                float num2 = __instance._15479B329 * 57.29578f;
                __instance._1E9DAA452 += num;
                __instance._1CFFF5F80 += num2;
                ExtendedViewBase.GrabExtendedView(T_34182F31.main, ref __instance._1D9C76B26, ref __instance._1C89EAB96);
                if (__instance._1C89EAB96 != null && T_1005C221.T_A9DD5E3E.IsExtendedViewAvailable())
                {
                    float num3 = T_34182F31.main.fieldOfView / 40f;
                    float num4 = __instance._1C89EAB96.Yaw * num3;
                    float num5 = __instance._1C89EAB96.Pitch * num3;
                    __instance._1E9DAA452 += num4 - __instance._1DD2696DE;
                    __instance._1CFFF5F80 += num5 - __instance._1CE82F9BB;
                    __instance._1DD2696DE = num4;
                    __instance._1CE82F9BB = num5;
                }
                Transform transform = __instance._11A7A7D26.transform;
                if (__instance._1552E3BEF != Vector3.zero)
                {
                    transform.forward = __instance._1552E3BEF;
                    __instance._13782B1A6 = Vector3.Angle(new Vector3(__instance._1552E3BEF.x, 0f, __instance._1552E3BEF.z), __instance.centeredAimDirection);
                    if (__instance._1552E3BEF.y < 0f)
                    {
                        __instance._13782B1A6 *= -1f;
                    }
                }
                else
                {
                    transform.forward = new Vector3(0f, 0f, 1f);
                    __instance._13782B1A6 = 0f;
                }
                __instance._1CFFF5F80 = Mathf.Clamp(__instance._1CFFF5F80, -89.99f + __instance._13782B1A6, 89.99f + __instance._13782B1A6);
                if (__instance._15234BCAB)
                {
                    if (__instance._1DC1D4026.x >= 0f && __instance._1DC1D4026.y >= 0f)
                    {
                        __instance._1E9DAA452 = Mathf.Clamp(__instance._1E9DAA452, -__instance._1DC1D4026.x, __instance._1DC1D4026.y);
                    }
                    if (__instance._1DC1D4026.z >= 0f && __instance._1DC1D4026.w >= 0f)
                    {
                        __instance._1CFFF5F80 = Mathf.Clamp(__instance._1CFFF5F80, -__instance._1DC1D4026.z, __instance._1DC1D4026.w);
                    }
                }
                if (__instance._122F739CA)
                {
                    __instance._1CDC44FE7 = __instance._1CFFF5F80;
                    __instance._1DC9DF77C = __instance._1E9DAA452;
                    transform.forward = Quaternion.AngleAxis(__instance._1E9DAA452, Vector3.up) * transform.forward;
                    Vector3 axis = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance._1CFFF5F80, axis) * transform.forward;
                }
                else
                {
                    __instance._1CDC44FE7 = Mathf.SmoothDamp(__instance._1CDC44FE7, __instance._1CFFF5F80, ref __instance._18346372D, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, __instance.m_maxViewChangeSpeed, __instance._19FA60FD3());
                    __instance._1DC9DF77C = Mathf.SmoothDamp(__instance._1DC9DF77C, __instance._1E9DAA452, ref __instance._1293578B2, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, float.PositiveInfinity, __instance._19FA60FD3());
                    transform.forward = Quaternion.AngleAxis(__instance._1DC9DF77C, Vector3.up) * transform.forward;
                    Vector3 axis2 = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance._1CDC44FE7, axis2) * transform.forward;
                }
                __instance.transform.forward = transform.forward;
                return false;
            }
            Vector4 vector = _1DD947C88 - _15E19D274;
            vector.Normalize();
            Vector4 vector2 = new Vector4(-vector.z, 0f, vector.x);
            vector2.Normalize();
            Vector4 vector3 = Vector3.Cross(vector2, vector);
            vector3.Normalize();
            vector2 = Vector3.Cross(vector3, vector);
            vector2.Normalize();
            __instance._146F8D839(_15E19D274, vector2, vector3, vector);
            if (!__instance.IsLocked)
            {
                __instance._1DD2696DE = 0f;
                __instance._1CE82F9BB = 0f;
            }
            if (T_1005C221.T_A9DD5E3E.IsExtendedViewAvailable() && __instance.m_isCameraDriver && !T_A6E913D1.Instance.Paused && !__instance.m_isInteractionBlocked && __instance.IsLocked && (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam || T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kCustomization) && (!__instance.isFreeroamStart || __instance.m_isFreelook) && T_34182F31.main != null)
            {
                ExtendedViewBase.GrabExtendedView<ExtendedViewThirdPerson>(T_34182F31.main, ref __instance._1D9C76B26, ref __instance._1C89EAB96);
                if (__instance._1C89EAB96 != null)
                {
                    float num6 = T_34182F31.main.fieldOfView / 40f;
                    float num7 = __instance._1CFFF5F80 + __instance._1C89EAB96.Pitch * num6;
                    float num8 = __instance._1E9DAA452 + __instance._1C89EAB96.Yaw * num6;
                    if (__instance._15234BCAB)
                    {
                        if ((double)__instance._1DC1D4026.x >= 0.0 && (double)__instance._1DC1D4026.y >= 0.0)
                        {
                            num8 = Mathf.Clamp(num8, -__instance._1DC1D4026.x, __instance._1DC1D4026.y);
                        }
                        if ((double)__instance._1DC1D4026.z >= 0.0 && (double)__instance._1DC1D4026.w >= 0.0)
                        {
                            num7 = Mathf.Clamp(num7, -__instance._1DC1D4026.z, __instance._1DC1D4026.w);
                        }
                    }
                    __instance.transform.forward = Quaternion.AngleAxis(num8, Vector3.up) * __instance.transform.forward;
                    Vector3 axis3 = Vector3.Cross(Vector3.up, __instance.transform.forward);
                    __instance.transform.forward = Quaternion.AngleAxis(num7, axis3) * __instance.transform.forward;
                }
            }
            return false;
        }

        private static readonly string[] scrollingTextOptions =
        {
            "Join the Flat2VR Discord (http://flat2vr.com) for more Flatscreen To VR mods!",
            "Support me on Ko-fi! https://ko-fi.com/trevtv",
            "I hope Deck Nine approves of this mod...",
            "Thank you for trying my VR mod!"
        };

        public static bool ReplaceScrollingText(ref string __result)
        {
            __result = scrollingTextOptions[UnityEngine.Random.Range(0, scrollingTextOptions.Length)];
            return false;
        }

        public static void OnPPEnable(T_190FC323 __instance)
        {
            if (VRRig.Instance.Camera.GetComponent<VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = VRRig.Instance.Camera.gameObject.AddComponent<VRPostProcessing>();
            vpp.profile = __instance.profile;
        }

        #endregion
    }
}