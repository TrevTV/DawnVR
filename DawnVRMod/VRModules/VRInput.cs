using System;
using Valve.VR;
using UnityEngine;

namespace DawnVR
{
    // This class is a large mess, but in a nutshell, it's a fancy wrapper for SteamVR_Action
    internal class VRInput
    {
        public LeftControllerData LeftController => lController;
        public RightControllerData RightController => rController;

        public VRInput()
        {
            lController = new LeftControllerData();
            rController = new RightControllerData();
        }

        private LeftControllerData lController;
        private RightControllerData rController;

        internal class Thumbstick
        {
            public Vector2 Axis => thumbAxis.GetAxis(thumbSource);
            public bool IsPointingUp => thumbUp.GetStateDown(thumbSource);
            public bool IsPointingDown => thumbDown.GetStateDown(thumbSource);
            public bool IsPointingLeft => thumbLeft.GetStateDown(thumbSource);
            public bool IsPointingRight => thumbRight.GetStateDown(thumbSource);

            public Thumbstick(SteamVR_Input_Sources src,
                SteamVR_Action_Boolean u,
                SteamVR_Action_Boolean d,
                SteamVR_Action_Boolean l,
                SteamVR_Action_Boolean r,
                SteamVR_Action_Vector2 axis)
            {
                thumbSource = src;
                thumbUp = u;
                thumbDown = d;
                thumbLeft = l;
                thumbRight = r;
                thumbAxis = axis;
            }

            private SteamVR_Input_Sources thumbSource;
            private SteamVR_Action_Boolean thumbUp;
            private SteamVR_Action_Boolean thumbDown;
            private SteamVR_Action_Boolean thumbLeft;
            private SteamVR_Action_Boolean thumbRight;
            private SteamVR_Action_Vector2 thumbAxis;
        }

        internal class LeftControllerData
        {
            public Thumbstick Thumbstick;

            public bool IsButtonXDown => SteamVR_Actions._default.Bool_ButtonX.GetStateDown(inputSource);
            public bool IsButtonYDown => SteamVR_Actions._default.Bool_ButtonY.GetStateDown(inputSource);
            public bool IsTriggerDown => SteamVR_Actions._default.Bool_LTrigger.GetStateDown(inputSource);
            public bool IsGripDown => SteamVR_Actions._default.Bool_LTrigger.GetStateDown(inputSource);
            public float TriggerState => SteamVR_Actions._default.V1_LTrigger.GetAxis(inputSource);
            public float GripState => SteamVR_Actions._default.V1_LGrip.GetAxis(inputSource);

            public LeftControllerData() => Thumbstick = new Thumbstick(
                inputSource,
                SteamVR_Actions._default.Bool_LThumbstickUp,
                SteamVR_Actions._default.Bool_LThumbstickDown,
                SteamVR_Actions._default.Bool_LThumbstickLeft,
                SteamVR_Actions._default.Bool_LThumbstickRight,
                SteamVR_Actions._default.V2_LeftThumbstick);

            /// <summary>
            /// Trigger the haptics at a certain time for a certain length
            /// </summary>
            /// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
            /// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
            /// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
            /// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
            public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude)
                => SteamVR_Actions._default.Haptic.Execute(secondsFromNow, durationSeconds, frequency, amplitude, inputSource);

            private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.LeftHand;
        }

        internal class RightControllerData
        {
            public Thumbstick Thumbstick;

            public bool IsButtonADown => SteamVR_Actions._default.Bool_ButtonA.GetStateDown(inputSource);
            public bool IsButtonBDown => SteamVR_Actions._default.Bool_ButtonB.GetStateDown(inputSource);
            public bool IsTriggerDown => SteamVR_Actions._default.Bool_RTrigger.GetStateDown(inputSource);
            public bool IsGripDown => SteamVR_Actions._default.Bool_RTrigger.GetStateDown(inputSource);
            public float TriggerState => SteamVR_Actions._default.V1_RTrigger.GetAxis(inputSource);
            public float GripState => SteamVR_Actions._default.V1_RGrip.GetAxis(inputSource);

            public RightControllerData() => Thumbstick = new Thumbstick(
                inputSource,
                SteamVR_Actions._default.Bool_RThumbstickUp,
                SteamVR_Actions._default.Bool_RThumbstickDown,
                SteamVR_Actions._default.Bool_RThumbstickLeft,
                SteamVR_Actions._default.Bool_RThumbstickRight,
                SteamVR_Actions._default.V2_RightThumbstick);

            /// <summary>
            /// Trigger the haptics at a certain time for a certain length
            /// </summary>
            /// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
            /// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
            /// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
            /// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
            public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude)
                => SteamVR_Actions._default.Haptic.Execute(secondsFromNow, durationSeconds, frequency, amplitude, inputSource);

            private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.RightHand;
        }
    }
}
