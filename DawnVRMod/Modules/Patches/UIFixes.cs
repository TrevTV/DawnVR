using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool GetMainUICamera(ref Camera __result)
        {
            Camera camera = null;
            try { camera = Camera.main; } catch { }
            if (camera == null)
            {
                if (T_34182F31._1444D8BF3 != null)
                    camera = T_34182F31._1444D8BF3.gameObject?.GetComponentInChildren<Camera>(true);
            }
            else if (T_34182F31._1444D8BF3 != camera)
                T_34182F31._1444D8BF3 = camera;

            __result = camera;
            return false;
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
