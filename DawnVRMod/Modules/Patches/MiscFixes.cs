using UnityEngine;
using UnityEngine._1F1547F66;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool MirrorReflectionAwake(T_55EA835B __instance)
        {
            __instance.enabled = false;
            __instance.GetComponent<MeshRenderer>().sharedMaterial.shader = Resources.MirrorShader;
            VRMirrorReflection reflection = __instance.gameObject.AddComponent<VRMirrorReflection>();
            __instance.gameObject.layer = 16; // sets layer to "UI3D" to cull it from other mirrors
            reflection.m_TextureSize = __instance.m_TextureSize;
            reflection.m_ClipPlaneOffset = __instance.m_ClipPlaneOffset;
            reflection.m_ReflectLayers = __instance.m_ReflectLayers;
            return false;
        }

        public static bool GetMainUICamera(ref Camera __result)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                if (T_34182F31._1444D8BF3 != null)
                    camera = T_34182F31._1444D8BF3.gameObject.GetComponentInChildren<Camera>(true);
            }
            else if (T_34182F31._1444D8BF3 != camera)
                T_34182F31._1444D8BF3 = camera;

            __result = camera;
            return false;
        }

        public static void DestroyAtomListener(T_165E4FE4 __instance)
        {
            if (__instance.name == "Main Camera")
                GameObject.Destroy(__instance);
        }

        public static void OnPPEnable(T_190FC323 __instance)
        {
            if (VRRig.Instance.Camera.GetComponent<VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = VRRig.Instance.Camera.gameObject.AddComponent<VRPostProcessing>();
            vpp.profile = __instance.profile;
        }
    }
}
