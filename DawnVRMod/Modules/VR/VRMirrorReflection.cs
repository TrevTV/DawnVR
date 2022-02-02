using UnityEngine;
using Valve.VR;
#if !REMASTER
using MirrorReflection = T_55EA835B;
using GameMaster = T_A6E913D1;
#endif

// heavily cleaned up version from https://forum.unity.com/threads/mirror-reflections-in-vr.416728/
namespace DawnVR.Modules.VR
{
    public class VRMirrorReflection : MonoBehaviour
    {
        private static bool CurrentlyRendering;

        public Camera camera;
        public int m_TextureSize = 256;
        public float m_ClipPlaneOffset = 0.07f;
        public int m_framesNeededToUpdate = 0;
        public GameObject[] m_objectsToDisable;

        public LayerMask m_ReflectLayers = -1;

        private int frameCounter = 0;
        private Renderer renderer;
        private Camera reflectionCamera;
        private RenderTexture leftReflectionTexture;
        private RenderTexture rightReflectionTexture;

        private void Start()
        {
            camera = VRRig.Instance.Camera.Component;
            renderer = GetComponent<Renderer>();
            SetupReflectionCamera();
            SetupRenderTexture(ref leftReflectionTexture);
            SetupRenderTexture(ref rightReflectionTexture);
        }

        private void OnWillRenderObject()
        {
            if (MirrorReflection.s_blockMirrorRenders || (GameMaster.Instance != null && GameMaster.Instance.m_levelManager.LoadInProgress))
                return;

            if (frameCounter > 0)
            {
                frameCounter--;
                return;
            }

            if (!enabled || !renderer || !renderer.sharedMaterial || !renderer.enabled)
                return;

            if (CurrentlyRendering)
                return;

            CurrentlyRendering = true;
            frameCounter = m_framesNeededToUpdate;

            if (VRRig.Instance.CutsceneHandler.IsActive)
                camera = VRRig.Instance.CutsceneHandler.CurrentCamera;
            else
                camera = VRRig.Instance.Camera.Component;

            RenderCamera(Camera.StereoscopicEye.Left, ref leftReflectionTexture);
            if (!VRRig.Instance.CutsceneHandler.IsActive || !VRRig.Instance.CutsceneHandler.IsIn2D)
                RenderCamera(Camera.StereoscopicEye.Right, ref rightReflectionTexture);
        }

        private void RenderCamera(Camera.StereoscopicEye eye, ref RenderTexture reflectionTexture)
        {
            Vector3 pos = transform.position;
            Vector3 normal = transform.up;

            float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);

            Vector3 oldEyePos;
            Matrix4x4 worldToCameraMatrix;
            if (!VRRig.Instance.CutsceneHandler.IsActive || !VRRig.Instance.CutsceneHandler.IsIn2D)
            {
                worldToCameraMatrix = camera.GetStereoViewMatrix(eye) * reflection;

                var eyeOffset = SteamVR.instance.eyes[(int)eye].pos;
                eyeOffset.z = 0.0f;
                oldEyePos = camera.transform.position + camera.transform.TransformVector(eyeOffset);
            }
            else
            {
                worldToCameraMatrix = camera.worldToCameraMatrix * reflection;
                oldEyePos = camera.transform.position;
            }

            Vector3 newEyePos = reflection.MultiplyPoint(oldEyePos);

            reflectionCamera.transform.position = newEyePos;
            reflectionCamera.worldToCameraMatrix = worldToCameraMatrix;

            Vector4 clipPlane = CameraSpacePlane(worldToCameraMatrix, pos, normal, 1.0f);
            Matrix4x4 projectionMatrix = VRRig.Instance.CutsceneHandler.IsActive && VRRig.Instance.CutsceneHandler.IsIn2D
                ? camera.projectionMatrix
                : HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix((EVREye)eye, camera.nearClipPlane, camera.farClipPlane));
            MakeProjectionMatrixOblique(ref projectionMatrix, clipPlane);

            reflectionCamera.projectionMatrix = projectionMatrix;
            reflectionCamera.targetTexture = reflectionTexture;

            GL.invertCulling = true;
            reflectionCamera.transform.rotation = camera.transform.rotation;
            reflectionCamera.Render();
            GL.invertCulling = false;

            Material[] materials = renderer.sharedMaterials;
            string property = "_ReflectionTex" + eye.ToString();
            foreach (Material mat in materials)
            {
                if (mat.HasProperty(property))
                    mat.SetTexture(property, reflectionTexture);
            }

            CurrentlyRendering = false;
        }

        private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
            Vector3 cpos = worldToCameraMatrix.MultiplyPoint(offsetPos);
            Vector3 cnormal = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        private void MakeProjectionMatrixOblique(ref Matrix4x4 matrix, Vector4 clipPlane)
        {
            Vector4 q;

            q.x = (sgn(clipPlane.x) + matrix[8]) / matrix[0];
            q.y = (sgn(clipPlane.y) + matrix[9]) / matrix[5];
            q.z = -1.0F;
            q.w = (1.0F + matrix[10]) / matrix[14];

            Vector4 c = clipPlane * (2.0F / Vector3.Dot(clipPlane, q));

            matrix[2] = c.x;
            matrix[6] = c.y;
            matrix[10] = c.z + 1.0F;
            matrix[14] = c.w;

            float sgn(float a)
            {
                if (a > 0.0f) return 1.0f;
                if (a < 0.0f) return -1.0f;
                return 0.0f;
            }
        }

        private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        private Matrix4x4 HMDMatrix4x4ToMatrix4x4(HmdMatrix44_t input)
        {
            var m = Matrix4x4.identity;

            m[0, 0] = input.m0;
            m[0, 1] = input.m1;
            m[0, 2] = input.m2;
            m[0, 3] = input.m3;

            m[1, 0] = input.m4;
            m[1, 1] = input.m5;
            m[1, 2] = input.m6;
            m[1, 3] = input.m7;

            m[2, 0] = input.m8;
            m[2, 1] = input.m9;
            m[2, 2] = input.m10;
            m[2, 3] = input.m11;

            m[3, 0] = input.m12;
            m[3, 1] = input.m13;
            m[3, 2] = input.m14;
            m[3, 3] = input.m15;

            return m;
        }

        private void SetupRenderTexture(ref RenderTexture rt)
        {
            rt = new RenderTexture(m_TextureSize, m_TextureSize, 16);
            rt.name = "MirrorReflection";
            rt.isPowerOfTwo = true;
            rt.hideFlags = HideFlags.DontSave;
            rt.antiAliasing = 2;
        }

        private void SetupReflectionCamera()
        {
            GameObject go = new GameObject("MirrorReflectionCamera");
            go.hideFlags = HideFlags.DontSave;
            go.transform.parent = transform;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            reflectionCamera = go.AddComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.cullingMask = (-17 & m_ReflectLayers.value & ~(1 << LayerMask.NameToLayer("EditorGizmo")) & ~(1 << LayerMask.NameToLayer("UI3D")) & ~(1 << LayerMask.NameToLayer("ObjectiveUI")));
            CopyCameraProperties(camera, reflectionCamera);

            void CopyCameraProperties(Camera src, Camera dest)
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
        }
    }
}