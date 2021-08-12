using System;
using Valve.VR;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    // really is just here to make things look slightly cleaner
    // todo: limit this to just controls that the game needs
    internal class VRInput
    {
        public SteamVR_Action_Boolean GetButtonA() => SteamVR_Actions.default_Bool_ButtonA;
        public SteamVR_Action_Boolean GetButtonB() => SteamVR_Actions.default_Bool_ButtonB;
        public SteamVR_Action_Boolean GetButtonX() => SteamVR_Actions.default_Bool_ButtonX;
        public SteamVR_Action_Boolean GetButtonY() => SteamVR_Actions.default_Bool_ButtonY;

        public SteamVR_Action_Single_Source GetTriggerAxis(Hand h) => SteamVR_Actions.default_V1_Trigger[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetTrigger(Hand h) => SteamVR_Actions.default_Bool_Trigger[GetSourceFromHand(h)];
        public SteamVR_Action_Single_Source GetGripAxis(Hand h) => SteamVR_Actions.default_V1_Grip[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetGrip(Hand h) => SteamVR_Actions.default_Bool_Grip[GetSourceFromHand(h)];

        public SteamVR_Action_Vector2_Source GetThumbstickVector(Hand h) => SteamVR_Actions.default_V2_Thumbstick[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetButtonThumbstick(Hand h) => SteamVR_Actions.default_Bool_ButtonThumbstick[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetThumbstickUp(Hand h) => SteamVR_Actions.default_Bool_ThumbstickUp[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetThumbstickDown(Hand h) => SteamVR_Actions.default_Bool_ThumbstickDown[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetThumbstickLeft(Hand h) => SteamVR_Actions.default_Bool_ThumbstickLeft[GetSourceFromHand(h)];
        public SteamVR_Action_Boolean_Source GetThumbstickRight(Hand h) => SteamVR_Actions.default_Bool_ThumbstickRight[GetSourceFromHand(h)];

        private SteamVR_Input_Sources GetSourceFromHand(Hand hand)
        {
            switch (hand)
            {
                case Hand.Left:
                    return SteamVR_Input_Sources.LeftHand;
                case Hand.Right:
                    return SteamVR_Input_Sources.RightHand;
                case Hand.Any:
                    return SteamVR_Input_Sources.Any;
                default:
                    return SteamVR_Input_Sources.Any;
            }
        }

        public enum Hand
        {
            Left,
            Right,
            Any
        }
    }
}
