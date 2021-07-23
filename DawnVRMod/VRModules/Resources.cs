using System;
using Valve.VR;
using UnityEngine;

namespace VRMod
{
    internal static class Resources
    {
        public static GameObject VRCameraRig => vrCameraRig;

        public static void Init()
        {
            #region VRCameraRig

            Shader standard = Shader.Find("Standard");
            AssetBundle camRig = ResourceLoader.GetAssetBundle("camerarig");
            vrCameraRig = camRig.LoadAssetWithHF<GameObject>("Assets/SteamVR/Prefabs/[VRCameraRig].prefab");
            vrCameraRig.AddComponent<SteamVR_PlayArea>().drawInGame = false;
            SteamVR_Behaviour_Pose pose1 = vrCameraRig.transform.Find("Controller (left)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose pose2 = vrCameraRig.transform.Find("Controller (right)").gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose1.poseAction = SteamVR_Actions._default.Pose;
            pose2.poseAction = SteamVR_Actions._default.Pose;
            pose1.inputSource = SteamVR_Input_Sources.LeftHand;
            pose2.inputSource = SteamVR_Input_Sources.RightHand;
            pose1.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;
            pose2.transform.Find("Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = standard;

            #endregion
        }

        private static GameObject vrCameraRig;
    }
}
