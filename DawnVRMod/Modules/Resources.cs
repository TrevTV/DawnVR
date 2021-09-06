using System;
using Valve.VR;
using UnityEngine;

namespace DawnVR.Modules
{
    internal static class Resources
    {
        public static GameObject VRCameraRig => vrCameraRig;
        public static Shader SteamFadeShader => steamFadeShader;
        public static Shader MirrorShader => mirrorShader;
        public static Material DitheredHandMaterial => ditheredHandMaterial;

        public static void Init()
        {
            #region VRCameraRig

            AssetBundle camRig = ResourceLoader.GetAssetBundle("camerarig");
            vrCameraRig = camRig.LoadAssetWithHF<GameObject>("Assets/SteamVR/Prefabs/[VRCameraRig].prefab");
            vrCameraRig.AddComponent<SteamVR_PlayArea>().drawInGame = shouldRenderCameraRig;
            SteamVR_Behaviour_Pose pose1 = vrCameraRig.transform.Find("Controller (left)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose pose2 = vrCameraRig.transform.Find("Controller (right)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose1.poseAction = SteamVR_Actions._default.Pose;
            pose2.poseAction = SteamVR_Actions._default.Pose;
            pose1.inputSource = SteamVR_Input_Sources.LeftHand;
            pose2.inputSource = SteamVR_Input_Sources.RightHand;
            pose1.transform.Find("ActuallyLeftHand/handpad").GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");
            ditheredHandMaterial = pose1.transform.Find("ActuallyLeftHand").GetComponent<MeshRenderer>().material;
            ditheredHandMaterial.hideFlags = HideFlags.DontUnloadUnusedAsset;
            if (shouldRenderCameraRig)
            {
                Shader standard = Shader.Find("Standard");
                pose1.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;
                pose2.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;
            }

            #endregion

            steamFadeShader = camRig.LoadAssetWithHF<Shader>("Assets/SteamVR/Resources/SteamVR_Fade.shader");
            mirrorShader = camRig.LoadAssetWithHF<Shader>("Assets/Mirror/Mirror.shader");
        }

        private static readonly bool shouldRenderCameraRig = false;
        private static GameObject vrCameraRig;
        private static Shader steamFadeShader;
        private static Shader mirrorShader;
        private static Material ditheredHandMaterial;
    }
}
