using UnityEngine;
#if !REMASTER
using Boundary = T_3BE79CFB;
using FollowCamera = T_884A92DB;
using GameMaster = T_A6E913D1;
using FreeRoamWindow = T_F8FE3E1C;
using ChoiceSelectionUI = T_6876113C;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static bool BoundaryStart(Boundary __instance)
        {
            __instance.GetComponent<Collider>().isTrigger = false;
            return false;
        }

        public static bool FollowCamUpdateInputVars(FollowCamera __instance)
        {
            float num = __instance.m_camMomentumStep * Time.deltaTime;
            Vector3 axisVector = Vector3.zero;
            Vector3 axisVector2 = GameMaster.Instance.m_inputManager.GetAxisVector3(eGameInput.kMovementXPositive, eGameInput.kNone, eGameInput.kMovementYPositive);
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

        public static void ChoiceButtonSelection(ChoiceSelectionUI __instance, bool __result)
        {
            if (__result)
                MelonLoader.MelonCoroutines.Start(EnableInteraction());
        }

        private static System.Collections.IEnumerator EnableInteraction()
        {
            yield return new WaitForSeconds(1);
            if (!VR.VRRig.Instance.CutsceneHandler.IsActive)
            {
                GameMaster.Instance.m_followCamera.m_isInteractionBlocked = false;
                GameMaster.Instance.m_gameModeManager.isDebug = false;
                FreeRoamWindow.s_hideUI = false;
            }
        }
    }
}
