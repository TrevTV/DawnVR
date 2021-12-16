using DawnVR.Modules.VR;
using UnityEngine;
using UnityEngine.AI;

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
				bool isOnNavMesh = IsAgentOnNavMesh(__instance.gameObject);
				if (isOnNavMesh) 
                    __instance.m_navAgent.Move(__instance._11C77E995 * axis * modifier);
            }
            return false;
        }

        public static bool CharControllerUpdate(T_C3DD66D9 __instance)
        {
			if (!__instance._1291EFDCF)
			{
				__instance._1291EFDCF = true;
				if (!__instance.CurrentAnimSet.IsValid())
				{
					__instance.SetupAnimSet(__instance.CurrentAnimSet);
				}
			}
			if (T_A6E913D1.Instance == null || T_A6E913D1.Instance.Paused)
				return false;

			if (__instance.m_hasValidNavmeshCast)
			{
				if (T_A6E913D1.Instance.m_followCamera.isFreeroamStart)
				{
					__instance.transform.position = Vector3.Lerp(__instance.m_preNavPositoin, __instance.m_postNavPosition, T_A6E913D1.Instance.m_followCamera.currentTransitionLerp);
					__instance.m_navAgent.enabled = false;
				}
				else
				{
					__instance.m_navAgent.enabled = true;
					__instance.m_hasValidNavmeshCast = false;
					__instance.m_navAgent.Warp(__instance.m_postNavPosition);
				}
			}
			else if (!__instance.m_navAgent.enabled && (T_A6E913D1.Instance.m_followCamera == null || !T_A6E913D1.Instance.m_followCamera.m_isFreelook))
				__instance.m_navAgent.enabled = true;
			if (__instance._1CC7DCCB6 != __instance.transform.position)
			{
				__instance.m_dissonantMovement = __instance.transform.position - __instance._1CC7DCCB6;
				__instance.m_dissonantMovement.y = 0f;
				__instance._1CC7DCCB6 = __instance.transform.position;
				if (!__instance.m_pushAnim && __instance.m_dissonantMovement.sqrMagnitude * Time.deltaTime > 0.00012f)
				{
					/*Debug.Log(string.Concat(new object[]
					{
					"m_dissonantMovement = ",
					__instance.m_dissonantMovement.sqrMagnitude * Time.deltaTime,
					"   N: ",
					__instance.m_dissonantMovement.normalized
					}));*/
				}
			}
			else
				__instance.m_dissonantMovement = Vector3.zero;

			if (__instance._1EEB2C6A8)
			{
				for (int i = 1; i <= 5; i++)
				{
					if (__instance.IsAnimValid((T_7C97EEE2.eCharMoveAnim)i))
					{
						if (__instance.m_animStates[i].weight > 0f)
							break;

						if (i == 5)
						{
							__instance._1EEB2C6A8 = false;
							for (int j = 1; j <= 5; j++)
								__instance._1643AD4F4[j] = 0f;
						}
					}
				}
			}

			if (!__instance.Active)
				return false;

			Vector3 vector = Vector3.zero;
			float num = 0f;
			if (T_C3DD66D9.CanWalk && !__instance._1EEB2C6A8)
			{
				vector = T_A6E913D1.Instance.m_inputManager.GetAxisVector3(eGameInput.kMovementXPositive, eGameInput.kNone, eGameInput.kMovementYPositive);
				num = T_A6E913D1.Instance.m_inputManager.GetAxisAndKeyValue(eGameInput.kJog, 1f, 1f, 1f);
			}
			if (__instance.m_activeBehavior != null)
			{
				if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
					return false;
				if (vector.sqrMagnitude <= 0.001f)
					return false;
				__instance.StartNewBehavior(null);
			}

			if (vector.sqrMagnitude >= 0.001f)
				__instance.CalculateAngle(vector);

			float num2 = 1f;
			float d = 0.1f;
			NavMeshHit navMeshHit;
			if (vector.sqrMagnitude > 0.001f && IsAgentOnNavMesh(__instance.gameObject) && __instance.m_navAgent.Raycast(__instance.transform.position + __instance._11C77E995 * (Vector3.forward * d), out navMeshHit))
			{
				Vector3 to = navMeshHit.normal;
				float num3 = Vector3.Angle(__instance._11C77E995 * -Vector3.forward, to);
				Quaternion rotation = Quaternion.Euler(0f, __instance._11C77E995.eulerAngles.y + 45f, 0f);
				if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
				{
					Vector3 normal = navMeshHit.normal;
					rotation = Quaternion.Euler(0f, __instance._11C77E995.eulerAngles.y - 45f, 0f);
					if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
						to = (normal + navMeshHit.normal).normalized;
				}

				bool flag = false;
				rotation = Quaternion.Euler(0f, __instance._11C77E995.eulerAngles.y + 90f, 0f);
				if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
				{
					rotation = Quaternion.Euler(0f, __instance._11C77E995.eulerAngles.y - 90f, 0f);
					if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
						flag = true;
				}

				num3 = Mathf.Min(num3, Vector3.Angle(__instance._11C77E995 * -Vector3.forward, to));
				if (num3 < 15f)
					num2 = 0f;
				else if (flag && num3 < 45f)
					num2 = 0f;
				else if (num3 < 90f)
					num2 = Mathf.Lerp(0.01f, 1f, (num3 - 15f) / 70f);
				else
					num2 = 1f;
			}
			if (vector.sqrMagnitude > 0.001f)
			{
				if (num2 < __instance._18001B5A7)
				{
					if (Mathf.Abs(num2 - __instance._18001B5A7) <= __instance._1114BD98 * Time.deltaTime * 2f)
						__instance._18001B5A7 = num2;
					else
						__instance._18001B5A7 -= __instance._1114BD98 * Time.deltaTime * 2f;
				}
				else if (num2 > __instance._18001B5A7)
				{
					if (Mathf.Abs(num2 - __instance._18001B5A7) <= __instance._1114BD98 * Time.deltaTime)
						__instance._18001B5A7 = num2;
					else
						__instance._18001B5A7 += __instance._1114BD98 * Time.deltaTime;
				}
				__instance._18001B5A7 = Mathf.Clamp01(__instance._18001B5A7);
			}
			vector *= __instance._18001B5A7;
			if (__instance._18001B5A7 < 1f)
				num = 0f;
			if (__instance.m_boundaryAnim)
			{
				__instance.m_rotateTrans.eulerAngles = __instance._11C77E995.eulerAngles;
				__instance.SetupBoundaryAnim();
				__instance.UpdateBlendTriggered();
			}
			else if (__instance.m_pushAnim)
			{
				__instance.SetupPushAnim();
				__instance.UpdatePushBlend();
			}
			else if (__instance.m_objectiveReminderAnimLock)
				__instance._13BF81CD6(vector);
			else if (vector.sqrMagnitude < 0.001f)
			{
				__instance.ResetAngle();
				__instance.SetupIdle();
				__instance.UpdateBlendIdle();
				__instance._1E76533D = true;
				__instance._1EEB2C6A8 = true;
				__instance._1FBC365A6(true);
			}
			else
			{
				if (__instance._1E76533D)
				{
					__instance._1291CAF40 = 0f;
					__instance.m_targetProgression = 0f;
					__instance._1E76533D = false;
				}
				__instance.SetupNewAnim(vector, num, __instance._18001B5A7);
				__instance.UpdateBlend();
				__instance._1FBC365A6(false);
			}
			__instance.m_trigger = num;
			for (int k = 0; k < __instance._1643AD4F4.Length; k++)
			{
				if (k < __instance.m_animStates.Length && __instance.m_animStates[k] != null)
					__instance._1643AD4F4[k] = __instance.m_animStates[k].time;
				else
					__instance._1643AD4F4[k] = 0f;
			}
			if (__instance.m_updateDebug)
			{
				for (int l = 0; l < __instance.m_animStates.Length; l++)
				{
					__instance.m_weights[l] = __instance.m_animStates[l].weight;
					__instance.m_times[l] = __instance.m_animStates[l].time;
					__instance.m_enabled[l] = __instance.m_animStates[l].enabled;
				}
			}
			if (__instance._1C5FA2AD9.HasProp())
			{
				if (__instance._1A9B9E85D != null)
					__instance._1A9B9E85D.time = __instance._19C474D6C.time;
				if (__instance._12DAD274 != null)
					__instance._12DAD274.time = __instance._13D29F389.time;
				for (int m = 0; m < __instance._1DB2E60A3.Length; m++)
					if (__instance._1DB2E60A3[m] != null && __instance.m_animStates[m] != null && __instance.m_animStates[m].enabled)
						__instance._1DB2E60A3[m].time = __instance.m_animStates[m].time;
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

		public static void AddVRCalibrationButton(T_51AF6A60 __instance)
		{
			if (__instance.transform.parent.name == "MainMenu")
			{
				GameObject newButton = GameObject.Instantiate(__instance.transform.Find("Buttons/ExitGame").gameObject);
				newButton.transform.parent = __instance.transform.Find("Buttons");
				newButton.name = "VRCalibration";
				newButton.GetComponent<T_A243E23D>().text = "VR Calibration";
				T_9E2FDFCF button = newButton.GetComponent<T_9E2FDFCF>();
				__instance.AddButton(button);
				button.m_callback = new T_B72FD206(VRRig.Instance, nameof(VRRig.BeginCalibration));
			}
		}

		// taken from https://stackoverflow.com/questions/45416515/check-if-disabled-navmesh-agent-player-is-on-navmesh
		public static bool IsAgentOnNavMesh(GameObject agentObject)
        {
            Vector3 agentPosition = agentObject.transform.position;
            NavMeshHit hit;

            // Check for nearest point on navmesh to agent, within onMeshThreshold
            if (NavMesh.SamplePosition(agentPosition, out hit, 0.025f, NavMesh.AllAreas))
            {
                // Check if the positions are vertically aligned
                if (Mathf.Approximately(agentPosition.x, hit.position.x)
                    && Mathf.Approximately(agentPosition.z, hit.position.z))
                {
                    // Lastly, check if object is below navmesh
                    return agentPosition.y >= hit.position.y;
                }
            }

            return false;
        }
    }
}
