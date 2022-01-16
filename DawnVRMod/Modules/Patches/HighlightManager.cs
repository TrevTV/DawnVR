using UnityEngine;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool CUICameraRelativeUpdate(T_1C1609D7 __instance)
        {
            var currentCookie = T_A6E913D1.Instance.m_dawnUI.currentViewCookie;
            if ((VRRig.Instance.CutsceneHandler.IsCutsceneActive
                && currentCookie != T_A7F99C25.eCookieChoices.kBinoculars)
                || currentCookie == T_A7F99C25.eCookieChoices.kE3Binoculars)
                return true;

            if (currentCookie != T_A7F99C25.eCookieChoices.kBinoculars)
                __instance.transform.rotation = VRRig.Instance.Camera.transform.rotation;
            else
                __instance.transform.rotation = VRRig.Instance.CutsceneHandler.CurrentCutsceneCamera.transform.rotation;
            return false;
        }

        public static bool CUIAnchorUpdatePosition(T_2D9F19A8 __instance)
        {
            if (VRRig.Instance.CutsceneHandler.IsCutsceneActive)
                return true;

            if (__instance.m_anchorObj != null)
            {
                Transform parent = __instance.transform.parent;
                __instance.transform.localPosition = ((!(parent != null)) ? __instance.m_anchorObj.transform.position : parent.InverseTransformPoint(__instance.m_anchorObj.transform.position)) + __instance.m_offset;
            }
            return false;
        }

        public static bool FreeroamWindowUpdate()
        {
            if (VRRig.Instance.CutsceneHandler.IsCutsceneActive && VRRig.Instance.CutsceneHandler.IsIn2DCutsceneMode)
                return true;

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

        public static void HotspotObjectInteract(T_6FD30C1C _1BAF664A9) => _1BAF664A9.m_lookAt = null;

        public static bool IsHotspotOnScreen(T_8F74F848 __instance, ref bool __result)
        {
            if (VRRig.Instance.CutsceneHandler.IsCutsceneActive)
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
            if (VRRig.Instance.CutsceneHandler.IsCutsceneActive)
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
            if (VRRig.Instance.CutsceneHandler.IsCutsceneActive)
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
    }
}
