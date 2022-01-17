using UnityEngine;
using DawnVR.Modules.VR;
using System.Collections.Generic;
using _15C6DD6D9._14FDF87F1;
using _169E4A3E._165972D5F;
using System;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static bool hasDisabledMovieWindow;

        public static void DisableSettingCurrentViewCookie(T_A7F99C25.eCookieChoices value)
        {
            foreach (var rend in GameObject.Find("/UIRoot/Camera/OverlayCookie/").GetComponentsInChildren<T_D186D1CC>())
                rend.enabled = false;

            SetCurrentViewCookie(value);
        }

        public static void SetCurrentViewCookie(T_A7F99C25.eCookieChoices cookie)
        {
            switch (cookie)
            {
                case T_A7F99C25.eCookieChoices.kNone:
                    VRRig.Instance.CutsceneHandler.EndCutscene();
                    if (VRRig.Instance?.ChloeComponent?.Camera != null)
                        VRRig.Instance.ChloeComponent.Camera.enabled = false;
                    break;
                case T_A7F99C25.eCookieChoices.kBinoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case T_A7F99C25.eCookieChoices.kE3Binoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case T_A7F99C25.eCookieChoices.kE4Binoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene(true);
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
            }
        }

        public static void TelescopeRotate(T_ADD17E7F __instance)
            => __instance.m_bottle = VRRig.Instance.CutsceneHandler.AmuletGlassTransform.gameObject;

        public static bool TelescopePuzzleUpdate(T_24E8F007 __instance)
        {
            if (__instance._1A0F64CF1)
            {
                if (__instance._1372031B.activeInHierarchy)
                {
                    Vector3 localEulerAngles = VRRig.Instance.CutsceneHandler.AmuletGlassTransform.localEulerAngles;
                    float num = __instance.m_targetZRotation + __instance.m_errorRange + 1f;
                    if (localEulerAngles.z >= 180f)
                        num = Mathf.Abs(localEulerAngles.z - 360f - __instance.m_targetZRotation);
                    else
                        num = Mathf.Abs(localEulerAngles.z - __instance.m_targetZRotation);

                    GameStateModel currentModel = T_A6E913D1.Instance.m_gameStateManager.GetCurrentModel();
                    if (currentModel != null)
                        currentModel.SetValue(__instance.m_lensAngleVariable, (int)Mathf.Floor(num), false);

                    num -= __instance.m_errorRange;
                    if (num <= 0f)
                    {
                        Transform camTrans = VRRig.Instance.CutsceneHandler.CurrentCutsceneCamera.transform;
                        __instance._13E72D93B = new Ray(camTrans.position, camTrans.forward);
                        RaycastHit[] array = Physics.RaycastAll(__instance._13E72D93B, __instance.m_maxRaycastDistance);
                        bool flag = false;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].transform.gameObject.GetComponent<T_24E8F007>() != null)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            __instance._1DB0E3B9C = 0f;
                            return false;
                        }
                        __instance._1DB0E3B9C += Time.deltaTime;
                        if (__instance._1DB0E3B9C >= __instance.m_hoverTime)
                        {
                            List<T_7808CA07> outputConnections = T_A6E913D1.Instance.m_graphManager.CurrentGraphNode.outputConnections;
                            if (outputConnections != null && outputConnections.Count != 0)
                            {
                                T_F45060BF t_F45060BF = null;
                                for (int j = 0; j < outputConnections.Count; j++)
                                {
                                    T_F45060BF t_F45060BF2 = outputConnections[j].to as T_F45060BF;
                                    if (t_F45060BF2 != null)
                                    {
                                        string text = (t_F45060BF2.sequence == null) ? string.Empty : t_F45060BF2.sequence.nodeName;
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            if (string.Equals(text, __instance.m_nodeName, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                t_F45060BF = t_F45060BF2;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (t_F45060BF == null)
                                    return false;

                                T_BBB6DDD9 t_BBB6DDD = T_A6E913D1.Instance.m_graphManager.CurrentGraphNode as T_BBB6DDD9;
                                if (t_BBB6DDD != null)
                                {
                                    t_BBB6DDD.m_exitGraphObject = t_F45060BF;
                                    T_A6E913D1.Instance.m_graphManager.ExitCurrentNode();
                                }
                            }
                        }
                    }
                }
            }
            else
                __instance._1B350D183();

            return false;
        }

        public static bool SetupFollowCameraMatrix(T_884A92DB __instance, Vector4 _1DD947C88, Vector4 _15E19D274)
        {
            if (__instance.m_isLineLocked)
            {
                if (__instance._19186BCE3)
                {
                    __instance._15479B337 *= -1f;
                }
                if (__instance._12E8B1EAF)
                {
                    __instance._15479B329 *= 0f;
                }
                if (__instance._15479B337 >= 0f && __instance._15479B329 >= 0f)
                {
                    __instance._1C0908541 -= Mathf.Max(__instance._15479B337, __instance._15479B329);
                }
                else if (__instance._15479B337 <= 0f && __instance._15479B329 <= 0f)
                {
                    __instance._1C0908541 -= Mathf.Min(__instance._15479B337, __instance._15479B329);
                }
                __instance._1C0908541 = Mathf.Clamp01(__instance._1C0908541);
                _1DD947C88 = Vector3.Lerp(__instance._18166372C, __instance._1854BFA4C, __instance._1C0908541);
                //_1DD947C88 += __instance._111890643;
                __instance._111890643 -= __instance._111890643 * ((Mathf.Abs(__instance._15479B337) + Mathf.Abs(__instance._15479B329)) * Time.deltaTime / 2f);
            }
            else if (__instance.IsLocked)
            {
                float num = -__instance._15479B337 * 57.29578f;
                float num2 = __instance._15479B329 * 57.29578f;
                __instance._1E9DAA452 += num;
                __instance._1CFFF5F80 += num2;
                ExtendedViewBase.GrabExtendedView(T_34182F31.main, ref __instance._1D9C76B26, ref __instance._1C89EAB96);
                if (__instance._1C89EAB96 != null && T_1005C221.T_A9DD5E3E.IsExtendedViewAvailable())
                {
                    float num3 = T_34182F31.main.fieldOfView / 40f;
                    float num4 = __instance._1C89EAB96.Yaw * num3;
                    float num5 = __instance._1C89EAB96.Pitch * num3;
                    __instance._1E9DAA452 += num4 - __instance._1DD2696DE;
                    __instance._1CFFF5F80 += num5 - __instance._1CE82F9BB;
                    __instance._1DD2696DE = num4;
                    __instance._1CE82F9BB = num5;
                }
                Transform transform = __instance._11A7A7D26.transform;
                if (__instance._1552E3BEF != Vector3.zero)
                {
                    transform.forward = __instance._1552E3BEF;
                    __instance._13782B1A6 = Vector3.Angle(new Vector3(__instance._1552E3BEF.x, 0f, __instance._1552E3BEF.z), __instance.centeredAimDirection);
                    if (__instance._1552E3BEF.y < 0f)
                    {
                        __instance._13782B1A6 *= -1f;
                    }
                }
                else
                {
                    transform.forward = new Vector3(0f, 0f, 1f);
                    __instance._13782B1A6 = 0f;
                }
                __instance._1CFFF5F80 = Mathf.Clamp(__instance._1CFFF5F80, -89.99f + __instance._13782B1A6, 89.99f + __instance._13782B1A6);
                if (__instance._15234BCAB)
                {
                    if (__instance._1DC1D4026.x >= 0f && __instance._1DC1D4026.y >= 0f)
                    {
                        __instance._1E9DAA452 = Mathf.Clamp(__instance._1E9DAA452, -__instance._1DC1D4026.x, __instance._1DC1D4026.y);
                    }
                    if (__instance._1DC1D4026.z >= 0f && __instance._1DC1D4026.w >= 0f)
                    {
                        __instance._1CFFF5F80 = Mathf.Clamp(__instance._1CFFF5F80, -__instance._1DC1D4026.z, __instance._1DC1D4026.w);
                    }
                }
                if (__instance._122F739CA)
                {
                    __instance._1CDC44FE7 = __instance._1CFFF5F80;
                    __instance._1DC9DF77C = __instance._1E9DAA452;
                    transform.forward = Quaternion.AngleAxis(__instance._1E9DAA452, Vector3.up) * transform.forward;
                    Vector3 axis = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance._1CFFF5F80, axis) * transform.forward;
                }
                else
                {
                    __instance._1CDC44FE7 = Mathf.SmoothDamp(__instance._1CDC44FE7, __instance._1CFFF5F80, ref __instance._18346372D, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, __instance.m_maxViewChangeSpeed, __instance._19FA60FD3());
                    __instance._1DC9DF77C = Mathf.SmoothDamp(__instance._1DC9DF77C, __instance._1E9DAA452, ref __instance._1293578B2, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, float.PositiveInfinity, __instance._19FA60FD3());
                    transform.forward = Quaternion.AngleAxis(__instance._1DC9DF77C, Vector3.up) * transform.forward;
                    Vector3 axis2 = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance._1CDC44FE7, axis2) * transform.forward;
                }
                __instance.transform.forward = transform.forward;
                return false;
            }
            Vector4 vector = _1DD947C88 - _15E19D274;
            vector.Normalize();
            Vector4 vector2 = new Vector4(-vector.z, 0f, vector.x);
            vector2.Normalize();
            Vector4 vector3 = Vector3.Cross(vector2, vector);
            vector3.Normalize();
            vector2 = Vector3.Cross(vector3, vector);
            vector2.Normalize();
            __instance._146F8D839(_15E19D274, vector2, vector3, vector);
            if (!__instance.IsLocked)
            {
                __instance._1DD2696DE = 0f;
                __instance._1CE82F9BB = 0f;
            }
            if (T_1005C221.T_A9DD5E3E.IsExtendedViewAvailable() && __instance.m_isCameraDriver && !T_A6E913D1.Instance.Paused && !__instance.m_isInteractionBlocked && __instance.IsLocked && (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam || T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kCustomization) && (!__instance.isFreeroamStart || __instance.m_isFreelook) && T_34182F31.main != null)
            {
                ExtendedViewBase.GrabExtendedView<ExtendedViewThirdPerson>(T_34182F31.main, ref __instance._1D9C76B26, ref __instance._1C89EAB96);
                if (__instance._1C89EAB96 != null)
                {
                    float num6 = T_34182F31.main.fieldOfView / 40f;
                    float num7 = __instance._1CFFF5F80 + __instance._1C89EAB96.Pitch * num6;
                    float num8 = __instance._1E9DAA452 + __instance._1C89EAB96.Yaw * num6;
                    if (__instance._15234BCAB)
                    {
                        if ((double)__instance._1DC1D4026.x >= 0.0 && (double)__instance._1DC1D4026.y >= 0.0)
                        {
                            num8 = Mathf.Clamp(num8, -__instance._1DC1D4026.x, __instance._1DC1D4026.y);
                        }
                        if ((double)__instance._1DC1D4026.z >= 0.0 && (double)__instance._1DC1D4026.w >= 0.0)
                        {
                            num7 = Mathf.Clamp(num7, -__instance._1DC1D4026.z, __instance._1DC1D4026.w);
                        }
                    }
                    __instance.transform.forward = Quaternion.AngleAxis(num8, Vector3.up) * __instance.transform.forward;
                    Vector3 axis3 = Vector3.Cross(Vector3.up, __instance.transform.forward);
                    __instance.transform.forward = Quaternion.AngleAxis(num7, axis3) * __instance.transform.forward;
                }
            }
            return false;
        }

        public static void OnMovieWillRenderObject(_1F28E2E62.T_E579AD8A __instance, T_DD163FE9 _17BBCD6A6)
        {
            if (__instance._1E67B0C9C == _1F28E2E62.T_E579AD8A.Status.Ready || __instance._1E67B0C9C == _1F28E2E62.T_E579AD8A.Status.Playing)
            {
                if (!hasDisabledMovieWindow)
                {
                    GameObject.Find("/UIRoot/Camera/MovieWindow/Widget/Quad").SetActive(false);
                    hasDisabledMovieWindow = true;
                }

                VRRig.Instance.CutsceneHandler.SetupCutscene(_17BBCD6A6.material);
            }
        }
    }
}
