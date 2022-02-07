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

            __instance.SetFieldValue("m_rightH", 0f);
            __instance.SetFieldValue("m_rightV", 0f);
            if (axisVector != Vector3.zero)
            {
                __instance.SetFieldValue("m_rightH", -__instance.CallMethod<float>("RemoveDeadZone", axisVector.x * __instance.m_OptionSensitivity, __instance.m_mouseDeadZone) * __instance.m_mouseHFactor);
                __instance.SetFieldValue("m_rightV", -__instance.CallMethod<float>("RemoveDeadZone", axisVector.x * __instance.m_OptionSensitivity, __instance.m_mouseDeadZone) * __instance.m_mouseVFactor);
                if (__instance.GetFieldValue<float>("m_rightH") != 0f || __instance.GetFieldValue<float>("m_rightV") != 0f)
                {
                    __instance.SetFieldValue("m_isPCInput", true);
                    __instance.SetFieldValue("m_camVMomentum", __instance.m_camMomentumMin);
                    __instance.SetFieldValue("m_camHMomentum", __instance.m_camMomentumMin);
                }
            }
            if (axisVector2 != Vector3.zero)
            {
                float num2 = __instance.CallMethod<float>("RemoveDeadZone", axisVector2.x, __instance.m_deadZone);
                float num3 = __instance.CallMethod<float>("RemoveDeadZone", axisVector2.z, __instance.m_deadZone);
                if (__instance.GetFieldValue<bool>("m_collidedLastFrame") && (__instance.GetFieldValue<float>("m_camHMomentum") == 0f || __instance.GetFieldValue<float>("m_camVMomentum") == 0f))
                {
                    num3 *= __instance.m_collisionMomentumCompensator;
                    num2 *= __instance.m_collisionMomentumCompensator;
                }
                __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") + (-num2 * __instance.m_joystickHFactor + __instance.GetFieldValue<float>("m_camHMomentum")) * __instance.m_OptionSensitivity);
                __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") + (-num3 * __instance.m_joystickVFactor + __instance.GetFieldValue<float>("m_camVMomentum")) * __instance.m_OptionSensitivity);
                if (Mathf.Abs(num2) > 0.001f)
                {
                    if (num2 < 0f)
                    {
                        if (__instance.GetFieldValue<float>("m_hStickLast") > 0f)
                        {
                            __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") + __instance.GetFieldValue<float>("m_camHMomentum"));
                            __instance.SetFieldValue("m_camHMomentum", __instance.m_camMomentumMin);
                        }
                        __instance.SetFieldValue("m_camHMomentum", Mathf.Min(__instance.m_camMomentumMax * (Mathf.Abs(num2) + __instance.m_deadZone), __instance.GetFieldValue<float>("m_camHMomentum") + num * -num2));
                    }
                    else
                    {
                        if (__instance.GetFieldValue<float>("m_hStickLast") < 0f)
                        {
                            __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") + __instance.GetFieldValue<float>("m_camHMomentum"));
                            __instance.SetFieldValue("m_camHMomentum", __instance.m_camMomentumMin);
                        }
                        __instance.SetFieldValue("m_camHMomentum", Mathf.Min(-__instance.m_camMomentumMax * (Mathf.Abs(num2) + __instance.m_deadZone), __instance.GetFieldValue<float>("m_camHMomentum") - num * num2));
                    }
                    __instance.SetFieldValue("m_isPCInput", false);
                }
                else
                {
                    __instance.SetFieldValue("m_camHMomentum", __instance.m_camMomentumMin);
                }

                __instance.SetFieldValue("m_hStickLast", num2);
                if (Mathf.Abs(num3) > 0.001f)
                {
                    if (num3 < 0f)
                    {
                        if (__instance.GetFieldValue<float>("m_vStickLast") > 0f)
                        {
                            __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") + __instance.GetFieldValue<float>("m_camVMomentum"));
                            __instance.SetFieldValue("m_camVMomentum", __instance.m_camMomentumMin);
                        }
                        __instance.SetFieldValue("m_camVMomentum", Mathf.Min(__instance.m_camMomentumMax * (Mathf.Abs(num3) + __instance.m_deadZone), __instance.GetFieldValue<float>("m_camVMomentum") + num * -num3));
                    }
                    else
                    {
                        if (__instance.GetFieldValue<float>("m_vStickLast") < 0f)
                        {
                            __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") + __instance.GetFieldValue<float>("m_camVMomentum"));
                            __instance.SetFieldValue("m_camVMomentum", __instance.m_camMomentumMin);
                        }
                        __instance.SetFieldValue("m_camVMomentum", Mathf.Min(-__instance.m_camMomentumMax * (Mathf.Abs(num3) + __instance.m_deadZone), __instance.GetFieldValue<float>("m_camVMomentum") - num * num3));
                    }
                    __instance.SetFieldValue("m_isPCInput", false);
                }
                else
                    __instance.SetFieldValue("m_camVMomentum", __instance.m_camMomentumMin);
                __instance.SetFieldValue("m_vStickLast", num3);
            }
            if (!__instance.GetFieldValue<bool>("m_isPCInput"))
            {
                if (__instance.GetFieldValue<float>("m_rightH") == 0f)
                {
                    __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") + __instance.GetFieldValue<float>("m_camHMomentum"));
                    __instance.SetFieldValue("m_camHMomentum", __instance.m_camMomentumMin);
                }
                if (__instance.GetFieldValue<float>("m_rightV") == 0f)
                {
                    __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") + __instance.GetFieldValue<float>("m_camVMomentum"));
                    __instance.SetFieldValue("m_camVMomentum", __instance.m_camMomentumMin);
                }
                __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") * Time.deltaTime);
                __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") * Time.deltaTime);
            }
            __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") * __instance.GetFieldValue<Vector2>("m_momentumOverride").x);
            __instance.SetFieldValue("m_rightV", __instance.GetFieldValue<float>("m_rightV") * __instance.GetFieldValue<Vector2>("m_momentumOverride").y);
            if (__instance.GetFieldValue<float>("m_rightH") == 0f && __instance.GetFieldValue<float>("m_rightV") == 0f)
            {
                if (__instance.Character.IsMoving || __instance.GetFieldValue<bool>("m_firstUserFrame"))
                    __instance.UpdateUserControlled();
            }
            else
                __instance.UpdateUserControlled();

            __instance.CallMethod("ResetFreedom");
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
