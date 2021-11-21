using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCutsceneHandler : MonoBehaviour
    {
        private bool cutsceneRunning;
        private Transform cameraLock;
        private Camera headsetCamera;
        private RenderTexture cutsceneRenderTexture;

        private void Start()
        {
            cutsceneRenderTexture = new RenderTexture(1920, 1080, 16);
            cutsceneRenderTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void LateUpdate()
        {
            if (headsetCamera != null && headsetCamera.enabled)
            {
                cameraLock.localPosition = (headsetCamera.transform.localPosition * -1) + T_34182F31.main.transform.position;
                cameraLock.localRotation = Quaternion.Euler(0f, T_34182F31.main.transform.localRotation.y, 0f);
                headsetCamera.fieldOfView = T_34182F31.main.fieldOfView;
                headsetCamera.nearClipPlane = T_34182F31.main.nearClipPlane;
                headsetCamera.farClipPlane = T_34182F31.main.farClipPlane;
            }
        }

        public void SetupCutscene()
        {
            if (cutsceneRunning)
                return;

            CheckCutsceneRequirements();

            cutsceneRunning = true;
            headsetCamera.enabled = true;

            VRRig.Instance.Camera.ReparentCutsceneVision(headsetCamera.transform, new Vector3(0, 0, 1.25f), new Vector3(-90f, -180f, 90));
            VRRig.Instance.Camera.CutsceneVision(true);
            VRRig.Instance.Camera.gameObject.SetActive(false);
        }

        public void EndCutscene()
        {
            cutsceneRunning = false;
            if (headsetCamera != null)
                headsetCamera.enabled = false;

            VRRig.Instance.Camera.gameObject.SetActive(true);
            VRRig.Instance.Camera.CutsceneVision(false);
        }

        private void CheckCutsceneRequirements()
        {
            if (cameraLock == null)
                cameraLock = new GameObject("CameraLock").transform;

            if (headsetCamera == null)
            {
                GameObject camObj = new GameObject("VRCutsceneCamera");
                headsetCamera = camObj.AddComponent<Camera>();
                headsetCamera.depth = 100;
                headsetCamera.transform.parent = cameraLock;
            }
        }
    }
}
