using UnityEngine;
using DawnVR.Modules.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool CUICameraRelativeUpdate(T_1C1609D7 __instance)
        {
            __instance.transform.rotation = VRRig.Instance.Camera.transform.rotation;
            return false;
        }

        public static bool CUIAnchorUpdatePosition(T_2D9F19A8 __instance)
        {
            if (__instance.m_anchorObj != null)
            {
                Transform parent = __instance.transform.parent;
                __instance.transform.localPosition = ((!(parent != null)) ? __instance.m_anchorObj.transform.position : parent.InverseTransformPoint(__instance.m_anchorObj.transform.position)) + __instance.m_offset;
            }
            return false;
        }

        public static void HotspotObjectInteract(T_6FD30C1C _1BAF664A9) => _1BAF664A9.m_lookAt = null;

        public static bool IsHotspotOnScreen(T_8F74F848 __instance, ref bool __result)
        {
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
