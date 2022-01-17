using UnityEngine;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static Camera CutsceneCamera => VRRig.Instance.CutsceneHandler.CurrentCamera;
        private static T_A7F99C25.eCookieChoices CurrentCookie => T_A6E913D1.Instance.m_dawnUI.currentViewCookie;

        public static bool CUICameraRelativeUpdate(T_1C1609D7 __instance)
        {
            if (CurrentCookie == T_A7F99C25.eCookieChoices.kBinoculars || CurrentCookie == T_A7F99C25.eCookieChoices.kE3Binoculars)
            {
                __instance.transform.rotation = CutsceneCamera.transform.rotation;
                return false;
            }

            __instance.transform.rotation = VRRig.Instance.Camera.transform.rotation;
            return false;
        }

        public static bool CUIAnchorUpdatePosition(T_2D9F19A8 __instance)
        {
            if (__instance.m_anchorObj != null)
            {
                Transform parent = __instance.transform.parent;
                __instance.transform.localPosition = (parent == null ? __instance.m_anchorObj.transform.position : parent.InverseTransformPoint(__instance.m_anchorObj.transform.position)) + __instance.m_offset;
            }
            return false;
        }

        public static bool FreeroamWindowUpdate(T_F8FE3E1C __instance)
        {
            bool isBinoc = CurrentCookie == T_A7F99C25.eCookieChoices.kBinoculars|| CurrentCookie == T_A7F99C25.eCookieChoices.kE3Binoculars;
            return isBinoc ? FlatUpdate(__instance) : VRUpdate();

            bool VRUpdate()
            {
                T_F8FE3E1C window = T_E7B3064D.Singleton.GetWindow<T_F8FE3E1C>("FreeRoamWindow");

                if (!window.gameObject.activeInHierarchy) return false;

                if (T_F8FE3E1C.s_currentTriggers.Count > 0)
                {
                    float num = 180f;
                    if (T_F8FE3E1C.s_isFreeLook)
                        num = VRRig.Instance.Camera.Component.fieldOfView / 3f;
                    int triggerIndex = -1;
                    float num3 = 1f;
                    float d = window.m_interactUI.transform.localScale.x;
                    for (int i = 0; i < T_F8FE3E1C.s_currentTriggers.Count; i++)
                    {
                        if (!(T_F8FE3E1C.s_currentTriggers[i].m_pointAt == null))
                        {
                            float num4 = Vector3.Distance(T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position, VRRig.Instance.Camera.transform.position);
                            if (T_F8FE3E1C.s_isFreeLook)
                            {
                                d = 1f;
                                num3 = (Mathf.Max(0f, num4 - 2f) + 1f) * window.m_distanceScaleFactor * (VRRig.Instance.Camera.Component.fieldOfView / 30f);
                                num3 /= 2;
                                T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num3, num3, 1f);
                            }
                            else
                            {
                                float num5 = 1f;
                                if (T_F8FE3E1C.s_currentTriggers[i] != null && T_F8FE3E1C.s_currentTriggers[i].m_nameUI != null)
                                {
                                    num5 = (Mathf.Max(0f, num4 - 3f) + 1f) * 0.28f;
                                    num5 = Mathf.Max(1f, num5);
                                    num5 = Mathf.Min(num5, 3.5f);
                                    T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num5, num5, 1f);
                                }
                                if (window.m_interactUI.hotSpotObj == T_F8FE3E1C.s_currentTriggers[i])
                                {
                                    d = num5;
                                }
                            }

                            Vector3 vector = T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position - VRRig.Instance.Camera.transform.position;
                            float angle = Vector3.Angle(vector, VRRig.Instance.Camera.transform.forward);
                            // angle interact doesnt seem to be imporant so this will just stay commented
                            /*if (T_F8FE3E1C.s_currentTriggers[i].usesAngleInteract)
                            {
                                vector.y = 0f;
                                float num7 = Vector3.Angle(-vector, Vector3.forward);
                                Vector3 vector2 = Vector3.Cross(vector, Vector3.forward);
                                if (vector2.y < 0f)
                                    num7 *= -1f;

                                bool angleValid = false;
                                float num0 = T_D3A1C202.NormalizeAngle(T_F8FE3E1C.s_currentTriggers[i].m_LeftInteractAngle, 360f);
                                float num2 = T_D3A1C202.NormalizeAngle(T_F8FE3E1C.s_currentTriggers[i].m_RightInteractAngle, 360f);
                                num7 = T_D3A1C202.NormalizeAngle(num7, normalizeAngle);
                                if (num0 > num2)
                                {
                                    angleValid = true;
                                    if (num7 < num0 && num7 > num2)
                                        angleValid = false;
                                }
                                if (num7 > num0 && num7 < num2)
                                    angleValid = true;
                                if (!angleValid)
                                    continue;
                            }*/

                            if (angle < num)
                            {
                                if (T_F8FE3E1C.s_isFreeLook)
                                {
                                    num = angle;
                                    triggerIndex = i;
                                    window.m_interactUI.transform.localScale = new Vector3(num3, num3, 1f);
                                }
                                else
                                {
                                    num = angle;
                                    triggerIndex = i;
                                    window.m_interactUI.transform.localScale = Vector2.one * d;
                                }
                            }
                        }
                    }

                    if (triggerIndex >= 0 && ShouldShowUI())
                        window.ShowInteractUI(triggerIndex);
                    else
                        window.HideInteractUI();
                }
                else
                {
                    window.m_interactUI.ClearHotSpot();
                    window.HideInteractUI();
                }

                return false;
            }

            bool FlatUpdate(T_F8FE3E1C instance)
            {
                if (T_34182F31.main != null && T_A6E913D1.Instance.m_followCamera != null)
                {
                    instance._1391877CE = T_A6E913D1.Instance.m_followCamera.gameObject.transform.rotation;
                    instance._11913C85F = T_A6E913D1.Instance.m_followCamera.gameObject.transform.position;
                    ExtendedViewBase.GrabExtendedView<ExtendedViewFirstPerson>(T_34182F31.main, ref instance._1D9C76B26, ref instance._1C89EAB96);
                    if (instance._1C89EAB96 != null && T_1005C221.T_A9DD5E3E.IsWorldInteractionAvailable())
                    {
                        Tobii.Gaming.GazePoint gazePoint = Tobii.Gaming.TobiiAPI.GetGazePoint();
                        if (gazePoint.IsRecent())
                        {
                            Vector3 position = new Vector3(gazePoint.Viewport.x, gazePoint.Viewport.y, 10f);
                            Quaternion rotation = Quaternion.LookRotation(instance._1C89EAB96.CameraWithExtendedView.ViewportPointToRay(position).direction);
                            T_A6E913D1.Instance.m_followCamera.gameObject.transform.position = instance._1C89EAB96.CameraWithExtendedView.transform.position;
                            T_A6E913D1.Instance.m_followCamera.gameObject.transform.rotation = rotation;
                        }
                    }
                }
                if (T_F8FE3E1C.s_hideUI)
                {
                    instance._178527F87();
                    return false;
                }
                if (instance._113C1A8F5)
                {
                    if (CutsceneCamera.fieldOfView != T_34182F31.main.fieldOfView)
                    {
                        CutsceneCamera.fieldOfView = T_34182F31.main.fieldOfView;
                    }
                    if (T_F8FE3E1C.s_isFreeLook && T_A6E913D1.Instance.m_inputManager.GetInputState(eGameInput.kActionRight, true, true, true) == eInputState.kDown)
                    {
                        _15C6DD6D9._14FDF87F1.T_BBB6DDD9 t_BBB6DDD = (_15C6DD6D9._14FDF87F1.T_BBB6DDD9)T_A6E913D1.Instance.m_graphManager.CurrentGraphNode;
                        if (t_BBB6DDD != null)
                        {
                            if (t_BBB6DDD.freeroamObject.canExitWithKey && t_BBB6DDD.freeroamObject.defaultExitTarget != null)
                            {
                                T_A6E913D1.Instance.m_graphManager.m_continuousPlayMode = true;
                                t_BBB6DDD.m_exitGraphObject = t_BBB6DDD.freeroamObject.defaultExitTarget;
                            }
                            else if (!t_BBB6DDD.freeroamObject.canExitWithKey)
                            {
                            }
                        }
                    }
                    if (T_F8FE3E1C.s_currentTriggers.Count > 0 && T_A6E913D1.Instance.m_followCamera != null)
                    {
                        float num = 180f;
                        if (T_F8FE3E1C.s_isFreeLook)
                        {
                            num = T_34182F31.main.fieldOfView / 3f;
                        }
                        int num2 = -1;
                        float num3 = 1f;
                        float num4 = instance.m_interactUI.transform.localScale.x;
                        for (int i = 0; i < T_F8FE3E1C.s_currentTriggers.Count; i++)
                        {
                            if (!(T_F8FE3E1C.s_currentTriggers[i].m_pointAt == null))
                            {
                                float num5 = Vector3.Distance(T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position, T_A6E913D1.Instance.m_followCamera.gameObject.transform.position);
                                if (T_F8FE3E1C.s_isFreeLook)
                                {
                                    num4 = 1f;
                                    num3 = (Mathf.Max(0f, num5 - 2f) + 1f) * instance.m_distanceScaleFactor * (T_34182F31.main.fieldOfView / 30f);
                                    T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num3, num3, 1f);
                                }
                                else
                                {
                                    float num6 = 1f;
                                    if (T_F8FE3E1C.s_currentTriggers[i] != null && T_F8FE3E1C.s_currentTriggers[i].m_nameUI != null)
                                    {
                                        num6 = (Mathf.Max(0f, num5 - 3f) + 1f) * 0.28f;
                                        num6 = Mathf.Max(1f, num6);
                                        num6 = Mathf.Min(num6, 3.5f);
                                        T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num6, num6, 1f);
                                    }
                                    if (instance.m_interactUI.hotSpotObj == T_F8FE3E1C.s_currentTriggers[i])
                                    {
                                        num4 = num6;
                                    }
                                }
                                Vector3 vector = T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position - T_A6E913D1.Instance.m_followCamera.gameObject.transform.position;
                                float num7 = Vector3.Angle(vector, T_A6E913D1.Instance.m_followCamera.gameObject.transform.forward);
                                if (T_F8FE3E1C.s_currentTriggers[i].usesAngleInteract)
                                {
                                    vector.y = 0f;
                                    float num8 = Vector3.Angle(-vector, Vector3.forward);
                                    if (Vector3.Cross(vector, Vector3.forward).y < 0f)
                                    {
                                        num8 *= -1f;
                                    }
                                    if (!T_F8FE3E1C.s_currentTriggers[i].GetAngleInteractValid(num8))
                                    {
                                        goto IL_613;
                                    }
                                }
                                if (num7 < num)
                                {
                                    if (T_F8FE3E1C.s_isFreeLook)
                                    {
                                        num = num7;
                                        num2 = i;
                                        instance.m_interactUI.transform.localScale = new Vector3(num3, num3, 1f);
                                    }
                                    else if (num5 < T_F8FE3E1C.s_currentTriggers[i].m_interactDistanceOverride)
                                    {
                                        num = num7;
                                        num2 = i;
                                        instance.m_interactUI.transform.localScale = Vector2.one * num4;
                                    }
                                    else if (T_A6E913D1.Instance.m_mainCharacter != null && Vector3.Distance(T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position, T_A6E913D1.Instance.m_mainCharacter.transform.position) < T_F8FE3E1C.s_currentTriggers[i].m_interactDistanceOverride)
                                    {
                                        num = num7;
                                        num2 = i;
                                        instance.m_interactUI.transform.localScale = Vector2.one * num4;
                                    }
                                }
                            }
                        IL_613:;
                        }
                        if (num2 >= 0)
                        {
                            float _149F = Vector3.Distance(T_F8FE3E1C.s_currentTriggers[num2].m_pointAt.transform.position, T_A6E913D1.Instance.m_followCamera.gameObject.transform.position);
                            if (T_1005C221.T_A9DD5E3E.IsWorldInteractionAvailable())
                            {
                                T_6FD30C1C t_6FD30C1C = T_F8FE3E1C.s_currentTriggers[num2];
                                if (instance._1BC8C4984 != t_6FD30C1C)
                                {
                                    instance._1C172E9A = Time.unscaledTime;
                                    instance._1BC8C4984 = t_6FD30C1C;
                                }
                                bool flag = (!T_408CFC35.s_inHotspotRange || T_A6E913D1.Instance.m_inputManager.GetInputState(eGameInput.kInteract, true, true, true) == eInputState.kNone) && T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam && !T_A6E913D1.Instance.Paused;
                                if (flag && Time.unscaledTime - instance._1C172E9A > instance._131D79451)
                                {
                                    instance._1F6A23D9C(T_F8FE3E1C.s_currentTriggers[num2], _149F, num4, num3);
                                    instance.ShowInteractUI(num2);
                                }
                            }
                            else if (T_1005C221.T_A9DD5E3E.IsExtendedViewAvailable())
                            {
                                bool flag2 = (!T_408CFC35.s_inHotspotRange || T_A6E913D1.Instance.m_inputManager.GetInputState(eGameInput.kInteract, true, true, true) == eInputState.kNone) && T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kFreeRoam && !T_A6E913D1.Instance.Paused;
                                if (flag2)
                                {
                                    instance._1F6A23D9C(T_F8FE3E1C.s_currentTriggers[num2], _149F, num4, num3);
                                    instance.ShowInteractUI(num2);
                                }
                            }
                            else
                            {
                                instance._1F6A23D9C(T_F8FE3E1C.s_currentTriggers[num2], _149F, num4, num3);
                                instance.ShowInteractUI(num2);
                            }
                        }
                        else
                        {
                            instance.HideInteractUI();
                            instance._1BC8C4984 = null;
                        }
                    }
                    else
                    {
                        instance.m_interactUI.ClearHotSpot();
                        instance.HideInteractUI();
                    }
                    if (instance._1343C5481)
                    {
                        T_6FCAE66C inputManager = T_A6E913D1.Instance.m_inputManager;
                        eGameInput _1561EDFFF = eGameInput.kOpenBurner;
                        bool _17C6D1EDA = instance._17C6D1EDA;
                        if (inputManager.GetInputState(_1561EDFFF, true, true, _17C6D1EDA) == eInputState.kDown)
                        {
                            T_A6E913D1.Instance.m_dawnUI.OpenBurnerSMS();
                        }
                    }
                    else
                    {
                        instance.CheckBurnerSMS();
                    }
                }
                instance._1649E566F();
                return false;
            }
        }

        public static bool IsHotspotOnScreen(T_8F74F848 __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
                return true;

            if (__instance.m_anchor == null || __instance.m_anchor.m_anchorObj == null)
            {
                __result = false;
                return false;
            }

            float distance = Vector3.Distance(VRRig.Instance.Camera.transform.position, __instance.m_anchor.m_anchorObj.transform.position);
            if (distance < 20)
            {
                __instance._14888EF3 = 1f;
                __result = true;
                return false;
            }
            else
                __instance._14888EF3 = 0f;

            __result = false;
            return false;
        }

        public static bool IsInteractOnScreen(T_572A4969 __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
                return true;

            if (__instance.m_anchor != null && __instance.m_anchor.m_anchorObj != null)
            {
                T_6FD30C1C hotspotObj = __instance._133075675;
                float num = Vector3.Angle(VRRig.Instance.Camera.transform.forward, __instance.m_anchor.m_anchorObj.transform.position - VRRig.Instance.Camera.transform.position);
                if (num < 90f)
                {
                    float distance = Vector3.Distance(VRRig.Instance.Camera.transform.position, __instance.m_anchor.m_anchorObj.transform.position);
                    if (distance < 3)
                    {
                        __instance.m_arrow.gameObject.SetActive(true);
                        __instance.m_choiceUI.gameObject.SetActive(true);
                        if (hotspotObj != null)
                            hotspotObj.Select(true, true);
                        __result = true;
                        return false;
                    }
                }
                __instance.m_arrow.gameObject.SetActive(false);
                __instance.m_choiceUI.gameObject.SetActive(false);
                if (hotspotObj != null)
                    hotspotObj.Select(false, false);
            }

            __result = false;
            return false;
        }

        public static bool IsHoverObjectOnScreen(T_A0A6EA62 __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
                return true;

            Vector3 vector = VRRig.Instance.Camera.Component.WorldToScreenPoint(__instance.m_anchor.m_anchorObj.transform.position);
            if (vector.x > 0f && vector.y > 0f && vector.x < VRRig.Instance.Camera.Component.pixelWidth && vector.y < VRRig.Instance.Camera.Component.pixelHeight)
            {
                float num = VRRig.Instance.Camera.Component.pixelWidth * T_A0A6EA62._1D66D99B4;

                if (vector.x < num)
                    __instance._14888EF3 = Mathf.Lerp(0f, 1f, vector.x / num);
                else if (vector.x > VRRig.Instance.Camera.Component.pixelWidth - num)
                    __instance._14888EF3 = Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelWidth - vector.x) / num);
                else
                    __instance._14888EF3 = 1f;

                num = VRRig.Instance.Camera.Component.pixelHeight * T_A0A6EA62._1D66D99B4;

                if (vector.y < num)
                    __instance._14888EF3 *= Mathf.Lerp(0f, 1f, vector.y / num);
                else if (vector.y > VRRig.Instance.Camera.Component.pixelHeight - num)
                    __instance._14888EF3 *= Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelHeight - vector.y) / num);

                __instance._1649E566F();
                __result = true;
                return false;
            }
            __instance._1649E566F();

            __result = false;
            return false;
        }

        public static void HotspotObjectInteract(T_6FD30C1C _1BAF664A9) => _1BAF664A9.m_lookAt = null;

        private static bool ShouldShowUI()
        {
            if (VRRig.Instance.Input.IsUsingViveWand)
            {
                var source = VRRig.Instance.Input.GetGrip(VRInput.Hand.Left);
                if (source.state)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }
}
