﻿using System;
using Valve.VR;
using UnityEngine;

namespace DawnVR.Modules
{
    internal static class Resources
    {
        public static GameObject VRCameraRig { get; private set; }
        public static Shader SteamFadeShader { get; private set; }
        public static Shader MirrorShader { get; private set; }
        public static Shader DitherShader { get; private set; }
        public static Shader NGUIOverlayShader { get; private set; }
        public static Shader StandardAssetsOverlayShader { get; private set; }
        public static Texture2D WhitePixelTexture { get; private set; }

        public static string OpenLeft { get; private set; }
        public static string OpenRight { get; private set; }
        public static string ClosedLeft { get; private set; }
        public static string ClosedRight { get; private set; }

        public static GameObject CutsceneRoom { get; private set; }
        public static GameObject CalibrationUI { get; private set; }

        public static void Init()
        {
            string bundle = MelonLoader.MelonUtils.IsGameIl2Cpp() ? "data_rm" : "data";
            AssetBundle dataBundle = ResourceLoader.GetAssetBundle(bundle);

            #region VRCameraRig

            VRCameraRig = dataBundle.LoadAssetWithHF<GameObject>("Assets/AssetBundleData/CameraRig/[VRCameraRig].prefab");
            SteamVR_Behaviour_Pose pose1 = VRCameraRig.transform.Find("CameraHolder/Controller (left)").gameObject.GetOrAddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose pose2 = VRCameraRig.transform.Find("CameraHolder/Controller (right)").gameObject.GetOrAddComponent<SteamVR_Behaviour_Pose>();
            pose1.poseAction = SteamVR_Actions._default.Pose;
            pose2.poseAction = SteamVR_Actions._default.Pose;
            pose1.inputSource = SteamVR_Input_Sources.LeftHand;
            pose2.inputSource = SteamVR_Input_Sources.RightHand;
            pose1.transform.Find("CustomModel (Chloe)/handpad").GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");
            DitherShader = pose1.transform.Find("CustomModel (Chloe)").GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
            DitherShader.hideFlags = HideFlags.DontUnloadUnusedAsset;
            VRCameraRig.GetOrAddComponent<SteamVR_PlayArea>().drawInGame = shouldRenderCameraRig;
            if (shouldRenderCameraRig)
            {
                Shader standard = Shader.Find("Standard");
                pose1.transform.Find("Model").gameObject.GetOrAddComponent<SteamVR_RenderModel>().shader = standard;
                pose2.transform.Find("Model").gameObject.GetOrAddComponent<SteamVR_RenderModel>().shader = standard;
            }

            #endregion

            SteamFadeShader = dataBundle.LoadAssetWithHF<Shader>("Assets/SteamVR/Resources/SteamVR_Fade.shader");
            MirrorShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/Mirror/Mirror.shader");
            NGUIOverlayShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/UnlitTransparentColored.shader");
            StandardAssetsOverlayShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/CameraRig/BrightnessOverlay/BlendModesOverlay.shader");
            WhitePixelTexture = dataBundle.LoadAssetWithHF<Texture2D>("Assets/AssetBundleData/CameraRig/BrightnessOverlay/white_pixel.png");

            CutsceneRoom = dataBundle.LoadAssetWithHF<GameObject>("Assets/AssetBundleData/CutsceneBox.prefab");
            CalibrationUI = dataBundle.LoadAssetWithHF<GameObject>("Assets/AssetBundleData/Calibration/CalibrationUI.prefab");

            #region Handposes

            OpenLeft = ResourceLoader.GetText("Handposes.OpenLeft.json");
            OpenRight = ResourceLoader.GetText("Handposes.OpenRight.json");
            ClosedLeft = ResourceLoader.GetText("Handposes.ClosedLeft.json");
            ClosedRight = ResourceLoader.GetText("Handposes.ClosedRight.json");

            #endregion
        }

        private static readonly bool shouldRenderCameraRig = false;
    }
}
