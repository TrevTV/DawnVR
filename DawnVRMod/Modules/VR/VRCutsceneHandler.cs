using System.Linq;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCutsceneHandler : MonoBehaviour
    {
        public bool IsCutsceneActive { get; private set; }
        public bool IsIn2DCutsceneMode { get; private set; }

        public Camera CurrentCutsceneCamera => cutsceneCamera;
        public Transform AmuletGlassTransform => amuletGlassTransform;

        private Transform cutsceneCameraTransform;
        private Camera cutsceneCamera;
        private MeshRenderer cutsceneCameraUIRenderer;
        private Material mainMaterial;
        private GameObject cutsceneRoom;
        private MeshRenderer screenRend;
        private GameObject amuletCookieView;
        private Transform amuletGlassTransform;
        private Quaternion rotationBeforeCutscene;
        private RenderTexture cutsceneRenderTexture;

        private void Start()
        {
            cutsceneRenderTexture = new RenderTexture(1920, 1080, 16);
            cutsceneRenderTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void LateUpdate()
        {
            if (cutsceneCameraTransform != null && cutsceneCamera.enabled)
            {
                if (Preferences.Use2DCutsceneViewer.Value || IsIn2DCutsceneMode)
                {
                    cutsceneCameraTransform.position = T_34182F31.main.transform.position;
                    cutsceneCameraTransform.rotation = T_34182F31.main.transform.rotation;
                    cutsceneCamera.fieldOfView = T_34182F31.main.fieldOfView;
                }
                else
                {
                    cutsceneCameraTransform.position = T_34182F31.main.transform.position - new Vector3(0f, cutsceneCamera.transform.localPosition.y, 0f);
                    cutsceneCameraTransform.rotation = T_34182F31.main.transform.rotation;
                }

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
            CheckCutsceneRequirements();

            // not the greatest but it does function
            if (IsCutsceneActive && !screenRend.sharedMaterial.name.Contains("CriMana"))
                return;

            screenRend.sharedMaterial = mainMaterial;

            IsCutsceneActive = true;
            cutsceneRoom.SetActive(true);
            cutsceneCamera.enabled = true;
            if (enableAmulet)
                amuletCookieView.SetActive(true);

            rotationBeforeCutscene = VRRig.Instance.transform.rotation;
            VRRig.Instance.Camera.ResetHolderPosition();
            VRRig.Instance.SetParent(null, new Vector3(0f, 1100f, 0f));
            cutsceneRoom.transform.eulerAngles = new Vector3(0f, VRRig.Instance.Camera.transform.eulerAngles.y, 0f);
        }

        public void SetupCutscene(Material screenMat)
        {
            if (IsCutsceneActive)
                return;

            CheckCutsceneRequirements();
            screenRend.sharedMaterial = screenMat;

            IsCutsceneActive = true;
            cutsceneRoom.SetActive(true);

            rotationBeforeCutscene = VRRig.Instance.transform.rotation;
            VRRig.Instance.Camera.ResetHolderPosition();
            VRRig.Instance.SetParent(null, new Vector3(0f, 1100f, 0f));
            cutsceneRoom.transform.eulerAngles = new Vector3(0f, VRRig.Instance.Camera.transform.eulerAngles.y, 0f);
        }

        public void EndCutscene()
        {
            IsCutsceneActive = false;
            if (cutsceneRoom != null)
                cutsceneRoom.SetActive(false);
            if (cutsceneCamera != null)
                cutsceneCamera.enabled = false;
            if (cutsceneCameraUIRenderer != null)
                cutsceneCameraUIRenderer.enabled = false;

            VRRig.Instance.transform.rotation = rotationBeforeCutscene;
        }

        private void CheckCutsceneRequirements()
        {
            if (cutsceneRoom == null)
            {
                cutsceneRoom = GameObject.Instantiate(Resources.CutsceneRoom);
                cutsceneRoom.transform.position = new Vector3(0f, 1100f, 0f);
                screenRend = cutsceneRoom.transform.Find("Screen").GetComponent<MeshRenderer>();
                mainMaterial = screenRend.sharedMaterial;
                mainMaterial.mainTexture = cutsceneRenderTexture;
                amuletCookieView = cutsceneRoom.transform.Find("Amulet").gameObject;
                amuletGlassTransform = amuletCookieView.transform.Find("Offset/Glass");
            }

            if (cutsceneCameraTransform == null)
            {
                cutsceneCameraTransform = new GameObject("VRCutsceneCameraHolder").transform;
                GameObject vrCamObj = new GameObject("VRCutsceneCamera");
                vrCamObj.transform.parent = cutsceneCameraTransform;
                cutsceneCamera = vrCamObj.AddComponent<Camera>();
                cutsceneCamera.depth = 100;
                if (Preferences.Use2DCutsceneViewer.Value)
                    cutsceneCamera.targetTexture = cutsceneRenderTexture;
                else
                {
                    Transform originalUIRendTransform = VRRig.Instance.Camera.transform.Find("UIRenderer");
                    cutsceneCameraUIRenderer = GameObject.Instantiate(originalUIRendTransform.gameObject).GetComponent<MeshRenderer>();
                    cutsceneCameraUIRenderer.transform.parent = cutsceneCamera.transform;
                    cutsceneCameraUIRenderer.transform.localPosition = originalUIRendTransform.localPosition;
                }
            }

            if (!Preferences.Use2DCutsceneViewer.Value)
            {
                if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kCustomization
                    || T_A6E913D1.Instance.m_dawnUI.currentViewCookie != T_A7F99C25.eCookieChoices.kNone)
                {
                    cutsceneCamera.targetTexture = cutsceneRenderTexture;
                    cutsceneCamera.stereoTargetEye = StereoTargetEyeMask.None;
                    cutsceneCameraUIRenderer.enabled = false;
                    IsIn2DCutsceneMode = true;
                }
                else
                {
                    cutsceneCamera.targetTexture = null;
                    cutsceneCamera.stereoTargetEye = StereoTargetEyeMask.Both;
                    cutsceneCameraUIRenderer.enabled = true;
                    IsIn2DCutsceneMode = false;
                }
            }
        }
    }
}
