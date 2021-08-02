using UnityEngine;

namespace DawnVR
{
    internal class VRCamera : MonoBehaviour
    {
        private Camera uiCamera;
        private Transform uiRenderer;
        private RenderTexture renderTexture;
        private bool shouldWeDoALittleTrolling = false;

        private void Start()
        {
            uiRenderer = transform.Find("UIRenderer");
            uiCamera = GameObject.Find("/UIRoot/Camera").GetComponent<Camera>();
            renderTexture = new RenderTexture(1920, 1080, 1);
            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = renderTexture;
            uiCamera.targetTexture = renderTexture;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0);
            uiRenderer.GetComponent<MeshRenderer>().material = mat;
        }

        private void OnDestroy()
        {
            Destroy(uiRenderer.gameObject);
        }

        private void LateUpdate()
        {
            if (shouldWeDoALittleTrolling)
            {
                if (T_A6E913D1.Instance.m_followCamera.enabled)
                    T_A6E913D1.Instance.m_followCamera.enabled = false;
                T_A6E913D1.Instance.m_followCamera.transform.position = transform.position;
                T_A6E913D1.Instance.m_followCamera.transform.rotation = transform.rotation;
            }
        }
    }
}
