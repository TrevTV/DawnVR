using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCamera : MonoBehaviour
    {
        public Camera Camera;

        private Camera uiCamera;
        private Transform uiRenderer;
        private RenderTexture renderTexture;
        private bool haveFollowCamFollowThis = true;

        private void Start()
        {
            Camera = GetComponent<Camera>();

            #region UI Renderer

            uiRenderer = transform.Find("UIRenderer");
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            renderTexture = new RenderTexture(1920, 1080, 1);
            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = renderTexture;
            uiCamera.targetTexture = renderTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material = mat;

            #endregion
        }

        private void OnDestroy()
        {
            Destroy(uiRenderer.gameObject);
        }

        private void LateUpdate()
        {
            if (haveFollowCamFollowThis)
            {
                if (T_A6E913D1.Instance.m_followCamera.enabled)
                    T_A6E913D1.Instance.m_followCamera.enabled = false;
                T_A6E913D1.Instance.m_followCamera.transform.position = transform.position;
                T_A6E913D1.Instance.m_followCamera.transform.rotation = transform.rotation;
            }
        }
    }
}
