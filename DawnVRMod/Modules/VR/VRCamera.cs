using Valve.VR;
using UnityEngine;
using System.Collections.Generic;

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
        public Camera Component;
        public Shader OverlayShader;
        public RenderTexture RenderToVRTexture;
        public Transform Holder;

        private Camera uiCamera;
        private GameObject uiRenderer;
        private GameObject visionBlocker;
        private Dictionary<T_A7F99C25.eCookieChoices, GameObject> overlayCookies;

        private Camera spectatorCamera;

        private void Start()
        {
            Component = GetComponent<Camera>();
            gameObject.AddComponent<SteamVR_Fade>();

            Holder = transform.parent;
            visionBlocker = transform.Find("VisionBlocker").gameObject;
            visionBlocker.SetActive(true);

            overlayCookies = new Dictionary<T_A7F99C25.eCookieChoices, GameObject>()
            {
                { T_A7F99C25.eCookieChoices.kBinoculars, transform.Find("OverlayCookies/Binocular").gameObject }
                //{ T_A7F99C25.eCookieChoices.kE3Binoculars, transform.Find("OverlayCookies/E3Binocular").gameObject },
                //{ T_A7F99C25.eCookieChoices.kE4Binoculars, transform.Find("OverlayCookies/E4Binocular").gameObject }
            };

            Component.backgroundColor = Color.black;

            #region UI Renderer

            uiRenderer = transform.Find("UIRenderer").gameObject;
            uiRenderer.transform.localScale = new Vector3(0.25f, 0.15f, 0.15f);
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            RenderToVRTexture = new RenderTexture(Screen.width, Screen.height, 1);
            uiCamera.targetTexture = RenderToVRTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material.mainTexture = RenderToVRTexture;
            OverlayShader = uiRenderer.GetComponent<MeshRenderer>().material.shader;
            OverlayShader.hideFlags = HideFlags.DontUnloadUnusedAsset;
            uiRenderer.gameObject.SetActive(true);

            #endregion

            CreateSpectatorCamera();
        }

        private void CreateSpectatorCamera()
        {
            if (!Preferences.SpectatorEnabled.Value)
                return;

            Transform specTransform = new GameObject("Spectator").transform;
            specTransform.parent = transform;
            spectatorCamera = specTransform.gameObject.AddComponent<Camera>();
            CopyCameraProperties(Component, spectatorCamera);
            spectatorCamera.depth = 100;
            spectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;
            spectatorCamera.fieldOfView = Preferences.SpectatorFOV.Value;
            specTransform.localPosition = Vector3.zero;
            specTransform.localRotation = Quaternion.identity;

            void CopyCameraProperties(Camera src, Camera dest)
            {
                dest.clearFlags = src.clearFlags;
                dest.backgroundColor = src.backgroundColor;
                dest.farClipPlane = src.farClipPlane;
                dest.nearClipPlane = src.nearClipPlane;
                dest.orthographic = src.orthographic;
                dest.fieldOfView = src.fieldOfView;
                dest.aspect = src.aspect;
                dest.orthographicSize = src.orthographicSize;
            }
        }

        public void BlockVision(bool block) => visionBlocker.SetActive(block);
    }
}
