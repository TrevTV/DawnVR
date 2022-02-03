using DawnVR.Modules.VR;
using UnityEngine;
using UnityEngine.AI;
#if !REMASTER
using CharController = T_C3DD66D9;
using GameMaster = T_A6E913D1;
using MathHelpers = T_D3A1C202;
using CharAnimSet = T_7C97EEE2;
using ButtonMenu = T_51AF6A60;
using IdolUiLabel = T_A243E23D;
using ButtonTweener = T_9E2FDFCF;
using EventDelegate = T_B72FD206;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private const float speedModifier = 0.05f;
        private const float sprintModifier = 0.08f;

        public static bool CharControllerMove(CharController __instance, Vector3 _17EEFAD12, bool _1AF4345B4)
        {
			if (VRRig.Instance.Input.IsUsingViveWand && VRRig.Instance.Input.GetGrip(VRInput.Hand.Left).state)
				return false;

			if (_1AF4345B4)
                __instance.Rotate();

            Vector3 vector;
            if (__instance.GetFieldValue<CharController.CurrentAnimSetInfo>("m_CurrentAnimSetInfo").animSet.m_motionData[__instance.m_baseAnim] != null)
                vector = __instance.GetFieldValue<CharController.CurrentAnimSetInfo>("m_CurrentAnimSetInfo").animSet.m_motionData[__instance.m_baseAnim].GetPositionAtTime(__instance.m_animStates[__instance.m_baseAnim].time);
            else
                vector = Vector3.zero;
            _17EEFAD12 += vector - __instance.GetFieldValue<Vector3>("m_prevPos");
			__instance.SetFieldValue("m_prevPos", vector);

            if (__instance.m_moveDirection != Vector3.zero)
            {
				var steamVRAxis = VRRig.Instance.Input.GetThumbstickVector(Preferences.MovementThumbstick.Value).axis;
				Vector3 axis = new Vector3(steamVRAxis.x, 0f, steamVRAxis.y);
                float modifier = GameMaster.Instance.m_inputManager.GetAxisAndKeyValue(eGameInput.kJog) == 1 ? sprintModifier : speedModifier;
				__instance.SetFieldValue("m_previousValidPosition", __instance.m_rotateTrans.position);
				__instance.SetFieldValue("m_priorDesiredPosition", __instance.transform.position + __instance.GetFieldValue<Quaternion>("m_targetRot") * _17EEFAD12);
				__instance.m_navAgent.Move(__instance.GetFieldValue<Quaternion>("m_targetRot") * axis * modifier);
            }
            return false;
        }

		public static bool CharControllerRotate(CharController __instance)
        {
			if (__instance.GetFieldValue<float>("m_targetTurn") != __instance.GetFieldValue<float>("m_currentTurn") && __instance.GetFieldValue<int>("m_turnDir") != 0)
			{
				float num = MathHelpers.MinimumAngle(__instance.GetFieldValue<float>("m_currentTurn"), MathHelpers.NormalizeAngle(__instance.GetFieldValue<float>("m_targetTurn"), 360f), 360f);
				Vector3 zero = Vector3.zero;
				if (Mathf.FloorToInt(__instance.m_currProgression) != 0)
				{
					float rotationVelocity = 0;
					zero.y = Mathf.SmoothDamp(__instance.GetFieldValue<float>("m_currentTurn"), __instance.GetFieldValue<float>("m_currentTurn") + num, ref rotationVelocity, 0.2f) - __instance.GetFieldValue<float>("m_currentTurn");
					__instance.SetFieldValue("rotationVelocity", rotationVelocity);
				}
				else if (Mathf.CeilToInt(__instance.m_currProgression) != 0)
				{
					float rotationVelocity = 0;
					zero.y = Mathf.SmoothDamp(__instance.GetFieldValue<float>("m_currentTurn"), __instance.GetFieldValue<float>("m_currentTurn") + num, ref rotationVelocity, 0.3f) - __instance.GetFieldValue<float>("m_currentTurn");
					__instance.SetFieldValue("rotationVelocity", rotationVelocity);
				}
				__instance.SetFieldValue("m_currentTurn", __instance.GetFieldValue<float>("m_currentTurn") + zero.y);
				__instance.SetFieldValue("m_currentTurn", MathHelpers.NormalizeAngle(__instance.GetFieldValue<float>("m_currentTurn"), 360f));
				//__instance.m_rotateTrans.localEulerAngles = new Vector3(__instance.m_rotateTrans.localEulerAngles.x, __instance._1A5A07929, __instance.m_rotateTrans.localEulerAngles.z);
			}

			return false;
		}

		public static bool CharControllerUpdate(CharController __instance)
        {
			if (!__instance.GetFieldValue<bool>("m_initializedDefaultAnim"))
			{
				__instance.SetFieldValue("m_initializedDefaultAnim", true);
				if (!__instance.CurrentAnimSet.IsValid())
					__instance.SetupAnimSet(__instance.CurrentAnimSet);
			}

			if (GameMaster.Instance == null || GameMaster.Instance.Paused)
				return false;

			if (__instance.m_hasValidNavmeshCast)
			{
				if (GameMaster.Instance.m_followCamera.isFreeroamStart)
				{
					__instance.transform.position = Vector3.Lerp(__instance.m_preNavPositoin, __instance.m_postNavPosition, GameMaster.Instance.m_followCamera.currentTransitionLerp);
					__instance.m_navAgent.enabled = false;
				}
				else
				{
					__instance.m_navAgent.enabled = true;
					__instance.m_hasValidNavmeshCast = false;
					__instance.m_navAgent.Warp(__instance.m_postNavPosition);
				}
			}
			else if (!__instance.m_navAgent.enabled && (GameMaster.Instance.m_followCamera == null || !GameMaster.Instance.m_followCamera.m_isFreelook))
				__instance.m_navAgent.enabled = true;

			if (__instance.GetFieldValue<Vector3>("m_priorDesiredPosition") != __instance.transform.position)
			{
				__instance.m_dissonantMovement = __instance.transform.position - __instance.GetFieldValue<Vector3>("m_priorDesiredPosition");
				Vector3 dm = __instance.m_dissonantMovement;
				dm.y = 0f;
				__instance.m_dissonantMovement = dm;
				__instance.SetFieldValue("m_priorDesiredPosition", __instance.transform.position);
			}
			else
				__instance.m_dissonantMovement = Vector3.zero;

			if (__instance.GetFieldValue<bool>("m_isStillFadingIdle"))
			{
				for (int i = 1; i <= 5; i++)
				{
					if (__instance.IsAnimValid((CharAnimSet.eCharMoveAnim)i))
					{
						if (__instance.m_animStates[i].weight > 0f)
							break;

						if (i == 5)
						{
							__instance.SetFieldValue("m_isStillFadingIdle", false);
							for (int j = 1; j <= 5; j++)
								__instance.GetFieldValue<float[]>("m_previousTimes")[j] = 0f;
						}
					}
				}
			}

			if (!__instance.Active)
				return false;

			Vector3 vector = Vector3.zero;
			float num = 0f;
			if (CharController.CanWalk && !__instance.GetFieldValue<bool>("m_isStillFadingIdle"))
			{
				vector = GameMaster.Instance.m_inputManager.GetAxisVector3(eGameInput.kMovementXPositive, eGameInput.kNone, eGameInput.kMovementYPositive);
				num = GameMaster.Instance.m_inputManager.GetAxisAndKeyValue(eGameInput.kJog, 1f, 1f, 1f);
			}
			if (__instance.m_activeBehavior != null)
			{
				if (GameMaster.Instance.m_gameModeManager.CurrentMode != eGameMode.kFreeRoam)
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
			if (vector.sqrMagnitude > 0.001f && IsAgentOnNavMesh(__instance.gameObject) && __instance.m_navAgent.Raycast(__instance.transform.position + __instance.GetFieldValue<Quaternion>("m_targetRot") * (Vector3.forward * d), out navMeshHit))
			{
				Vector3 to = navMeshHit.normal;
				float num3 = Vector3.Angle(__instance.GetFieldValue<Quaternion>("m_targetRot") * -Vector3.forward, to);
				Quaternion rotation = Quaternion.Euler(0f, __instance.GetFieldValue<Quaternion>("m_targetRot").eulerAngles.y + 45f, 0f);
				if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
				{
					Vector3 normal = navMeshHit.normal;
					rotation = Quaternion.Euler(0f, __instance.GetFieldValue<Quaternion>("m_targetRot").eulerAngles.y - 45f, 0f);
					if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
						to = (normal + navMeshHit.normal).normalized;
				}

				bool flag = false;
				rotation = Quaternion.Euler(0f, __instance.GetFieldValue<Quaternion>("m_targetRot").eulerAngles.y + 90f, 0f);
				if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
				{
					rotation = Quaternion.Euler(0f, __instance.GetFieldValue<Quaternion>("m_targetRot").eulerAngles.y - 90f, 0f);
					if (__instance.m_navAgent.Raycast(__instance.transform.position + rotation * (Vector3.forward * d), out navMeshHit))
						flag = true;
				}

				num3 = Mathf.Min(num3, Vector3.Angle(__instance.GetFieldValue<Quaternion>("m_targetRot") * -Vector3.forward, to));
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
				if (num2 < __instance.GetFieldValue<float>("m_moveMultiplier"))
				{
					if (Mathf.Abs(num2 - __instance.GetFieldValue<float>("m_moveMultiplier")) <= __instance.GetFieldValue<float>("m_moveStopStep") * Time.deltaTime * 2f)
						__instance.SetFieldValue("m_moveMultiplier", num2);
					else
						__instance.SetFieldValue("m_moveMultiplier", __instance.GetFieldValue<float>("m_moveMultiplier") - __instance.GetFieldValue<float>("m_moveStopStep") * Time.deltaTime * 2f);
				}
				else if (num2 > __instance.GetFieldValue<float>("m_moveMultiplier"))
				{
					if (Mathf.Abs(num2 - __instance.GetFieldValue<float>("m_moveMultiplier")) <= __instance.GetFieldValue<float>("m_moveStopStep") * Time.deltaTime)
						__instance.SetFieldValue("m_moveMultiplier", num2);
					else
						__instance.SetFieldValue("m_moveMultiplier", __instance.GetFieldValue<float>("m_moveMultiplier") + __instance.GetFieldValue<float>("m_moveStopStep") * Time.deltaTime);
				}
				__instance.SetFieldValue("m_moveMultiplier", Mathf.Clamp01(__instance.GetFieldValue<float>("m_moveMultiplier")));
			}
			vector *= __instance.GetFieldValue<float>("m_moveMultiplier");
			if (__instance.GetFieldValue<float>("m_moveMultiplier") < 1f)
				num = 0f;
			if (__instance.m_boundaryAnim)
			{
				__instance.m_rotateTrans.eulerAngles = __instance.GetFieldValue<Quaternion>("m_targetRot").eulerAngles;
				__instance.SetupBoundaryAnim();
				__instance.UpdateBlendTriggered();
			}
			else if (__instance.m_pushAnim)
			{
				__instance.SetupPushAnim();
				__instance.UpdatePushBlend();
			}
			else if (__instance.m_objectiveReminderAnimLock)
				__instance.CallMethod("UpdateObjectiveReminderBlend", vector);
			else if (vector.sqrMagnitude < 0.001f)
			{
				__instance.ResetAngle();
				__instance.SetupIdle();
				__instance.UpdateBlendIdle();
				__instance.SetFieldValue("m_firstMoveFrame", true);
				__instance.SetFieldValue("m_isStillFadingIdle", true);
				__instance.CallMethod("UpdateExtrasTimer", true);
			}
			else
			{
				if (__instance.GetFieldValue<bool>("m_firstMoveFrame"))
				{
					__instance.SetFieldValue("m_boundaryProgression", 0f);
					__instance.m_targetProgression = 0f;
					__instance.SetFieldValue("m_firstMoveFrame", false);
				}
				__instance.SetupNewAnim(vector, num, __instance.GetFieldValue<float>("m_moveMultiplier"));
				__instance.UpdateBlend();
				__instance.CallMethod("UpdateExtrasTimer", false);
			}
			__instance.m_trigger = num;
			for (int k = 0; k < __instance.GetFieldValue<float[]>("m_previousTimes").Length; k++)
			{
				if (k < __instance.m_animStates.Length && __instance.m_animStates[k] != null)
					__instance.GetFieldValue<float[]>("m_previousTimes")[k] = __instance.m_animStates[k].time;
				else
					__instance.GetFieldValue<float[]>("m_previousTimes")[k] = 0f;
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
			if (__instance.GetFieldValue<CharController.CurrentAnimSetInfo>("m_CurrentAnimSetInfo").HasProp())
			{
				if (__instance.GetFieldValue<AnimationState>("m_currentPropExtraIdle") != null)
					__instance.GetFieldValue<AnimationState>("m_currentPropExtraIdle").time = __instance.GetFieldValue<AnimationState>("m_currentExtraIdle").time;
				if (__instance.GetFieldValue<AnimationState>("m_currentPropExtraWalk") != null)
					__instance.GetFieldValue<AnimationState>("m_currentPropExtraWalk").time = __instance.GetFieldValue<AnimationState>("m_currentExtraWalk").time;
				for (int m = 0; m < __instance.GetFieldValue<AnimationState[]>("m_propAnimStates").Length; m++)
					if (__instance.GetFieldValue<AnimationState[]>("m_propAnimStates")[m] != null && __instance.m_animStates[m] != null && __instance.m_animStates[m].enabled)
						__instance.GetFieldValue<AnimationState[]>("m_propAnimStates")[m].time = __instance.m_animStates[m].time;
			}

			return false;
		}

        public static bool CalculateCharAngle(CharController __instance, Vector3 __0)
        {
            __instance.SetFieldValue("m_targetRot", Quaternion.Euler(0, VRRig.Instance.Camera.transform.eulerAngles.y, 0));
            if (__0 != __instance.m_moveDirection)
            {
                __instance.m_moveDirection = (__instance.m_nonNormalMoveDirection = __0);
                __instance.m_moveDirection.Normalize();
                __instance.SetFieldValue("m_worldAngle", Vector3.Angle(Vector3.forward, __instance.m_moveDirection));
                if (__0.x < 0f)
                    __instance.SetFieldValue("m_worldAngle", 360f - __instance.GetFieldValue<float>("m_worldAngle"));
            }
            return false;
        }

		public static void AddVRCalibrationButton(ButtonMenu __instance)
		{
			if (__instance.transform.parent.name == "MainMenu")
			{
				GameObject newButton = GameObject.Instantiate(__instance.transform.Find("Buttons/ExitGame").gameObject);
				newButton.transform.parent = __instance.transform.Find("Buttons");
				newButton.name = "VRCalibration";
				newButton.GetComponent<IdolUiLabel>().text = "VR Calibration";
				ButtonTweener button = newButton.GetComponent<ButtonTweener>();
				__instance.AddButton(button);
				button.m_callback = new EventDelegate(VRRig.Instance.Calibrator, nameof(VRCalibration.SetupCalibrator));
			}
		}

		// taken from https://stackoverflow.com/questions/45416515/check-if-disabled-navmesh-agent-player-is-on-navmesh
		public static bool IsAgentOnNavMesh(GameObject agentObject)
        {
			// not very good but eh
#if REMASTER
			return true;
#else
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
#endif
		}
	}
}
