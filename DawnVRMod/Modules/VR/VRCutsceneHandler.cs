using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCutsceneHandler : MonoBehaviour
    {
        public bool IsCutsceneActive { get; private set; }
        public Camera CurrentCutsceneCamera => cutsceneCamera;

        private Camera cutsceneCamera;
        private GameObject cutsceneRoom;
        private RenderTexture cutsceneRenderTexture;

        private void Start()
        {
            cutsceneRenderTexture = new RenderTexture(1920, 1080, 16);
            cutsceneRenderTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void LateUpdate()
        {
            if (cutsceneCamera != null && cutsceneCamera.enabled)
            {
                cutsceneCamera.transform.position = T_34182F31.main.transform.position;
                cutsceneCamera.transform.rotation = T_34182F31.main.transform.rotation;
                cutsceneCamera.fieldOfView = T_34182F31.main.fieldOfView;
                cutsceneCamera.nearClipPlane = T_34182F31.main.nearClipPlane;
                cutsceneCamera.farClipPlane = T_34182F31.main.farClipPlane;
            }
        }

        public void SetupCutscene()
        {
            if (IsCutsceneActive)
                return;

            CheckCutsceneRequirements();

            IsCutsceneActive = true;
            cutsceneRoom.SetActive(true);
            cutsceneCamera.enabled = true;

            VRRig.Instance.SetParent(null, new Vector3(0f, 1100f, 0f));
        }

        public void EndCutscene()
        {
            IsCutsceneActive = false;
            if (cutsceneRoom != null)
                cutsceneRoom.SetActive(false);
            if (cutsceneCamera != null)
                cutsceneCamera.enabled = false;
        }

        private void CheckCutsceneRequirements()
        {
            if (cutsceneRoom == null)
            {
                cutsceneRoom = GameObject.Instantiate(Resources.CutsceneRoom);
                cutsceneRoom.transform.position = new Vector3(0f, 1100f, 0f);
                cutsceneRoom.transform.Find("Screen").GetComponent<MeshRenderer>().sharedMaterial.mainTexture = cutsceneRenderTexture;
            }

            if (cutsceneCamera == null)
            {
                GameObject camObj = new GameObject("VRCutsceneCamera");
                cutsceneCamera = camObj.AddComponent<Camera>();
                cutsceneCamera.depth = 100;
                cutsceneCamera.targetTexture = cutsceneRenderTexture;
            }
        }
    }
}
