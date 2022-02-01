using UnityEngine;
using UnityEngine._1F1547F66;
using DawnVR.Modules.VR;
using System.Collections.Generic;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool ParallelHighlightTrigger(T_FD3AF1C2 __instance, bool _186835D38)
        {
            // i can't replicate the issue so i'm just throwing a try catch onto it
            try
            {
                if (__instance.m_highlight != null)
                    __instance.m_highlight.UISelect(_186835D38);

                if (__instance.m_send && T_A6E913D1.Instance != null)
                {
                    List<T_E6DAC959> tagsFromPrefix = T_A6E913D1.Instance.m_tagManager.GetTagsFromPrefix(__instance.m_channelPrefix);
                    for (int i = 0; i < tagsFromPrefix.Count; i++)
                    {
                        T_FD3AF1C2 component = tagsFromPrefix[i].GetComponent<T_FD3AF1C2>();
                        if (component != null)
                            component.Trigger(_186835D38);
                    }
                }
            }
            catch { }

            return false;
        }

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
