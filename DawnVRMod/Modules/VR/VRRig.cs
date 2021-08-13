using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRRig : MonoBehaviour
    {
        public static VRRig Instance;

		public Transform ChloeTransform => cachedChloe?.transform;
		public VRCamera Camera;
        public VRInput Input;

		private float turnSpeed = 4;
		private T_C3DD66D9 cachedChloe;
		private static readonly System.Reflection.FieldInfo CharControl_TargetRot = typeof(T_C3DD66D9).GetField("_11C77E995", HarmonyLib.AccessTools.all);

		private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Camera = transform.Find("Camera").gameObject.AddComponent<VRCamera>();
            Input = new VRInput();

            Input.GetThumbstickVector(VRInput.Hand.Right).onAxis += OnThumbstickAxis;
        }

        private void OnThumbstickAxis(Valve.VR.SteamVR_Action_Vector2 fromAction, Valve.VR.SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
		{
			transform.Rotate(new Vector3(0, axis.x * turnSpeed, 0));
			CharControl_TargetRot.SetValue(cachedChloe, transform.rotation);
		}

        private void Update()
        {
			#region Mostly a copypaste from the FreeRoamWindow but modifed to use the vr cam

			T_F8FE3E1C window = T_E7B3064D.Singleton.GetWindow<T_F8FE3E1C>("FreeRoamWindow");

			if (!window.gameObject.activeInHierarchy) return;

			if (T_F8FE3E1C.s_currentTriggers.Count > 0)
			{
				float num = 180f;
				int triggerIndex = -1;
				float num3 = 1f;
				float d = window.m_interactUI.transform.localScale.x;
				for (int i = 0; i < T_F8FE3E1C.s_currentTriggers.Count; i++)
				{
					if (!(T_F8FE3E1C.s_currentTriggers[i].m_pointAt == null))
					{
						float num4 = Vector3.Distance(T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position, Camera.transform.position);
						if (T_F8FE3E1C.s_isFreeLook)
						{
							d = 1f;
							num3 = (Mathf.Max(0f, num4 - 2f) + 1f) * window.m_distanceScaleFactor * (Camera.Component.fieldOfView / 30f);
							T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num3, num3, 1f);
						}
						else
						{
							float num5 = 1f;
							if (T_F8FE3E1C.s_currentTriggers[i] != null && T_F8FE3E1C.s_currentTriggers[i].m_nameUI != null)
							{
								num5 = (Mathf.Max(0f, num4 - 3f) + 1f) * 0.28f;
								num5 = Mathf.Max(1f, num5);
								num5 = Mathf.Min(num5, 3.5f);
								T_F8FE3E1C.s_currentTriggers[i].m_nameUI.transform.localScale = new Vector3(num5, num5, 1f);
							}
							if (window.m_interactUI.hotSpotObj == T_F8FE3E1C.s_currentTriggers[i])
							{
								d = num5;
							}
						}

						Vector3 vector = T_F8FE3E1C.s_currentTriggers[i].m_pointAt.transform.position - Camera.transform.position;
						float angle = Vector3.Angle(vector, Camera.transform.forward);
						// todo: might be important? leaving a log here for the future
						if (T_F8FE3E1C.s_currentTriggers[i].usesAngleInteract)
							MelonLoader.MelonLogger.Msg(T_F8FE3E1C.s_currentTriggers[i].m_friendlyName + " USES ANGLE INTERACT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
						/*if (T_F8FE3E1C.s_currentTriggers[i].usesAngleInteract)
                        {
                            vector.y = 0f;
							float num7 = Vector3.Angle(-vector, Vector3.forward);
							Vector3 vector2 = Vector3.Cross(vector, Vector3.forward);
							if (vector2.y < 0f)
								num7 *= -1f;

							bool angleValid = false;
							float num0 = T_D3A1C202.NormalizeAngle(T_F8FE3E1C.s_currentTriggers[i].m_LeftInteractAngle, 360f);
							float num2 = T_D3A1C202.NormalizeAngle(T_F8FE3E1C.s_currentTriggers[i].m_RightInteractAngle, 360f);
							num7 = T_D3A1C202.NormalizeAngle(num7, normalizeAngle);
							if (num0 > num2)
							{
								angleValid = true;
								if (num7 < num0 && num7 > num2)
									angleValid = false;
							}
							if (num7 > num0 && num7 < num2)
								angleValid = true;
							if (!angleValid)
								continue;
						}*/

						if (angle < num)
						{
							if (T_F8FE3E1C.s_isFreeLook)
							{
								num = angle;
								triggerIndex = i;
								window.m_interactUI.transform.localScale = new Vector3(num3, num3, 1f);
							}
							else
                            {
								num = angle;
								triggerIndex = i;
								window.m_interactUI.transform.localScale = Vector2.one * d;
							}
						}
					}
				}

				if (triggerIndex >= 0)
					window.ShowInteractUI(triggerIndex);
				else
					window.HideInteractUI();
			}
			else
			{
				window.m_interactUI.ClearHotSpot();
				window.HideInteractUI();
			}

            #endregion
        }

        public void UpdateRigParent(eGameMode gameMode)
        {
			// prevents double renders of ui elements, both in headset, and on screen
			T_D4EA31BB.s_ui3DCamera.m_camera.stereoTargetEye = StereoTargetEyeMask.None;
			T_D4EA31BB.s_ui3DCamera.m_camera.targetDisplay = 10;

			switch (gameMode)
            {
                case eGameMode.kCustomization:
                    throw new System.NotImplementedException();
                case eGameMode.kCutscene:
					// todo: set camera's rendertexture to the one used for the ui to prevent motion sickness
					if (transform.parent == cachedChloe.transform)
						SetParent(null, null, false);
                    break;
                case eGameMode.kDialog:
                    // todo: figure out how to handle this, its when talking with character (ex: talking to david and joyce)
                case eGameMode.kFreeRoam:
                    SetParent(cachedChloe.transform);
                    break;
                case eGameMode.kLoading:
                    break;
                case eGameMode.kMainMenu:
                    transform.rotation = Quaternion.Euler(0, 85, 0);
                    transform.position = ((Camera)typeof(T_C3DD66D9).Assembly.GetType("T_34182F31").GetProperty("main").GetValue(null, null))?.transform.position ?? Vector3.zero;
                    break;
                case eGameMode.kNone:
                    SetParent(null);
                    break;
                case eGameMode.kPosterView:
                    throw new System.NotImplementedException();
                case eGameMode.kVideo:
                    throw new System.NotImplementedException();
            }
        }

        private void SetParent(Transform t, Vector3? newLocalPosition = null, bool resetPos = true)
        {
            transform.parent = t;
            if (resetPos)
            {
                if (newLocalPosition.HasValue)
                    transform.localPosition = newLocalPosition.Value;
                else
                    transform.localPosition = Vector3.zero;
            }
        }

        public void UpdateCachedChloe(T_C3DD66D9 newChloe, bool updateParent = true)
        {
            cachedChloe = newChloe;
            if (updateParent)
                UpdateRigParent(T_A6E913D1.Instance.m_gameModeManager.CurrentMode);
        }
    }
}