using System;
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

        public static void Init()
        {
            #region VRCameraRig

            AssetBundle dataBundle = ResourceLoader.GetAssetBundle("data");
            VRCameraRig = dataBundle.LoadAssetWithHF<GameObject>("Assets/SteamVR/Prefabs/[VRCameraRig].prefab");
            VRCameraRig.AddComponent<SteamVR_PlayArea>().drawInGame = shouldRenderCameraRig;
            SteamVR_Behaviour_Pose pose1 = VRCameraRig.transform.Find("Controller (left)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose pose2 = VRCameraRig.transform.Find("Controller (right)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose1.poseAction = SteamVR_Actions._default.Pose;
            pose2.poseAction = SteamVR_Actions._default.Pose;
            pose1.inputSource = SteamVR_Input_Sources.LeftHand;
            pose2.inputSource = SteamVR_Input_Sources.RightHand;
            pose1.transform.Find("CustomModel (Chloe)/handpad").GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");
            DitherShader = pose1.transform.Find("CustomModel (Chloe)").GetComponent<MeshRenderer>().material.shader;
            DitherShader.hideFlags = HideFlags.DontUnloadUnusedAsset;
            if (shouldRenderCameraRig)
            {
                Shader standard = Shader.Find("Standard");
                pose1.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;
                pose2.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;
            }

            #endregion

            SteamFadeShader = dataBundle.LoadAssetWithHF<Shader>("Assets/SteamVR/Resources/SteamVR_Fade.shader");
            MirrorShader = dataBundle.LoadAssetWithHF<Shader>("Assets/AssetBundleData/Mirror/Mirror.shader");
        }

        private static readonly bool shouldRenderCameraRig = false;
    }
}
