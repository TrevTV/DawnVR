using System;
using System.Linq;
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
            PatchPre(typeof(T_6FCAE66C).GetMethod("_1B350D183", HarmonyLib.AccessTools.all), nameof(ManagerInit)); // makes the game think we're using an xbox controller
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
            PatchPre(typeof(T_1928221C).GetMethod("Update", HarmonyLib.AccessTools.all), nameof(DontRunMe)); // makes it so the objective view button doesnt work since it's useless in vr

            // HighlightManager
            PatchPre(typeof(T_244D769F).GetMethod("Interact"), nameof(HotspotObjectInteract)); // prevents some weird bug
            PatchPre(typeof(T_1C1609D7).GetMethod("Update"), nameof(CUICameraRelativeUpdate)); // overrides the ui3d camera with the vrcamera rotation
            PatchPre(typeof(T_2D9F19A8).GetMethod("UpdatePosition"), nameof(CUIAnchorUpdatePosition)); // makes it so OverlayPosition isnt needed
            PatchPre(typeof(T_F8FE3E1C).GetMethod("Update"), nameof(DontRunMe)); // honestly can't remember why this is here
            PatchPre(typeof(T_8F74F848).GetMethod("CheckOnScreen"), nameof(IsHotspotOnScreen)); // HotSpotUI, makes it use the vr camera for calculations
            PatchPre(typeof(T_572A4969).GetMethod("CheckOnScreen"), nameof(IsInteractOnScreen)); // InteractUI, makes it use the vr camera for calculations
            PatchPre(typeof(T_A0A6EA62).GetMethod("CheckOnScreen"), nameof(IsHoverObjectOnScreen)); // HoverObjectUI, makes it use the vr camera for calculations

            // UIFixes
            PatchPre(typeof(T_34182F31).GetProperty("MainUICamera").GetGetMethod(), nameof(GetMainUICamera)); // fixes nullrefs after get_MainUICamera() tries to use Camera.main
            PatchPre(typeof(T_421B9CDF).GetMethod("SetCameraCullingMask"), nameof(SetCameraCullingMask)); // fix random nullrefs part 1
            PatchPre(typeof(T_D4EA31BB).GetMethod("Reset"), nameof(DontRunMe)); // fix random nullrefs part 2
            PatchPre(typeof(T_408CFC35).GetMethod("UpdateFade"), nameof(UpdateUIFade)); // makes fades use SteamVR_Fade instead of a transition window
            PatchPre(typeof(T_64B68373).GetMethod("SetTutorial"), nameof(SetTutorialInfo)); // fixes the issue after disabling the objective reminder button

            // CustomizationFixes
            PatchPost(typeof(T_9806DB88).GetMethod("_19966578C", HarmonyLib.AccessTools.all), nameof(UpdateHighlightedItem));

            // InteractionFixes
            PatchPre(typeof(T_3BE79CFB).GetMethod("Start", HarmonyLib.AccessTools.all), nameof(BoundaryStart)); // prevents a bug with the boundaries
            PatchPre(typeof(T_3BE79CFB).GetMethod("OnTriggerEnter", HarmonyLib.AccessTools.all), nameof(DontRunMe)); // part 2 of the boundary issue fix
            PatchPost(typeof(T_884A92DB).GetMethod("Start"), nameof(FollowCamStart)); // prevents bug with FollowCamera disabling interaction
            PatchPre(typeof(T_884A92DB).GetMethod("LateUpdate"), nameof(FollowCamLateUpdate)); // part 2 of FollowCamera fix

            // Misc
            PatchPost(typeof(T_C3DD66D9).GetMethod("Start"), nameof(PostCharControllerStart)); // mainly updates VRRig's chloe and material
            PatchPre(typeof(T_96E81635).GetProperty("ScrollingText").GetGetMethod(), nameof(ReplaceScrollingText)); // adds a personal touch lol
            PatchPre(typeof(T_55EA835B).GetMethod("Awake", HarmonyLib.AccessTools.all), nameof(MirrorReflectionAwake)); // overrides the mirror component with a modified one made for vr
            // post processing doesnt seem to render correctly in vr, so this is gonna stay disabled
            //PatchPre(typeof(T_190FC323).GetMethod("OnEnable", HarmonyLib.AccessTools.all), nameof(OnPPEnable));
        }

        public static void InitNoVR(HarmonyLib.Harmony hInstance)
        {
            HarmonyInstance = hInstance;
            PatchPre(typeof(T_A6E913D1).GetMethod("IsAllowDebugOptions"), nameof(ReturnTrue));
            PatchPre(typeof(T_A6E913D1).GetMethod("IsTool"), nameof(ReturnTrue));
            PatchPost(typeof(T_EDB11480).GetMethod("StartSplash"), nameof(DisableSplashScreen));
            PatchPre(typeof(T_BF5A5EEC).GetMethod("SkipPressed"), nameof(CutsceneSkipPressed));
            PatchPost(typeof(T_6B664603).GetMethod("SetMode"), nameof(OnSetMode2));
            PatchPre(typeof(T_421B9CDF).GetMethod("SetCameraPosition"), nameof(SetCameraPosition2));
        }

        public static bool DontRunMe() => false;

        #region Misc

        public static void PostCharControllerStart(T_C3DD66D9 __instance)
        {
            VRRig.Instance?.UpdateCachedChloe(__instance);

            #region Add Hand Material

            Material material = null;
            foreach (SkinnedMeshRenderer sMesh in __instance.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                material = sMesh.sharedMaterials?.SingleOrDefault((m) => m.name.Contains("Arms"));
                if (material == null)
                    material = sMesh.sharedMaterials?.SingleOrDefault((m) => m.name.Contains("Farewell_Body"));
            }
            if (material == null)
            {
                MelonLogger.Error("Failed to locate the hand material in scene " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                VRRig.Instance.ChloeMaterial = new Material(Shader.Find("Standard"));
                return;
            }

            material.hideFlags = HideFlags.DontUnloadUnusedAsset;
            VRRig.Instance.ChloeMaterial = material;

            #endregion
        }

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