using UnityEngine;
using Valve.VR;
#if !REMASTER
using UIDrawCall = T_632CCBA1;
using IMScene = T_E29491C9;
using GameMaster = T_A6E913D1;
using TutorialWindow = T_64B68373;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static void CreateDrawCallMat(UIDrawCall __instance)
        {
            if (__instance.mainTexture.name.Contains("Fonts"))
                //__instance.dynamicMaterial.shader = Resources.NGUIOverlayShader;
                return;
            else
                __instance.dynamicMaterial.shader = Resources.NGUIOverlayShader;
        }

        public static void FixFakeFogQueue(IMScene __instance)
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

        public static bool SetCameraCullingMask(Camera __0, int __1)
        {
            if (__0 != null)
                __0.cullingMask = __1;

            return false;
        }

        public static bool UpdateUIFade(float _13C05413A, Color _1A2D6C82C)
        {
            if (GameMaster.Instance != null && GameMaster.Instance.m_overrideBlackScreen)
                SteamVR_Fade.Start(Color.black, 0);
            else
            {
                _1A2D6C82C.a = _13C05413A;
                SteamVR_Fade.Start(_1A2D6C82C, 0);
            }
            return false;
        }

        public static bool SetTutorialInfo(TutorialWindow __instance, TutorialWindow.eCurrentLesson _1B1E89CA4)
        {
            if (_1B1E89CA4 == TutorialWindow.eCurrentLesson.kObjective)
            {
                // reflection is kinda ehh in this case but its the best way to get the method depending on game
                typeof(TutorialWindow).GetMethod(ObfuscationTools.GetRealMethodName("NextLesson")).Invoke(__instance, new object[] { TutorialWindow.eCurrentLesson.kCloseWindow });
                return false;
            }
            return true;
        }
    }
}
