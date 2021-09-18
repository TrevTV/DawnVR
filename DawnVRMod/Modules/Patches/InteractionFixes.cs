using UnityEngine;
using MelonLoader;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool BoundaryStart(T_3BE79CFB __instance)
        {
            __instance.GetComponent<Collider>().isTrigger = false;
            return false;
        }

        public static void FollowCamStart(T_884A92DB __instance) => __instance.enabled = false;

        public static bool FollowCamLateUpdate(T_884A92DB __instance)
        {
            if (__instance.m_haltTransitionPreemtively || (__instance.isFreeroamStart && (__instance.m_cameraStartTransitionTime == 0f || __instance.m_cameraStartMaxTransitionSpeed == 0f)))
            {
                __instance.isFreeroamStart = false;
                if (__instance.m_camera != null)
                {
                    __instance._1B2CE6DA7(1f);
                    __instance.m_camera.useDepthOfField = false;
                    __instance.m_camera.ApplyValues(T_34182F31.main);
                }
                __instance.m_haltTransitionPreemtively = false;
            }
            if (__instance.Character == null) return false;

            __instance._13A040863 = __instance.Character.transform.position;
            if (__instance.m_isCameraDriver && __instance.m_camera != null)
            {
                if (__instance.isFreeroamStart)
                {
                    if (!__instance.m_isInteractionBlocked && __instance.m_startTransitionLerp == 0f)
                    {
                        __instance.specificTween = 0;
                        __instance._1A6D21309 = __instance.m_camera.localPosition;
                        __instance._1CD4518D7 = __instance.m_camera.localRotation;
                        if (__instance.m_camera.useDepthOfField)
                            __instance.m_initialStartDOFAperture = __instance.m_camera.aperture;
                        else
                            __instance.m_initialStartDOFAperture = 0f;
                        __instance._1C5B95284 = Vector3.Distance(__instance._1A6D21309, __instance._1B0A2CC1E);
                    }

                    __instance.m_startTransitionLerp = Mathf.SmoothDamp(__instance.m_startTransitionLerp, 1f, ref __instance._18EAA3360, __instance.m_cameraStartTransitionTime / 5f, float.PositiveInfinity, __instance._19FA60FD3());
                    __instance._1A6D21309 = Vector3.SmoothDamp(__instance._1A6D21309, __instance._1B0A2CC1E, ref __instance.m_cameraTransitionVel, __instance.m_cameraStartTransitionTime / 5f, __instance.m_cameraStartMaxTransitionSpeed, __instance._19FA60FD3());
                    if (__instance._1C5B95284 > 0.01f)
                        __instance.currentTransitionLerp = Mathf.Min(__instance.m_startTransitionLerp, 1f - (__instance._1A6D21309 - __instance._1B0A2CC1E).magnitude / __instance._1C5B95284);
                    else
                        __instance.currentTransitionLerp = __instance.m_startTransitionLerp;
                    __instance._1B2CE6DA7(__instance.currentTransitionLerp);
                    __instance.m_camera.ApplyValues(T_34182F31.main);
                    T_34182F31.main.transform.position = __instance._1A6D21309;
                    T_34182F31.main.transform.rotation = Quaternion.Slerp(__instance._1CD4518D7, __instance._13B9C8A95, __instance.currentTransitionLerp);
                    if (__instance.currentTransitionLerp - 0.999f >= 0f)
                    {
                        __instance.isFreeroamStart = false;
                        __instance.m_freeroamStartTransitionOverride = false;
                        __instance.m_isInteractionBlocked = false;
                        __instance.m_camera.focalLength = __instance._16DBB8DE;
                        __instance.m_camera.useDepthOfField = false;
                        __instance.m_startTransitionLerp = 0f;
                        __instance.currentTransitionLerp = 0f;
                        __instance.m_cameraStartTransitionTime = 0f;
                        __instance.m_cameraStartMaxTransitionSpeed = 500f;
                    }
                }
                else if (__instance.specificTween > 0)
                {
                    __instance.m_exitTweenLerp = Mathf.SmoothDamp(__instance.m_exitTweenLerp, 1f, ref __instance._18EAA3360, __instance.m_exitTweenTime / 5f, float.PositiveInfinity, __instance._19FA60FD3());
                    Quaternion rotation = T_34182F31.main.transform.rotation;
                    if (__instance.specificTween == 1)
                        rotation = Quaternion.Slerp(__instance.m_exitTweenStartOrientation, __instance.m_exitTweenGoalOrientation, __instance.m_exitTweenLerp);
                    else if (__instance.specificTween == 2)
                        rotation = Quaternion.Slerp(__instance.m_exitTweenGoalOrientation, Quaternion.identity, __instance.m_exitTweenLerp);
                    if (__instance.m_exitTweenLerp - 0.999 >= 0.0)
                    {
                        __instance.m_exitTweenLerp = 0f;
                        if (__instance.specificTween == 1)
                            __instance.specificTween = -1;
                        else
                            __instance.specificTween = 0;
                        if (__instance._1CFF88EAF != null)
                        {
                            __instance._1CFF88EAF();
                            __instance._1CFF88EAF = null;
                        }
                    }
                    __instance.m_camera.ApplyValues(T_34182F31.main);
                    T_34182F31.main.transform.position = __instance.transform.position;
                    T_34182F31.main.transform.up = Vector3.up;
                    T_34182F31.main.transform.forward = rotation * __instance.transform.forward;
                }
                else if (__instance.specificTween == -1)
                {
                    __instance.m_camera.ApplyValues(T_34182F31.main);
                    T_34182F31.main.transform.position = __instance.transform.position;
                    T_34182F31.main.transform.up = Vector3.up;
                    T_34182F31.main.transform.forward = __instance.m_exitTweenGoalOrientation * __instance.transform.forward;
                }
                else
                {
                    __instance.m_cameraStartTransitionTime = 0f;
                    __instance.m_cameraStartMaxTransitionSpeed = 500f;
                    __instance.m_cameraUseDefaultStartTransition = false;
                    T_34182F31.main.transform.position = __instance.transform.position;
                    T_34182F31.main.transform.rotation = __instance.transform.rotation;
                }
            }
            else if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam || __instance.m_isCustomizationFreeLook)
            {
                T_34182F31.main.nearClipPlane = 0.01f;
                T_34182F31.main.transform.position = __instance.transform.position;
                T_34182F31.main.transform.rotation = __instance.transform.rotation;
            }
            return false;
        }
    }
}
