using Valve.VR;
using UnityEngine;

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

        private Camera spectatorCamera;

        private float brightnessIntensity;
        private Material overlayEffectMaterial;

        private void Start()
        {
            Component = GetComponent<Camera>();
            gameObject.AddComponent<SteamVR_Fade>();

            Component.hdr = true;

            Holder = transform.parent;
            visionBlocker = transform.Find("VisionBlocker").gameObject;
            visionBlocker.SetActive(true);

            Component.backgroundColor = Color.black;
            overlayEffectMaterial = new Material(Resources.StandardAssetsOverlayShader);
            brightnessIntensity = T_1005C221.T_429306B8.GetBrightness();

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

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Vector4 UV_Transform = new Vector4(1, 0, 0, 1);
            overlayEffectMaterial.SetVector("_UV_Transform", UV_Transform);
            overlayEffectMaterial.SetFloat("_Intensity", brightnessIntensity - 1);
            overlayEffectMaterial.SetTexture("_Overlay", Resources.WhitePixelTexture);
            Graphics.Blit(source, destination, overlayEffectMaterial, 3);
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
            spectatorCamera.hdr = true;
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

        public void UpdateBrightness() => brightnessIntensity = T_1005C221.T_429306B8.GetBrightness();

        public void BlockVision(bool block) => visionBlocker.SetActive(block);

        public void ResetHolderPosition() => Holder.transform.localPosition = Vector3.zero;
    }
}
