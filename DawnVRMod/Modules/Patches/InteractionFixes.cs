using UnityEngine;

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

        public static bool FollowCamUpdateInputVars(T_884A92DB __instance)
        {
            float num = __instance.m_camMomentumStep * Time.deltaTime;
            Vector3 axisVector = Vector3.zero;
            Vector3 axisVector2 = T_A6E913D1.Instance.m_inputManager.GetAxisVector3(eGameInput.kMovementXPositive, eGameInput.kNone, eGameInput.kMovementYPositive);
            if (__instance.m_invertX)
            {
                axisVector2.x *= -1f;
                axisVector.x *= -1f;
            }
            if (__instance.m_invertY)
            {
                axisVector2.z *= -1f;
                axisVector.z *= -1f;
            }
            __instance._15479B337 = 0f;
            __instance._15479B329 = 0f;
            if (axisVector != Vector3.zero)
            {
                __instance._15479B337 = -__instance._1D4E7496D(axisVector.x * __instance.m_OptionSensitivity, __instance.m_mouseDeadZone) * __instance.m_mouseHFactor;
                __instance._15479B329 = -__instance._1D4E7496D(axisVector.z * __instance.m_OptionSensitivity, __instance.m_mouseDeadZone) * __instance.m_mouseVFactor;
                if (__instance._15479B337 != 0f || __instance._15479B329 != 0f)
                {
                    __instance._122F739CA = true;
                    __instance._1C945E638 = (__instance._178F6876A = __instance.m_camMomentumMin);
                }
            }
            if (axisVector2 != Vector3.zero)
            {
                float num2 = __instance._1D4E7496D(axisVector2.x, __instance.m_deadZone);
                float num3 = __instance._1D4E7496D(axisVector2.z, __instance.m_deadZone);
                if (__instance._13BF9FFE2 && (__instance._1C945E638 == 0f || __instance._178F6876A == 0f))
                {
                    num3 *= __instance.m_collisionMomentumCompensator;
                    num2 *= __instance.m_collisionMomentumCompensator;
                }
                __instance._15479B337 += (-num2 * __instance.m_joystickHFactor + __instance._1C945E638) * __instance.m_OptionSensitivity;
                __instance._15479B329 += (-num3 * __instance.m_joystickVFactor + __instance._178F6876A) * __instance.m_OptionSensitivity;
                if (Mathf.Abs(num2) > 0.001f)
                {
                    if (num2 < 0f)
                    {
                        if (__instance._1A1B3DD19 > 0f)
                        {
                            __instance._15479B337 += __instance._1C945E638;
                            __instance._1C945E638 = __instance.m_camMomentumMin;
                        }
                        __instance._1C945E638 = Mathf.Min(__instance.m_camMomentumMax * (Mathf.Abs(num2) + __instance.m_deadZone), __instance._1C945E638 + num * -num2);
                    }
                    else
                    {
                        if (__instance._1A1B3DD19 < 0f)
                        {
                            __instance._15479B337 += __instance._1C945E638;
                            __instance._1C945E638 = __instance.m_camMomentumMin;
                        }
                        __instance._1C945E638 = Mathf.Max(-__instance.m_camMomentumMax * (Mathf.Abs(num2) + __instance.m_deadZone), __instance._1C945E638 - num * num2);
                    }
                    __instance._122F739CA = false;
                }
                else
                {
                    __instance._1C945E638 = __instance.m_camMomentumMin;
                }
                __instance._1A1B3DD19 = num2;
                if (Mathf.Abs(num3) > 0.001f)
                {
                    if (num3 < 0f)
                    {
                        if (__instance._11AD89447 > 0f)
                        {
                            __instance._15479B329 += __instance._178F6876A;
                            __instance._178F6876A = __instance.m_camMomentumMin;
                        }
                        __instance._178F6876A = Mathf.Min(__instance.m_camMomentumMax * (Mathf.Abs(num3) + __instance.m_deadZone), __instance._178F6876A + num * -num3);
                    }
                    else
                    {
                        if (__instance._11AD89447 < 0f)
                        {
                            __instance._15479B329 += __instance._178F6876A;
                            __instance._178F6876A = __instance.m_camMomentumMin;
                        }
                        __instance._178F6876A = Mathf.Max(-__instance.m_camMomentumMax * (Mathf.Abs(num3) + __instance.m_deadZone), __instance._178F6876A - num * num3);
                    }
                    __instance._122F739CA = false;
                }
                else
                {
                    __instance._178F6876A = __instance.m_camMomentumMin;
                }
                __instance._11AD89447 = num3;
            }
            if (!__instance._122F739CA)
            {
                if (__instance._15479B337 == 0f)
                {
                    __instance._15479B337 += __instance._1C945E638;
                    __instance._1C945E638 = __instance.m_camMomentumMin;
                }
                if (__instance._15479B329 == 0f)
                {
                    __instance._15479B329 += __instance._178F6876A;
                    __instance._178F6876A = __instance.m_camMomentumMin;
                }
                __instance._15479B337 *= Time.deltaTime;
                __instance._15479B329 *= Time.deltaTime;
            }
            __instance._15479B337 *= __instance._15D72179F.x;
            __instance._15479B329 *= __instance._15D72179F.y;
            if (__instance._15479B337 == 0f && __instance._15479B329 == 0f)
            {
                if (__instance.Character.IsMoving || __instance._1CDFD68D3)
                {
                    __instance.UpdateUserControlled();
                }
            }
            else
            {
                __instance.UpdateUserControlled();
            }
            __instance._18F22C9F8();

            return false;
        }

        public static void ChoiceButtonSelection(T_6876113C __instance, bool __result)
        {
            if (__result)
                MelonLoader.MelonCoroutines.Start(EnableInteraction());
        }

        private static System.Collections.IEnumerator EnableInteraction()
        {
            yield return new WaitForSeconds(1);
            if (!VR.VRRig.Instance.CutsceneHandler.IsCutsceneActive)
            {
                T_A6E913D1.Instance.m_followCamera.m_isInteractionBlocked = false;
                T_A6E913D1.Instance.m_gameModeManager.isDebug = false;
                T_F8FE3E1C.s_hideUI = false;
            }
        }
    }
}
