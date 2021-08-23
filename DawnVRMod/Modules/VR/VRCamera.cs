using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
        public Camera Component;
        public RenderTexture RenderToVRTexture;

        private Camera uiCamera;
        private Transform uiRenderer;

        private void Start()
        {
            Component = GetComponent<Camera>();

            #region UI Renderer

            uiRenderer = transform.Find("UIRenderer");
            uiRenderer.localScale = new Vector3(0.25f, 0.15f, 0.15f);
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            RenderToVRTexture = new RenderTexture(1920, 1080, 1);
            uiCamera.targetTexture = RenderToVRTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material.mainTexture = RenderToVRTexture;

            #endregion
        }
    }
}
