﻿using DawnVR.Modules.VR;
using UnityEngine;

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private const float speedModifier = 0.05f;
        private const float sprintModifier = 0.08f;

        public static bool CharControllerMove(T_C3DD66D9 __instance, bool _1AF4345B4)
        {
            if (_1AF4345B4)
                __instance.Rotate();
            if (__instance.m_moveDirection != Vector3.zero)
            {
                Vector3 axis = T_A6E913D1.Instance.m_inputManager.GetAxisVector3(eGameInput.kMovementXPositive, eGameInput.kNone, eGameInput.kMovementYPositive);
                float modifier = T_A6E913D1.Instance.m_inputManager.GetAxisAndKeyValue(eGameInput.kJog) == 1 ? sprintModifier : speedModifier;
                try { __instance.m_navAgent.Move(__instance._11C77E995 * axis * modifier); }
                catch { }
            }
            return false;
        }

        public static bool CalculateCharAngle(T_C3DD66D9 __instance, Vector3 _13F806F29)
        {
            __instance._11C77E995 = Quaternion.Euler(0, VRRig.Instance.Camera.transform.eulerAngles.y, 0);
            if (_13F806F29 != __instance.m_moveDirection)
            {
                __instance.m_moveDirection = (__instance.m_nonNormalMoveDirection = _13F806F29);
                __instance.m_moveDirection.Normalize();
                __instance._15B7EF7A4 = Vector3.Angle(Vector3.forward, __instance.m_moveDirection);
                if (_13F806F29.x < 0f)
                    __instance._15B7EF7A4 = 360f - __instance._15B7EF7A4;
            }
            return false;
        }

        public static void SetCameraPosition(Camera _13A97A3A2, Vector3 _1ACF98885)
        {
            if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
            {
                VRRig.Instance.transform.position = _1ACF98885 - Vector3.up;
                Vector3 rot = _13A97A3A2.transform.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                VRRig.Instance.transform.eulerAngles = rot;
            }
        }
    }
}