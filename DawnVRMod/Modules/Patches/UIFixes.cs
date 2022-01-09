using UnityEngine;
using Valve.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void CreateDrawCallMat(T_632CCBA1 __instance)
        {
            if (__instance.mainTexture.name.Contains("Fonts"))
                //__instance.dynamicMaterial.shader = Resources.NGUIOverlayShader;
                return;
            else
                __instance.dynamicMaterial.shader = Resources.NGUIOverlayShader;
        }

        public static void FixFakeFogQueue(T_E29491C9 __instance)
        {
            Transform vignetteArea = __instance.transform.Find("VignetteArea");
            if (vignetteArea != null)
            {
                foreach (MeshRenderer renderer in vignetteArea.Find("OuterVolume").GetComponentsInChildren<MeshRenderer>())
                    renderer.material.renderQueue = 3000;
                foreach (MeshRenderer renderer in vignetteArea.Find("FreelookFacades").GetComponentsInChildren<MeshRenderer>())
                    renderer.material.renderQueue = 3000;
            }
        }

        public static bool SetCameraCullingMask(Camera _13A97A3A2, int _1A676C459)
        {
            if (_13A97A3A2 != null)
                _13A97A3A2.cullingMask = _1A676C459;

            return false;
        }

        public static bool UpdateUIFade(float _13C05413A, Color _1A2D6C82C)
        {
            if (T_A6E913D1.Instance != null && T_A6E913D1.Instance.m_overrideBlackScreen)
                SteamVR_Fade.Start(Color.black, 0);
            else
            {
                _1A2D6C82C.a = _13C05413A;
                SteamVR_Fade.Start(_1A2D6C82C, 0);
            }
            return false;
        }

        public static bool SetTutorialInfo(T_64B68373 __instance, T_64B68373.eCurrentLesson _1B1E89CA4)
        {
            if (_1B1E89CA4 == T_64B68373.eCurrentLesson.kObjective)
            {
                __instance._133003896(T_64B68373.eCurrentLesson.kCloseWindow); // NextLesson
                return false;
            }
            return true;
        }
    }
}
