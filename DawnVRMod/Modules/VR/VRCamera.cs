using Valve.VR;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
        public Camera Component;
        public Shader OverlayShader;
        public RenderTexture RenderToVRTexture;

        private Camera uiCamera;
        private Transform uiRenderer;
        private GameObject visionBlocker;
        private GameObject cutsceneBlocker;

        private void Start()
        {
            Component = GetComponent<Camera>();
            gameObject.AddComponent<SteamVR_Fade>();

            visionBlocker = transform.Find("VisionBlocker").gameObject;
            cutsceneBlocker = transform.Find("CutsceneVision").gameObject;
            cutsceneBlocker.GetComponent<MeshRenderer>().material.renderQueue = 5000;
            cutsceneBlocker.transform.localScale = new Vector3(2.5f, 3f, 3f);
            cutsceneBlocker.SetActive(false);

            Component.backgroundColor = Color.black;

            #region UI Renderer

            uiRenderer = transform.Find("UIRenderer");
            uiRenderer.localScale = new Vector3(0.25f, 0.15f, 0.15f);
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            RenderToVRTexture = new RenderTexture(Screen.width, Screen.height, 1);
            uiCamera.targetTexture = RenderToVRTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material.mainTexture = RenderToVRTexture;
            OverlayShader = uiRenderer.GetComponent<MeshRenderer>().material.shader;
            OverlayShader.hideFlags = HideFlags.DontUnloadUnusedAsset;

            #endregion
        }

        public void BlockVision(bool block) => visionBlocker.SetActive(block);

        public void CutsceneVision(bool enabled)
        {
            if (Preferences.EnableCutsceneBorder)
                cutsceneBlocker.SetActive(enabled);
        }
    }
}
