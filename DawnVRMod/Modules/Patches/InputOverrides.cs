using DawnVR.Modules.VR;
using UnityEngine;
using System.Linq;
using Valve.VR;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        // used for unlocking UIRenderer from the headset
        private const float delayBetweenPresses = 0.2f;
        private static bool pressedFirstTime = false;
        private static float lastPressedTime;

        public static void ManagerInit(T_6FCAE66C __instance) => __instance._1C6FBAE09 = eControlType.kXboxOne;

        public static eInputState GetInputState_Binding(T_6FCAE66C inputManInstance, T_9005A419 binding)
        {
            if (inputManInstance.InputBlocked)
                return eInputState.kNone;

            for (int i = 0; i < binding.m_joystick.Count; i++)
            {
                eInputState buttonState = T_D9E8342E.Singleton.GetButtonState(binding.m_joystick[i]);
                if (buttonState != eInputState.kNone)
                    return buttonState;
            }

            return eInputState.kNone;
        }

        public static bool GetInputState_Enum(T_6FCAE66C __instance, ref eInputState __result, eGameInput _1561EDFFF)
        {
            if (__instance.InputBlocked)
            {
                __result = eInputState.kNone;
                return false;
            }

            if (_1561EDFFF == eGameInput.kAny)
            {
                if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.stateDown))
                {
                    __result = eInputState.kDown;
                    return false;
                }
                if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.state))
                {
                    __result = eInputState.kHeld;
                    return false;
                }
            }
            else if (__instance.m_keyBindings.ContainsKey((int)_1561EDFFF))
            {
                T_9005A419 keybinding = __instance.m_keyBindings[(int)_1561EDFFF];
                __result = GetInputState_Binding(__instance, keybinding);
                return false;
            }

            __result = eInputState.kNone;
            return false;
        }

        public static bool GetButtonState(ref eInputState __result, eJoystickKey _13A42C455)
        {
            __result = eInputState.kNone;

            switch (_13A42C455)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kStart:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kSelect:
                    var inputState = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                    if (inputState == eInputState.kDown)
                    {
                        if (pressedFirstTime)
                        {
                            bool isDoublePress = Time.time - lastPressedTime <= delayBetweenPresses;
                            if (isDoublePress)
                            {
                                VRRig.Instance.Camera.ToggleUIRendererParent();
                                pressedFirstTime = false;
                            }
                        }
                        else
                            pressedFirstTime = true;
                        lastPressedTime = Time.time;
                    }

                    if (pressedFirstTime && Time.time - lastPressedTime > delayBetweenPresses)
                        __result = inputState;
                    break;
                case eJoystickKey.kR1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kR2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kR3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                    break;
                case eJoystickKey.kL1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kL2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kL3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                    break;
                case eJoystickKey.kAction1:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonX());
                    break;
                case eJoystickKey.kAction2:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonY());
                    break;
                case eJoystickKey.kAction3:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonB());
                    break;
                case eJoystickKey.kAction4:
                    __result = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonA());
                    break;
                // are these used here?
                case eJoystickKey.kPlatform:
                    break;
                case eJoystickKey.kLeftStickX:
                    break;
                case eJoystickKey.kLeftStickY:
                    break;
                case eJoystickKey.kRightStickX:
                    break;
                case eJoystickKey.kRightStickY:
                    break;
                default:
                    break;
            }
            return false;
        }

        public static bool GetAxis(ref float __result, eJoystickKey _1BBA85C4E)
        {
            __result = 0;

            switch (_1BBA85C4E)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kLeftStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Left).axis.x;
                    break;
                case eJoystickKey.kLeftStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Left).axis.y;
                    break;
                case eJoystickKey.kRightStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Right).axis.x;
                    break;
                case eJoystickKey.kRightStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(VRInput.Hand.Right).axis.y;
                    break;
                // are these used here?
                case eJoystickKey.kR1:
                    break;
                case eJoystickKey.kR2:
                    break;
                case eJoystickKey.kL1:
                    break;
                case eJoystickKey.kL2:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
