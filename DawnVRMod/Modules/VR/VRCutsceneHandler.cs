using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRCutsceneHandler : MonoBehaviour
    {
        public bool IsCutsceneActive { get; private set; }
        public Camera CurrentCutsceneCamera => cutsceneCamera;

        private Camera cutsceneCamera;
        private GameObject cutsceneRoom;
        //private RenderTexture cutsceneRenderTexture;
        private Cutscene3DHandler ThreeDHandler;

        private void Start()
        {
            //cutsceneRenderTexture = new RenderTexture(1920, 1080, 16);
            //cutsceneRenderTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        private void LateUpdate()
        {
            if (cutsceneCamera != null && IsCutsceneActive)
            {
                // todo: maybe take rotation from vr camera
                cutsceneCamera.transform.position = T_34182F31.main.transform.position;
                cutsceneCamera.transform.rotation = T_34182F31.main.transform.rotation;
                cutsceneCamera.fieldOfView = T_34182F31.main.fieldOfView;
                cutsceneCamera.nearClipPlane = T_34182F31.main.nearClipPlane;
                cutsceneCamera.farClipPlane = T_34182F31.main.farClipPlane;
                ThreeDHandler.CopyCameraProperties(cutsceneCamera, ThreeDHandler.reflectionCamera);
            }
        }

        public void SetupCutscene()
        {
            if (IsCutsceneActive)
                return;

            CheckCutsceneRequirements();

            IsCutsceneActive = true;
            cutsceneRoom.SetActive(true);
            //cutsceneCamera.enabled = true;

            VRRig.Instance.SetParent(null, new Vector3(0f, 1000f, 0f));
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
            if (cutsceneCamera == null)
            {
                GameObject camObj = new GameObject("VRCutsceneCamera");
                cutsceneCamera = camObj.AddComponent<Camera>();
                cutsceneCamera.enabled = false;
                //cutsceneCamera.depth = 100;
                //cutsceneCamera.targetTexture = cutsceneRenderTexture;
            }

            if (cutsceneRoom == null)
            {
                cutsceneRoom = GameObject.Instantiate(Resources.CutsceneRoom);
                cutsceneRoom.transform.position = new Vector3(0f, 1000f, 0f);
                //cutsceneRoom.transform.Find("Screen").GetComponent<MeshRenderer>().sharedMaterial.mainTexture = cutsceneRenderTexture;
                ThreeDHandler = cutsceneRoom.transform.Find("Screen").gameObject.AddComponent<Cutscene3DHandler>();
                ThreeDHandler.camera = cutsceneCamera;
                ThreeDHandler.leftReflectionTexture = (RenderTexture)ThreeDHandler.GetComponent<Renderer>().sharedMaterial.GetTexture("_LeftEyeTexture");
                ThreeDHandler.rightReflectionTexture = (RenderTexture)ThreeDHandler.GetComponent<Renderer>().sharedMaterial.GetTexture("_RightEyeTexture");
                ThreeDHandler.SetupReflectionCamera();
            }
        }
    }

    internal class Cutscene3DHandler : MonoBehaviour
    {
        public Camera camera;
        public RenderTexture leftReflectionTexture;
        public RenderTexture rightReflectionTexture;

        public Camera reflectionCamera;
        private Vector3 basePosition;
        private float eyeDistance;
        private bool isSetup;

        private void Start()
        {
            eyeDistance = Vector3.Distance(Valve.VR.SteamVR.instance.eyes[0].pos, Valve.VR.SteamVR.instance.eyes[1].pos);
        }

        public void SetupReflectionCamera()
        {
            GameObject go = new GameObject("ReflectionCamera");
            go.hideFlags = HideFlags.DontSave;
            go.transform.parent = camera.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            reflectionCamera = go.AddComponent<Camera>();
            reflectionCamera.enabled = false;
            basePosition = reflectionCamera.transform.localPosition;
            isSetup = true;
        }

        public void CopyCameraProperties(Camera src, Camera dest)
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

        private void OnWillRenderObject()
        {
            if (!isSetup) return;
            RenderCamera(Camera.StereoscopicEye.Left, ref leftReflectionTexture);
            RenderCamera(Camera.StereoscopicEye.Right, ref rightReflectionTexture);
        }

        private void RenderCamera(Camera.StereoscopicEye eye, ref RenderTexture reflectionTexture)
        {
            if (eye == Camera.StereoscopicEye.Left)
                reflectionCamera.transform.localPosition = basePosition - new Vector3(eyeDistance, 0f, 0f);
            else
                reflectionCamera.transform.localPosition = basePosition + new Vector3(eyeDistance, 0f, 0f);

            reflectionCamera.targetTexture = reflectionTexture;
            reflectionCamera.Render();
        }
    }
}
