using UnityEngine;
using DawnVR.Modules.VR;
#if REMASTER
using PrototyperData.GraphObjects;
#else
using OverlayCookie = T_A7F99C25;
using GameMaster = T_A6E913D1;
using CUICameraRelative = T_1C1609D7;
using CUI3DAnchor = T_2D9F19A8;
using FreeRoamWindow = T_F8FE3E1C;
using HotSpotUI = T_8F74F848;
using InteractUI = T_572A4969;
using ST_Hotspot = T_6FD30C1C;
using HoverObjectUI = T_A0A6EA62;
using CWindowManager = T_E7B3064D;
using DawnMainCamera = T_34182F31;
using FreeroamGraphObject = _15C6DD6D9._14FDF87F1.T_BBB6DDD9;
using InputManager = T_6FCAE66C;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static Camera CutsceneCamera => VRRig.Instance.CutsceneHandler.CurrentCamera;
        private static OverlayCookie.eCookieChoices CurrentCookie => GameMaster.Instance.m_dawnUI.currentViewCookie;

        public static bool CUICameraRelativeUpdate(CUICameraRelative __instance)
        {
            if (CurrentCookie == OverlayCookie.eCookieChoices.kBinoculars || CurrentCookie == OverlayCookie.eCookieChoices.kE3Binoculars)
            {
                __instance.transform.rotation = CutsceneCamera.transform.rotation;
                return false;
            }

            __instance.transform.rotation = VRRig.Instance.Camera.transform.rotation;
            return false;
        }

        public static bool CUIAnchorUpdatePosition(CUI3DAnchor __instance)
        {
            if (GameMaster.Instance.m_gameModeManager.CurrentMode == eGameMode.kCustomization)
                return true;

            if (__instance.m_anchorObj != null)
            {
                Transform parent = __instance.transform.parent;
                __instance.transform.localPosition = (parent == null ? __instance.m_anchorObj.transform.position : parent.InverseTransformPoint(__instance.m_anchorObj.transform.position)) + __instance.m_offset;
            }
            return false;
        }

        public static bool FreeroamWindowUpdate(FreeRoamWindow __instance)
        {
            bool isBinoc = CurrentCookie == OverlayCookie.eCookieChoices.kBinoculars || CurrentCookie == OverlayCookie.eCookieChoices.kE3Binoculars;
            return isBinoc ? FlatUpdate(__instance) : VRUpdate();

            bool VRUpdate()
            {
                FreeRoamWindow window = CWindowManager.Singleton.GetWindow<FreeRoamWindow>("FreeRoamWindow");

                if (!window.gameObject.activeInHierarchy) return false;

                if (FreeRoamWindow.s_currentTriggers.Count > 0)
                {
                    float num = 180f;
                    if (FreeRoamWindow.s_isFreeLook)
                        num = VRRig.Instance.Camera.Component.fieldOfView / 3f;
                    int triggerIndex = -1;
                    float num3 = 1f;
                    float d = window.m_interactUI.transform.localScale.x;
                    for (int i = 0; i < FreeRoamWindow.s_currentTriggers.Count; i++)
                    {
                        if (!(FreeRoamWindow.s_currentTriggers[i].m_pointAt == null))
                        {
                            float num4 = Vector3.Distance(FreeRoamWindow.s_currentTriggers[i].m_pointAt.transform.position, VRRig.Instance.Camera.transform.position);
                            if (FreeRoamWindow.s_isFreeLook)
                            {
                                d = 1f;
                                num3 = (Mathf.Max(0f, num4 - 2f) + 1f) * window.m_distanceScaleFactor * (VRRig.Instance.Camera.Component.fieldOfView / 30f);
                                num3 /= 2;
                                FreeRoamWindow.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num3, num3, 1f);
                            }
                            else
                            {
                                float num5 = 1f;
                                if (FreeRoamWindow.s_currentTriggers[i] != null && FreeRoamWindow.s_currentTriggers[i].m_nameUI != null)
                                {
                                    num5 = (Mathf.Max(0f, num4 - 3f) + 1f) * 0.28f;
                                    num5 = Mathf.Max(1f, num5);
                                    num5 = Mathf.Min(num5, 3.5f);
                                    FreeRoamWindow.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num5, num5, 1f);
                                }
                                if (window.m_interactUI.hotSpotObj == FreeRoamWindow.s_currentTriggers[i])
                                {
                                    d = num5;
                                }
                            }

                            Vector3 vector = FreeRoamWindow.s_currentTriggers[i].m_pointAt.transform.position - VRRig.Instance.Camera.transform.position;
                            float angle = Vector3.Angle(vector, VRRig.Instance.Camera.transform.forward);
                            // angle interact doesnt seem to be imporant so this will just stay commented
                            /*if (FreeRoamWindow.s_currentTriggers[i].usesAngleInteract)
                            {
                                vector.y = 0f;
                                float num7 = Vector3.Angle(-vector, Vector3.forward);
                                Vector3 vector2 = Vector3.Cross(vector, Vector3.forward);
                                if (vector2.y < 0f)
                                    num7 *= -1f;

                                bool angleValid = false;
                                float num0 = T_D3A1C202.NormalizeAngle(FreeRoamWindow.s_currentTriggers[i].m_LeftInteractAngle, 360f);
                                float num2 = T_D3A1C202.NormalizeAngle(FreeRoamWindow.s_currentTriggers[i].m_RightInteractAngle, 360f);
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
                                if (FreeRoamWindow.s_isFreeLook)
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

            bool FlatUpdate(FreeRoamWindow instance)
            {
                // dont ask questions, i dont know either
#if !REMASTER
                if (DawnMainCamera.main != null && GameMaster.Instance.m_followCamera != null)
                {
                    instance._1391877CE = GameMaster.Instance.m_followCamera.gameObject.transform.rotation;
                    instance._11913C85F = GameMaster.Instance.m_followCamera.gameObject.transform.position;
                    ExtendedViewBase.GrabExtendedView<ExtendedViewFirstPerson>(T_34182F31.main, ref instance._1D9C76B26, ref instance._1C89EAB96);
                    if (instance._1C89EAB96 != null && T_1005C221.T_A9DD5E3E.IsWorldInteractionAvailable())
                    {
                        Tobii.Gaming.GazePoint gazePoint = Tobii.Gaming.TobiiAPI.GetGazePoint();
                        if (gazePoint.IsRecent())
                        {
                            Vector3 position = new Vector3(gazePoint.Viewport.x, gazePoint.Viewport.y, 10f);
                            Quaternion rotation = Quaternion.LookRotation(instance._1C89EAB96.CameraWithExtendedView.ViewportPointToRay(position).direction);
                            GameMaster.Instance.m_followCamera.gameObject.transform.position = instance._1C89EAB96.CameraWithExtendedView.transform.position;
                            GameMaster.Instance.m_followCamera.gameObject.transform.rotation = rotation;
                        }
                    }
                }
#endif
                if (FreeRoamWindow.s_hideUI)
                {
                    instance.CallMethod("HideUI");
                    return false;
                }
                if (instance.GetFieldValue<bool>("m_acceptingInput"))
                {
                    if (FreeRoamWindow.s_isFreeLook && GameMaster.Instance.m_inputManager.GetInputState(eGameInput.kActionRight, true, true, true) == eInputState.kDown)
                    {
#if REMASTER
                        FreeroamGraphObject freeroamGraphObj = GameMaster.Instance.m_graphManager.CurrentGraphNode.Cast<FreeroamGraphObject>();
#else
                        FreeroamGraphObject freeroamGraphObj = (FreeroamGraphObject)GameMaster.Instance.m_graphManager.CurrentGraphNode;
#endif
                        if (freeroamGraphObj != null)
                        {
                            if (freeroamGraphObj.freeroamObject.canExitWithKey && freeroamGraphObj.freeroamObject.defaultExitTarget != null)
                            {
                                GameMaster.Instance.m_graphManager.m_continuousPlayMode = true;
                                freeroamGraphObj.m_exitGraphObject = freeroamGraphObj.freeroamObject.defaultExitTarget;
                            }
                        }
                    }
                    if (FreeRoamWindow.s_currentTriggers.Count > 0 && GameMaster.Instance.m_followCamera != null)
                    {
                        float num = 180f;
                        if (FreeRoamWindow.s_isFreeLook)
                        {
                            num = VRRig.Instance.CutsceneHandler.CurrentCamera.fieldOfView / 3f;
                        }
                        int num2 = -1;
                        float num3 = 1f;
                        float num4 = instance.m_interactUI.transform.localScale.x;
                        for (int i = 0; i < FreeRoamWindow.s_currentTriggers.Count; i++)
                        {
                            if (!(FreeRoamWindow.s_currentTriggers[i].m_pointAt == null))
                            {
                                float num5 = Vector3.Distance(FreeRoamWindow.s_currentTriggers[i].m_pointAt.transform.position, GameMaster.Instance.m_followCamera.gameObject.transform.position);
                                if (FreeRoamWindow.s_isFreeLook)
                                {
                                    num4 = 1f;
                                    num3 = (Mathf.Max(0f, num5 - 2f) + 1f) * instance.m_distanceScaleFactor * (VRRig.Instance.CutsceneHandler.CurrentCamera.fieldOfView / 30f);
                                    FreeRoamWindow.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num3, num3, 1f);
                                }
                                else
                                {
                                    float num6 = 1f;
                                    if (FreeRoamWindow.s_currentTriggers[i] != null && FreeRoamWindow.s_currentTriggers[i].m_nameUI != null)
                                    {
                                        num6 = (Mathf.Max(0f, num5 - 3f) + 1f) * 0.28f;
                                        num6 = Mathf.Max(1f, num6);
                                        num6 = Mathf.Min(num6, 3.5f);
                                        FreeRoamWindow.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num6, num6, 1f);
                                    }
                                    if (instance.m_interactUI.hotSpotObj == FreeRoamWindow.s_currentTriggers[i])
                                    {
                                        num4 = num6;
                                    }
                                }
                                Vector3 vector = FreeRoamWindow.s_currentTriggers[i].m_pointAt.transform.position - GameMaster.Instance.m_followCamera.gameObject.transform.position;
                                float num7 = Vector3.Angle(vector, GameMaster.Instance.m_followCamera.gameObject.transform.forward);
                                if (FreeRoamWindow.s_currentTriggers[i].usesAngleInteract)
                                {
                                    vector.y = 0f;
                                    float num8 = Vector3.Angle(-vector, Vector3.forward);
                                    if (Vector3.Cross(vector, Vector3.forward).y < 0f)
                                    {
                                        num8 *= -1f;
                                    }
                                    if (!FreeRoamWindow.s_currentTriggers[i].GetAngleInteractValid(num8))
                                    {
                                        goto IL_613;
                                    }
                                }
                                if (num7 < num)
                                {
                                    if (FreeRoamWindow.s_isFreeLook)
                                    {
                                        num = num7;
                                        num2 = i;
                                        instance.m_interactUI.transform.localScale = new Vector3(num3, num3, 1f);
                                    }
                                    else if (num5 < FreeRoamWindow.s_currentTriggers[i].m_interactDistanceOverride)
                                    {
                                        num = num7;
                                        num2 = i;
                                        instance.m_interactUI.transform.localScale = Vector2.one * num4;
                                    }
                                    else if (GameMaster.Instance.m_mainCharacter != null && Vector3.Distance(FreeRoamWindow.s_currentTriggers[i].m_pointAt.transform.position, GameMaster.Instance.m_mainCharacter.transform.position) < FreeRoamWindow.s_currentTriggers[i].m_interactDistanceOverride)
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
                            float _149F = Vector3.Distance(FreeRoamWindow.s_currentTriggers[num2].m_pointAt.transform.position, GameMaster.Instance.m_followCamera.gameObject.transform.position);
                            // removed tobii support... we're in vr
#if !REMASTER
                            instance._1F6A23D9C(FreeRoamWindow.s_currentTriggers[num2], _149F, num4, num3);
#endif
                            instance.ShowInteractUI(num2);
                        }
                        else
                        {
                            instance.HideInteractUI();
#if !REMASTER
                            instance._1BC8C4984 = null;
#endif
                        }
                    }
                    else
                    {
                        instance.m_interactUI.ClearHotSpot();
                        instance.HideInteractUI();
                    }
                    if (instance.GetFieldValue<bool>("m_shouldShowBurner"))
                    {
                        InputManager inputManager = GameMaster.Instance.m_inputManager;
                        eGameInput _1561EDFFF = eGameInput.kOpenBurner;
                        bool _17C6D1EDA = instance.GetFieldValue<bool>("m_burnerKeyActive");
                        if (inputManager.GetInputState(_1561EDFFF, true, true, _17C6D1EDA) == eInputState.kDown)
                        {
                            GameMaster.Instance.m_dawnUI.OpenBurnerSMS();
                        }
                    }
                    else
                    {
                        instance.CheckBurnerSMS();
                    }
                }
#if !REMASTER
                instance._1649E566F();
#endif
                return false;
            }
        }

        public static bool IsHotspotOnScreen(HotSpotUI __instance, ref bool __result)
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
                __instance.SetFieldValue("m_screenAlpha", 1f);
                __result = true;
                return false;
            }
            else
                __instance.SetFieldValue("m_screenAlpha", 0f);

            __result = false;
            return false;
        }

        public static bool IsInteractOnScreen(InteractUI __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
                return true;

            if (__instance.m_anchor != null && __instance.m_anchor.m_anchorObj != null)
            {
                ST_Hotspot hotspotObj = __instance.GetFieldValue<ST_Hotspot>("m_hotSpotObj");
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

        public static bool IsHoverObjectOnScreen(HoverObjectUI __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
                return true;

            Vector3 vector = VRRig.Instance.Camera.Component.WorldToScreenPoint(__instance.m_anchor.m_anchorObj.transform.position);
            if (vector.x > 0f && vector.y > 0f && vector.x < VRRig.Instance.Camera.Component.pixelWidth && vector.y < VRRig.Instance.Camera.Component.pixelHeight)
            {
                float num = VRRig.Instance.Camera.Component.pixelWidth * ObfuscationTools.GetFieldValue<float>(null, "s_fadePercent", typeof(HoverObjectUI));

                if (vector.x < num)
                    __instance.SetFieldValue("m_screenAlpha", Mathf.Lerp(0f, 1f, vector.x / num));
                else if (vector.x > VRRig.Instance.Camera.Component.pixelWidth - num)
                    __instance.SetFieldValue("m_screenAlpha", Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelWidth - vector.x) / num));
                else
                    __instance.SetFieldValue("m_screenAlpha", 1f);

                num = VRRig.Instance.Camera.Component.pixelHeight * ObfuscationTools.GetFieldValue<float>(null, "s_fadePercent", typeof(HoverObjectUI));

                if (vector.y < num)
                    __instance.SetFieldValue("m_screenAlpha", __instance.GetFieldValue<float>("m_screenAlpha") * Mathf.Lerp(0f, 1f, vector.y / num));
                else if (vector.y > VRRig.Instance.Camera.Component.pixelHeight - num)
                    __instance.SetFieldValue("m_screenAlpha", __instance.GetFieldValue<float>("m_screenAlpha") * Mathf.Lerp(0f, 1f, (VRRig.Instance.Camera.Component.pixelHeight - vector.y) / num));

#if !REMASTER
                __instance._1649E566F();
#endif
                __result = true;
                return false;
            }
#if !REMASTER
            __instance._1649E566F();
#endif

            __result = false;
            return false;
        }

        public static void HotspotObjectInteract(ST_Hotspot __1) => __1.m_lookAt = null;

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
