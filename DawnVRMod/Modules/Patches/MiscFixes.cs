using UnityEngine;
using DawnVR.Modules.VR;
using System.Collections.Generic;
#if REMASTER
using UnityEngine.PostProcessing;
#else
using GameMaster = T_A6E913D1;
using ST_ParallelHighlight = T_FD3AF1C2;
using MirrorReflection = T_55EA835B;
using DawnMainCamera = T_34182F31;
using CriAtomListener = T_165E4FE4;
using PostProcessingBehaviour = UnityEngine._1F1547F66.T_190FC323;
#endif


namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool ParallelHighlightTrigger(ST_ParallelHighlight __instance, bool __0)
        {
            // i can't replicate the issue so i'm just throwing a try catch onto it
            try
            {
                if (__instance.m_highlight != null)
                    __instance.m_highlight.UISelect(__0);

                if (__instance.m_send && GameMaster.Instance != null)
                {
                    var tagsFromPrefix = GameMaster.Instance.m_tagManager.GetTagsFromPrefix(__instance.m_channelPrefix);
                    for (int i = 0; i < tagsFromPrefix.Count; i++)
                    {
                        ST_ParallelHighlight component = tagsFromPrefix[i].GetComponent<ST_ParallelHighlight>();
                        if (component != null)
                            component.Trigger(__0);
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool MirrorReflectionAwake(MirrorReflection __instance)
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
            Camera lastMainCam = ObfuscationTools.GetPropertyValue<Camera>(null, "m_lastMainCamera", typeof(DawnMainCamera));
            Camera camera = null;

            // why does this method cause so many issues
            try
            {
                try { camera = Camera.main; } catch { }
                if (camera == null)
                {
                    if (lastMainCam != null)
                        camera = lastMainCam.gameObject.GetComponentInChildren<Camera>(true);
                }
                else if (lastMainCam != camera && camera != null)
                    ObfuscationTools.SetFieldValue(typeof(DawnMainCamera), "m_lastMainCamera", camera);
            }
            catch { }

            __result = camera;
            return false;
        }

        public static void DestroyAtomListener(CriAtomListener __instance)
        {
            if (__instance.name == "Main Camera")
                GameObject.Destroy(__instance);
        }

        public static void OnPPEnable(PostProcessingBehaviour __instance)
        {
            if (VRRig.Instance.Camera.GetComponent<VRPostProcessing>())
                return;

            __instance.enabled = false;
            var vpp = VRRig.Instance.Camera.gameObject.AddComponent<VRPostProcessing>();
            //vpp.profile = __instance.profile;
        }
    }
}
