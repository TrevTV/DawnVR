﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Handles aligning audio listener when using speakers.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

namespace Valve.VR
{
    //[RequireComponent(typeof(AudioListener))]
    public class SteamVR_Ears : MonoBehaviour
    {
#if REMASTER
        public SteamVR_Ears(System.IntPtr ptr) : base(ptr) { }
#endif

        public SteamVR_Camera vrcam;
        Quaternion offset;

        private void OnNewPosesApplied()
        {
            var origin = vrcam.origin;
            var baseRotation = origin != null ? origin.rotation : Quaternion.identity;
            transform.rotation = baseRotation * offset;
        }

        void OnEnable()
        {
            var settings = OpenVR.Settings;
            if (settings != null)
            {
                var error = EVRSettingsError.None;
                if (settings.GetBool(OpenVR.k_pch_SteamVR_Section, OpenVR.k_pch_SteamVR_UsingSpeakers_Bool, ref error))
                {
                    var yawOffset = settings.GetFloat(OpenVR.k_pch_SteamVR_Section, OpenVR.k_pch_SteamVR_SpeakersForwardYawOffsetDegrees_Float, ref error);
                    offset = Quaternion.Euler(0.0f, yawOffset, 0.0f);
                }
            }

            /*if (usingSpeakers)
                SteamVR_Events.NewPosesApplied.Listen(OnNewPosesApplied);*/
        }

        void OnDisable()
        {
            /*if (usingSpeakers)
                SteamVR_Events.NewPosesApplied.Remove(OnNewPosesApplied);*/
        }
    }
}