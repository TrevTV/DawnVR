using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCutsceneHandler : MonoBehaviour
    {
        public bool IsCutsceneActive { get; private set; }

        public Camera CurrentCutsceneCamera => cutsceneCamera;
        public Transform AmuletGlassTransform => amuletGlassTransform;

        private Camera cutsceneCamera;
        private GameObject cutsceneRoom;
        private GameObject amuletCookieView;
        private Transform amuletGlassTransform;
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

                if (VRMain.CurrentSceneName == "E4_S03_Backyard")
                {
                    T_A7F99C25 window = T_E7B3064D.Singleton.GetWindow<T_A7F99C25>("OverlayCookie");
                    if (window.gameObject.activeInHierarchy)
                        amuletCookieView.SetActive(true);
                    else
                        amuletCookieView.SetActive(false);
                }
            }
        }

        public void SetupCutscene(bool enableAmulet = false)
        {
            if (IsCutsceneActive)
                return;

            CheckCutsceneRequirements();

            IsCutsceneActive = true;
            cutsceneRoom.SetActive(true);
            cutsceneCamera.enabled = true;
            if (enableAmulet)
                amuletCookieView.SetActive(true);

            VRRig.Instance.SetParent(null, new Vector3(0f, 1100f, 0f));
            cutsceneRoom.transform.eulerAngles = new Vector3(0f, VRRig.Instance.Camera.transform.eulerAngles.y, 0f);
        }

        public void EndCutscene()
        {
            IsCutsceneActive = false;
            if (cutsceneRoom != null)
                cutsceneRoom.SetActive(false);
            /*if (amuletCookieView != null)
                amuletCookieView.SetActive(false);*/
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
                amuletCookieView = cutsceneRoom.transform.Find("Amulet").gameObject;
                amuletGlassTransform = amuletCookieView.transform.Find("Offset/Glass");
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
