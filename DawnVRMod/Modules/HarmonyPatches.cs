using System;
using HarmonyLib;
using MelonLoader;
using System.Reflection;
using DawnVR.Modules.VR;
#if REMASTER
using UnityEngine.PostProcessing;
using PrototyperData;
using CriMana;
#else
using UnityEngine._1F1547F66;
using SplashScreenWindow = T_EDB11480;
using InputManager = T_6FCAE66C;
using JoystickInputManager = T_D9E8342E;
using CharController = T_C3DD66D9;
using FollowCamera = T_884A92DB;
using ButtonMenu = T_51AF6A60;
using LevelManager = T_91FF9D92;
using GameMaster = T_A6E913D1;
using GameModeManager = T_6B664603;
using FreeroamObject = _15C6DD6D9.T_A0855345;
using ChloeReminderDisplay = T_81803C2C;
using ObjectiveReminder = T_1928221C;
using HotspotObject = T_244D769F;
using CUICameraRelative = T_1C1609D7;
using CUI3DAnchor = T_2D9F19A8;
using FreeRoamWindow = T_F8FE3E1C;
using HotSpotUI = T_8F74F848;
using InteractUI = T_572A4969;
using HoverObjectUI = T_A0A6EA62;
using CameraUtils = T_421B9CDF;
using UI3DCamera = T_D4EA31BB;
using DawnUI = T_408CFC35;
using TutorialWindow = T_64B68373;
using UIDrawCall = T_632CCBA1;
using IMScene = T_E29491C9;
using Boundary = T_3BE79CFB;
using ChoiceSelectionUI = T_6876113C;
using TelescopePuzzle = T_24E8F007;
using Telescope = T_ADD17E7F;
using Player = _1F28E2E62.T_E579AD8A;
using MirrorReflection = T_55EA835B;
using DawnMainCamera = T_34182F31;
using PostProcessingDofPass = T_C0F7FD02;
using CriAtomListener = T_165E4FE4;
using ST_ParallelHighlight = T_FD3AF1C2;
using BrightnessUI = T_32770A6A;
using MainMenuUI = T_96E81635;
#endif

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
            PatchPost(typeof(SplashScreenWindow).GetMethod("StartSplash"), nameof(DisableSplashScreen)); // skips most of the splash screens
            //PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed)); // allows skipping any cutscene

            // InputOverrides
#if REMASTER
            PatchPre(typeof(InputManager).GetMethod("RestoreDefaults"), nameof(ManagerInit)); // makes the game think we're using an xbox controller
#else
            PatchPre(typeof(InputManager).GetMethod("Init".ToMethodName()), nameof(ManagerInit)); // makes the game think we're using an xbox controller
#endif
            PatchPre(typeof(InputManager).GetMethod("GetInputState",
                new Type[] { typeof(eGameInput), typeof(bool), typeof(bool), typeof(bool) }), nameof(GetInputState_Enum)); // redirect input to vr controllers
            PatchPre(typeof(JoystickInputManager).GetMethod("GetAxis"), nameof(GetAxis)); // redirect input to vr controllers

            // RigMovement
            PatchPre(typeof(CharController).GetMethod("CalculateAngle"), nameof(CalculateCharAngle)); // overrides it so it doesnt actually calculate the angle, as VRRig and CharControllerMove handles that
            PatchPre(typeof(CharController).GetMethod("Move"), nameof(CharControllerMove)); // improves the game's movement controller to better fit vr
            PatchPre(typeof(CharController).GetMethod("Rotate"), nameof(CharControllerRotate)); // slightly improves roomscale collision
            PatchPre(typeof(CharController).GetMethod("Update"), nameof(CharControllerUpdate)); // copy-paste of CharController::Update to fix a small bug, jank but i dont care
            PatchPre(typeof(FollowCamera).GetProperty("isFreeroamStart").GetSetMethod(), nameof(DontRunMe)); // fixes some movement issues
            PatchPre(typeof(ButtonMenu).GetMethod("Start"), nameof(AddVRCalibrationButton)); // adds the VR Calibration button to the main menu

            // RigParentModifer
            PatchPre(typeof(LevelManager).GetMethod("UnloadCurrentLevel"), nameof(UnloadCurrentLevel)); // prevents the vrrig from getting destroyed after unloading a scene
            PatchPre(typeof(GameMaster).GetMethod("Restart"), nameof(UnloadCurrentLevel)); // prevents the vrrig from getting destroyed after unloading a scene
            PatchPost(typeof(GameModeManager).GetMethod("SetMode"), nameof(OnSetMode)); // lets VRRig know when the game's mode changes
            PatchPost(typeof(FreeroamObject).GetMethod("Activate"), nameof(OnFreeroamObjectActivate)); // pretty jank fix for some sequences constantly getting offset

            // ObjectiveManager
            PatchPost(typeof(ChloeReminderDisplay).GetMethod("SetReminder"), nameof(SetReminderTexture)); // adds the reminder to the vr hands
            PatchPre(typeof(ObjectiveReminder).GetMethod("Update", AccessTools.all), nameof(DontRunMe)); // makes it so the objective view button doesnt work since it's useless in vr

            // HighlightManager
            PatchPre(typeof(HotspotObject).GetMethod("Interact"), nameof(HotspotObjectInteract)); // prevents some weird bug
            PatchPre(typeof(CUICameraRelative).GetMethod("Update"), nameof(CUICameraRelativeUpdate)); // overrides the ui3d camera with the vrcamera rotation
            PatchPre(typeof(CUI3DAnchor).GetMethod("UpdatePosition"), nameof(CUIAnchorUpdatePosition)); // makes it so OverlayPosition isnt needed
            PatchPre(typeof(FreeRoamWindow).GetMethod("Update"), nameof(FreeroamWindowUpdate)); // overrides some references to use the vr camera
            PatchPre(typeof(HotSpotUI).GetMethod("CheckOnScreen"), nameof(IsHotspotOnScreen)); // HotSpotUI, makes it use the vr camera for calculations
            PatchPre(typeof(InteractUI).GetMethod("CheckOnScreen"), nameof(IsInteractOnScreen)); // InteractUI, makes it use the vr camera for calculations
            PatchPre(typeof(HoverObjectUI).GetMethod("CheckOnScreen"), nameof(IsHoverObjectOnScreen)); // HoverObjectUI, makes it use the vr camera for calculations

            // UIFixes
            PatchPre(typeof(CameraUtils).GetMethod("SetCameraCullingMask"), nameof(SetCameraCullingMask)); // fix random nullrefs part 1
            PatchPre(typeof(UI3DCamera).GetMethod("Reset"), nameof(DontRunMe)); // fix random nullrefs part 2
            PatchPre(typeof(DawnUI).GetMethod("UpdateFade"), nameof(UpdateUIFade)); // makes fades use SteamVR_Fade instead of a transition window
            PatchPre(typeof(TutorialWindow).GetMethod("SetTutorial"), nameof(SetTutorialInfo)); // fixes the issue after disabling the objective reminder button
            PatchPost(typeof(UIDrawCall).GetMethod("CreateMaterial".ToMethodName(), AccessTools.all), nameof(CreateDrawCallMat)); // overrides the interaction ui's shader to force it to always stay over everything
            PatchPost(typeof(IMScene).GetMethod("Start", AccessTools.all), nameof(FixFakeFogQueue)); // hooks scene root's start, there is probably a better way i dont know of

            // InteractionFixes
            PatchPre(typeof(Boundary).GetMethod("Start", AccessTools.all), nameof(BoundaryStart)); // prevents a bug with the boundaries
            PatchPre(typeof(Boundary).GetMethod("OnTriggerEnter", AccessTools.all), nameof(DontRunMe)); // part 2 of the boundary issue fix
            PatchPre(typeof(FollowCamera).GetMethod("CalculateInputVariables".ToMethodName(), AccessTools.all), nameof(FollowCamUpdateInputVars)); // makes camera drives use the left thumbstick
            PatchPost(typeof(ChoiceSelectionUI).GetMethod("ButtonPressed"), nameof(ChoiceButtonSelection)); // fixes some weird interaction bugs

            // CutsceneFixes
            PatchPost(typeof(DawnUI).GetProperty("currentViewCookie").GetSetMethod(), nameof(DisableSettingCurrentViewCookie)); // disables overlays in some scenes
            PatchPre(typeof(TelescopePuzzle).GetMethod("Update", AccessTools.all), nameof(TelescopePuzzleUpdate)); // fixes the amulet thing in e4
            PatchPre(typeof(Telescope).GetMethod("Update", AccessTools.all), nameof(TelescopeRotate)); // fixes the amulet thing in e4
            PatchPre(typeof(FollowCamera).GetMethod("SetupCameraMatrix".ToMethodName(), AccessTools.all), nameof(SetupFollowCameraMatrix)); // fixes a null ref
            PatchPost(typeof(Player).GetMethod("OnWillRenderObject"), nameof(OnMovieWillRenderObject)); // displays pre-rendered videos in the cutscene box
            PatchPre(typeof(CameraUtils).GetMethod("SetMainCameraFOV".ToMethodName()), nameof(SetMainCamFOV)); // fixes weird fov stuff

            // Misc Fixes
            PatchPre(typeof(MirrorReflection).GetMethod("Awake", AccessTools.all), nameof(MirrorReflectionAwake)); // overrides the mirror component with a modified one made for vr
            PatchPre(typeof(DawnMainCamera).GetProperty("MainUICamera").GetGetMethod(), nameof(GetMainUICamera)); // fixes an uncommon null ref hopefully
            PatchPre(typeof(PostProcessingDofPass).GetMethod("OnRenderImage", AccessTools.all), nameof(DontRunMe)); // fixes a random null ref
            PatchPre(typeof(CriAtomListener).GetMethod("OnEnable", AccessTools.all), nameof(DestroyAtomListener)); // fixes audio crackling
            PatchPre(typeof(CharController).GetMethod("SetAttachObjActive".ToMethodName(), AccessTools.all), nameof(DontRunMe)); // prevents AttachObjects from displaying since they arent connected to the vr hands
            PatchPre(typeof(ST_ParallelHighlight).GetMethod("Trigger"), nameof(ParallelHighlightTrigger)); // fixes a random null ref

            // Misc
            PatchPost(typeof(CharController).GetMethod("Start"), nameof(PostCharControllerStart)); // updates the current VRRig::ChloeComponent
            PatchPost(typeof(BrightnessUI).GetMethod("OnSliderChange"), nameof(OnChangeBrightnessSetting)); // allows gamma adjustments in-headset
            PatchPost(typeof(BrightnessUI).GetMethod("Update", AccessTools.all), nameof(OnBrightnessUIUpdate)); // fix gamma slider
            PatchPre(typeof(MainMenuUI).GetProperty("ScrollingText").GetGetMethod(), nameof(ReplaceScrollingText)); // adds a personal touch lol

            // post processing doesnt seem to render correctly in vr, so this is gonna stay disabled
            //PatchPre(typeof(T_190FC323).GetMethod("OnEnable", AccessTools.all), nameof(OnPPEnable));
        }

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPost(typeof(SplashScreenWindow).GetMethod("StartSplash"), nameof(DisableSplashScreen));
            PatchPre(typeof(GraphManager).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed));
        }

        public static bool DontRunMe() => false;

        #region Misc

        public static void PostCharControllerStart(CharController __instance) 
            => VRRig.Instance?.UpdateCachedChloe(__instance);

        public static void OnChangeBrightnessSetting()
            => VRRig.Instance.Camera.UpdateBrightness();

        public static void OnBrightnessUIUpdate(BrightnessUI __instance)
            => __instance.m_slider.OnPan(VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Left).axis.normalized / 100);

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

#if REMASTER
        public static bool UnhollowerWarningPrefix(string __0) => !__0.Contains("unsupported return type") && !__0.Contains("unsupported parameter") && !__0.Contains("called directly from anywhere");
#endif

        #endregion
    }
}