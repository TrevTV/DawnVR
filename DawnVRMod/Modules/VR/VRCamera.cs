using Valve.VR;
using UnityEngine;
using System.Collections;
#if REMASTER
using BrightnessSettingsManager = DawnSettingsManager.BrightnessSettingsManager;
#else
using DawnSettingsManager = T_1005C221;
using BrightnessSettingsManager = T_1005C221.T_429306B8;
#endif

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
#if REMASTER
        public VRCamera(System.IntPtr ptr) : base(ptr) { }
#endif

        public Camera Component;
        public Shader OverlayShader;
        public RenderTexture RenderToVRTexture;
        public Transform Holder;

        private Camera uiCamera;
        private GameObject uiRenderer;
        private GameObject visionBlocker;
        private Material vignetteMat;
        private GameObject vignetteGo;

        private Camera spectatorCamera;

        private float brightnessIntensity;
        private Material overlayEffectMaterial;

        private void Start()
        {
            Component = GetComponent<Camera>();
            gameObject.AddComponent<SteamVR_Fade>();

#if REMASTER
            Component.allowHDR = true;
#else
            Component.hdr = true;
#endif

            Component.ResetAspect();

            Holder = transform.parent;
            visionBlocker = transform.Find("VisionBlocker").gameObject;
            visionBlocker.SetActive(true);
            vignetteGo = transform.Find("InvertedSphere").gameObject;
            vignetteMat = transform.Find("InvertedSphere").GetComponent<MeshRenderer>().material;
            //vignetteMat.SetFloat("_Inner", Preferences.VignetteInner.Value);
            //vignetteMat.SetFloat("_Outter", Preferences.VignetteOuter.Value);
            //vignetteGo.SetActive(false);
            //vignetteGo.transform.localScale = Vector3.zero;

            Component.backgroundColor = Color.black;
            overlayEffectMaterial = new Material(Resources.StandardAssetsOverlayShader);
            overlayEffectMaterial.hideFlags = HideFlags.DontUnloadUnusedAsset;
            brightnessIntensity = BrightnessSettingsManager.GetBrightness();

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

        private void LateUpdate()
        {
            if (VRRig.Instance.ChloeComponent == null)
                return;

            var moveDir = VRRig.Instance.ChloeComponent.m_moveDirection;
            if (moveDir != Vector3.zero)
                LerpVignette(true);
            else
                LerpVignette(false);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Vector4 UV_Transform = new Vector4(1, 0, 0, 1);
            overlayEffectMaterial.SetVector("_UV_Transform", UV_Transform);
            overlayEffectMaterial.SetFloat("_Intensity", brightnessIntensity);
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
#if REMASTER
            spectatorCamera.allowHDR = true;
#else
            spectatorCamera.hdr = true;
#endif
            spectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;
            spectatorCamera.fieldOfView = Preferences.SpectatorFOV.Value;
            specTransform.localPosition = Vector3.zero;
            specTransform.localRotation = Quaternion.identity;
            spectatorCamera.ResetAspect();

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

        public void ToggleUIRendererParent(bool parentToHolder)
        {
            Transform uiRendTrans = uiRenderer.transform;
            if (parentToHolder)
            {
                uiRendTrans.parent = VRRig.Instance.Camera.Holder;
                uiRendTrans.localPosition = new Vector3(0f, 1.5f, 1.5f);
                uiRendTrans.localRotation = Quaternion.Euler(-270f, -90f, 90);
            }
            else
            {
                uiRendTrans.parent = transform;
                uiRendTrans.localPosition = new Vector3(0f, 0f, 2.25f);
                uiRendTrans.localRotation = Quaternion.Euler(-270f, -90f, 90);
            }
        }

        private Coroutine lerpCoroute;
        private bool vigState;

        public void LerpVignette(bool enable)
        {
            if (vigState != enable)
            {
                if (lerpCoroute != null)
                    MelonLoader.MelonCoroutines.Stop(lerpCoroute);

                //lerpCoroute = (Coroutine)MelonLoader.MelonCoroutines.Start(enable ? CoLerp(0, 1, 0.3f) : CoLerp(1, 0, 0.3f));
                if (!enable || Preferences.VignetteSmoothEnable.Value)
                    lerpCoroute = (Coroutine)MelonLoader.MelonCoroutines.Start(
                    enable
                    ? CoLerp(Preferences.VignetteInner.Value, Preferences.VignetteOuter.Value, 0.3f)
                    : CoLerp(0f, 0f, 0.3f));
                else
                {
                    vignetteMat.SetFloat("_Inner", Preferences.VignetteInner.Value);
                    vignetteMat.SetFloat("_Outter", Preferences.VignetteOuter.Value);
                }

                vigState = enable;
            }

            IEnumerator CoLerp(float endInnerVal, float endOuterVal, float duration)
            {
                float innerStartValue = !enable ? Preferences.VignetteInner.Value : 0f;
                float outerStartValue = !enable ? Preferences.VignetteOuter.Value : 0f;

                float timeElapsed = 0;
                while (timeElapsed < duration)
                {
                    vignetteMat.SetFloat("_Inner", Mathf.Lerp(innerStartValue, endInnerVal, timeElapsed / duration));
                    vignetteMat.SetFloat("_Outter", Mathf.Lerp(outerStartValue, endOuterVal, timeElapsed / duration));
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                vignetteMat.SetFloat("_Inner", endInnerVal);
                vignetteMat.SetFloat("_Outter", endOuterVal);
            }
        }

        public void UpdateBrightness() => brightnessIntensity = BrightnessSettingsManager.GetBrightness();

        public void BlockVision(bool block) => visionBlocker.SetActive(block);

        public void ResetHolderPosition() => Holder.transform.localPosition = Vector3.zero;
    }
}
