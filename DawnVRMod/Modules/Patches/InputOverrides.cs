using DawnVR.Modules.VR;
using UnityEngine;
using System.Linq;
using Valve.VR;
#if !REMASTER
using InputManager = T_6FCAE66C;
using KeyBinding = T_9005A419;
using JoystickInputManager = T_D9E8342E;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        // used for unlocking UIRenderer from the headset
        private const float delayBetweenPresses = 0.1f;
        private static bool pressedFirstTime = false;
        private static float lastPressedTime;
        private static eInputState currentState;

        public static void UpdateJournalInput()
        {
            if (pressedFirstTime && Time.time - lastPressedTime > delayBetweenPresses)
            {
                currentState = eInputState.kDown;
                pressedFirstTime = false;
            }
        }

        public static void ManagerInit(InputManager __instance) => __instance.SetFieldValue("m_overrideType", eControlType.kXboxOne);

        public static bool GetInputState_Enum(InputManager __instance, ref eInputState __result, eGameInput __0)
        {  
            if (__instance.InputBlocked)
            {
                __result = eInputState.kNone;
                return false;
            }

            if (__0 == eGameInput.kAny)
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
            else if (__instance.m_keyBindings.ContainsKey((int)__0))
            {
                KeyBinding keyBinding = __instance.m_keyBindings[(int)__0];

                for (int i = 0; i < keyBinding.m_joystick.Count; i++)
                {
                    eInputState buttonState = GetButtonState(keyBinding.m_joystick[i]);
                    if (buttonState != eInputState.kNone)
                        __result = buttonState;
                }

                return false;
            }

            __result = eInputState.kNone;
            return false;
        }

        public static eInputState GetButtonState(eJoystickKey key)
        {
            switch (key)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kStart:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                case eJoystickKey.kSelect:
                    var inputState = VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                    if (inputState == eInputState.kDown)
                    {
                        currentState = eInputState.kNone;
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

                    eInputState tempState = currentState;
                    if (currentState == eInputState.kDown)
                        currentState = eInputState.kNone;
                    return tempState;
                case eJoystickKey.kR1:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Right));
                case eJoystickKey.kR2:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Right));
                case eJoystickKey.kR3:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Right));
                case eJoystickKey.kL1:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetTrigger(VRInput.Hand.Left));
                case eJoystickKey.kL2:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetGrip(VRInput.Hand.Left));
                case eJoystickKey.kL3:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonThumbstick(VRInput.Hand.Left));
                case eJoystickKey.kAction1:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonX());
                case eJoystickKey.kAction2:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonY());
#if REMASTER
                case eJoystickKey.kMenuBack:
#endif
                case eJoystickKey.kAction3:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonB());
#if REMASTER
                case eJoystickKey.kMenuSelect:
#endif
                case eJoystickKey.kAction4:
                    return VRInput.GetBooleanInputState(VRRig.Instance.Input.GetButtonA());
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

            return eInputState.kNone;
        }

        public static bool GetAxis(ref float __result, eJoystickKey __0)
        {
            __result = 0;

            switch (__0)
            {
                case eJoystickKey.kNone:
                    break;
                case eJoystickKey.kLeftStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(Preferences.MovementThumbstick.Value).axis.x;
                    break;
                case eJoystickKey.kLeftStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(Preferences.MovementThumbstick.Value).axis.y;
                    break;
                case eJoystickKey.kRightStickX:
                    __result = VRRig.Instance.Input.GetThumbstickVector(Preferences.MovementThumbstick.Value).axis.x;
                    break;
                case eJoystickKey.kRightStickY:
                    __result = VRRig.Instance.Input.GetThumbstickVector(Preferences.MovementThumbstick.Value).axis.y;
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
