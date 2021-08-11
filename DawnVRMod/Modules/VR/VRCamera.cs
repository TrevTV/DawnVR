using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
        public Camera Component;
        public RenderTexture RenderToVRTexture;

        private Camera uiCamera;
        private Transform uiRenderer;
        private bool haveFollowCamFollowThis = true;

        private void Start()
        {
            Component = GetComponent<Camera>();

            #region UI Renderer

            uiRenderer = transform.Find("UIRenderer");
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            RenderToVRTexture = new RenderTexture(1920, 1080, 1);
            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = RenderToVRTexture;
            uiCamera.targetTexture = RenderToVRTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material = mat;

            #endregion
        }

        private void OnDestroy()
        {
            Destroy(uiRenderer.gameObject);
        }

        private System.Reflection.PropertyInfo DawnUICamera = typeof(T_C3DD66D9).Assembly.GetType("T_34182F31").GetProperty("main");

        private void LateUpdate()
        {
            if (haveFollowCamFollowThis)
            {
                /*if (T_A6E913D1.Instance.m_followCamera.enabled)
                    T_A6E913D1.Instance.m_followCamera.enabled = false;
                T_A6E913D1.Instance.m_followCamera.transform.position = transform.position;
                T_A6E913D1.Instance.m_followCamera.transform.rotation = transform.rotation;
                Camera camValue = (Camera)DawnUICamera.GetValue(null, null);
                if (camValue != null)
                {
                    camValue.transform.position = transform.position;
                    camValue.transform.rotation = transform.rotation;
                }*/
            }
        }
    }
}
