using System.Linq;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRRig : MonoBehaviour
    {
        public static VRRig Instance;

		public Renderer[] ActiveHandRenderers;
		public T_C3DD66D9 ChloeComponent;
		public Material ChloeMaterial;
		public VRCamera Camera;
        public VRInput Input;
		public VRCutsceneHandler CutsceneHandler;

		private Renderer[] chloeHandRenderers;
		private Renderer[] maxHandRenderers;
		private Transform handpadTransform;
		private Vector3 lastPos;

		private void Start()
        {
            DontDestroyOnLoad(gameObject);
			Transform cameraHolder = transform.Find("CameraHolder");
			Camera = cameraHolder.Find("Camera").gameObject.AddComponent<VRCamera>();
			CutsceneHandler = gameObject.AddComponent<VRCutsceneHandler>();
            Input = new VRInput();

			SetupHandposes(cameraHolder);

			chloeHandRenderers = new Renderer[]
			{
				cameraHolder.Find("Controller (left)/CustomModel (Chloe)").GetComponentInChildren<SkinnedMeshRenderer>(),
				cameraHolder.Find("Controller (right)/CustomModel (Chloe)").GetComponentInChildren<SkinnedMeshRenderer>()
			};

			maxHandRenderers = new Renderer[]
			{
				cameraHolder.Find("Controller (left)/CustomModel (Max)").GetComponentInChildren<SkinnedMeshRenderer>(),
				cameraHolder.Find("Controller (right)/CustomModel (Max)").GetComponentInChildren<SkinnedMeshRenderer>()
			};

			handpadTransform = chloeHandRenderers[0].transform.parent.Find("handpad");
			ActiveHandRenderers = chloeHandRenderers;

			if (Preferences.UseSmoothTurning.Value)
				Input.GetThumbstickVector(VRInput.Hand.Right).onAxis += OnThumbstickAxis;
            else if (Preferences.UseSnapTurning.Value)
            {
				Input.GetThumbstickLeft(VRInput.Hand.Right).onStateDown += OnThumbstickLeft;
				Input.GetThumbstickRight(VRInput.Hand.Right).onStateDown += OnThumbstickRight;
			}
		}

		private void SetupHandposes(Transform cameraHolder)
        {
			var pose1 = cameraHolder.Find("Controller (left)/CustomModel (Chloe)").gameObject.AddComponent<Handposes.HandposeChanger>();
			var pose2 = cameraHolder.Find("Controller (right)/CustomModel (Chloe)").gameObject.AddComponent<Handposes.HandposeChanger>();
			pose1.source = Valve.VR.SteamVR_Input_Sources.LeftHand;
			pose2.source = Valve.VR.SteamVR_Input_Sources.RightHand;
			pose1.maxPoseJson = Resources.OpenLeft;
			pose1.minPoseJson = Resources.ClosedLeft;
			pose2.maxPoseJson = Resources.OpenRight;
			pose2.minPoseJson = Resources.ClosedRight;
			pose1.BeginSetup();
			pose2.BeginSetup();

			var pose3 = cameraHolder.Find("Controller (left)/CustomModel (Max)").gameObject.AddComponent<Handposes.HandposeChanger>();
			var pose4 = cameraHolder.Find("Controller (right)/CustomModel (Max)").gameObject.AddComponent<Handposes.HandposeChanger>();
			pose3.source = Valve.VR.SteamVR_Input_Sources.LeftHand;
			pose4.source = Valve.VR.SteamVR_Input_Sources.RightHand;
			pose3.maxPoseJson = Resources.OpenLeft;
			pose3.minPoseJson = Resources.ClosedLeft;
			pose4.maxPoseJson = Resources.OpenRight;
			pose4.minPoseJson = Resources.ClosedRight;
			pose3.BeginSetup();
			pose4.BeginSetup();
		}

		#region Turning Stuff

		private void OnThumbstickAxis(Valve.VR.SteamVR_Action_Vector2 fromAction, Valve.VR.SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
		{
			try
            {
				Camera.Holder.RotateAround(Camera.transform.position, Vector3.up, Preferences.SmoothTurnSpeed.Value * axis.x * Time.deltaTime);
				ChloeComponent._11C77E995 = transform.rotation;
			}
			catch { }
		}

		private void OnThumbstickLeft(Valve.VR.SteamVR_Action_Boolean fromAction, Valve.VR.SteamVR_Input_Sources fromSource)
		{
			try
            {
				Camera.Holder.RotateAround(Camera.transform.position, Vector3.up, -Preferences.SnapTurnAngle.Value);
				ChloeComponent._11C77E995 = transform.rotation;
			}
			catch { }
		}

		private void OnThumbstickRight(Valve.VR.SteamVR_Action_Boolean fromAction, Valve.VR.SteamVR_Input_Sources fromSource)
		{
			try
            {
				Camera.Holder.RotateAround(Camera.transform.position, Vector3.up, Preferences.SnapTurnAngle.Value);
				ChloeComponent._11C77E995 = transform.rotation;
			}
			catch { }
		}

		#endregion

		private void Update()
        {
			#region Modified FreeRoamWindow.Update()

			T_F8FE3E1C window = T_E7B3064D.Singleton.GetWindow<T_F8FE3E1C>("FreeRoamWindow");

			if (!window.gameObject.activeInHierarchy) return;

			if (T_F8FE3E1C.s_currentTriggers.Count > 0)
			{
				float num = 180f;
				if (T_F8FE3E1C.s_isFreeLook)
					num = Camera.Component.fieldOfView / 3f;
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
							num3 /= 2;
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
						// angle interact doesnt seem to be imporant so this will just stay commented
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

		private void FixedUpdate()
        {
			// todo: collision is still kinda jank, alec said it may be me needing to add an InverseTransformDirection somewhere
			if (ChloeComponent != null && transform.parent == ChloeComponent.transform)
			{
                Vector3 offset = Camera.transform.localPosition - lastPos;
				offset.y = 0;
				ChloeComponent.transform.localPosition += offset;
				Camera.Holder.localPosition -= offset;
                lastPos = Camera.transform.localPosition;
            }
		}

		public void UpdateRigParent(eGameMode gameMode)
        {
			// prevents double renders of ui elements, both in headset, and on screen
			if (T_D4EA31BB.s_ui3DCamera?.m_camera != null)
            {
				T_D4EA31BB.s_ui3DCamera.m_camera.stereoTargetEye = StereoTargetEyeMask.None;
				T_D4EA31BB.s_ui3DCamera.m_camera.targetDisplay = 10;
            }
			// disable unused camera, improves performance
			T_34182F31.main.enabled = false;

			int currentEpisode = T_A6E913D1.Instance?.m_gameDataManager?.currentEpisodeNumber ?? -1;
			if (currentEpisode == 4) ChangeHandModel(maxHandRenderers);
			else ChangeHandModel(chloeHandRenderers);

			CutsceneHandler.EndCutscene();

			switch (gameMode)
			{
				case eGameMode.kCutscene:
					SetMeshActive(true);
					CutsceneHandler.SetupCutscene();
                    break;
				case eGameMode.kDialog:
				case eGameMode.kPosterView:
				case eGameMode.kCustomization:
					CutsceneHandler.SetupCutscene();
					break;
				case eGameMode.kFreeRoam:
					SetParent(ChloeComponent.transform);
					SetMeshActive(false);
					T_A6E913D1.Instance.m_followCamera.m_isInteractionBlocked = false;
					T_F8FE3E1C.s_hideUI = false;
					break;
				case eGameMode.kLoading:
					break;
				case eGameMode.kMainMenu:
					Camera.BlockVision(true);
					ChangeHandModel(chloeHandRenderers);
					ChangeHandShader(Resources.DitherShader);
					transform.rotation = Quaternion.Euler(0, 85, 0);
					transform.position = new Vector3(-29.2f, -27.3f, -68.9f);
					Camera.BlockVision(false);
					break;
				case eGameMode.kNone:
					SetParent(null);
					break;
				case eGameMode.kVideo:
					break;
			}
		}

        public void SetParent(Transform t, Vector3? newLocalPosition = null, bool resetPos = true)
        {
            transform.parent = t;
            if (resetPos)
            {
                if (newLocalPosition.HasValue)
                    transform.localPosition = newLocalPosition.Value;
                else
                    transform.localPosition = Vector3.zero;

				transform.eulerAngles = Vector3.zero;
            }
        }

		public float floorOffset;
		public float irlHeight;

		public void BeginCalibration()
        {
            // todo: calibration
            /*if (sitStandMode.Equals(PlayerSitStandMode.Seated))
            {
                seatedOffset = irlHeight - player.cam.localPosition.y - floorOffset;
                player.heightOffset = seatedOffset;
                scale = ((player.height) / (irlHeight / 10f));
            }
            else
            {
                floorOffset = irlHeight - player.cam.localPosition.y;
                player.heightOffset = -floorOffset;
                scale = ((player.height) / (irlHeight / 10f));
            }*/
        }

		public void UpdateCachedChloe(T_C3DD66D9 newChloe, bool updateParent = true)
        {
            ChloeComponent = newChloe;
            if (updateParent)
                UpdateRigParent(T_A6E913D1.Instance.m_gameModeManager.CurrentMode);
        }

		public void ChangeHandModel(Renderer[] renderers)
        {
			if (ActiveHandRenderers == renderers) return;

            foreach (Renderer renderer in ActiveHandRenderers)
                renderer.transform.parent.gameObject.SetActive(false);
            ActiveHandRenderers = renderers;
            foreach (Renderer renderer in ActiveHandRenderers)
                renderer.transform.parent.gameObject.SetActive(true);
		}

		public void ChangeHandShader(Shader shader)
        {
			foreach (Renderer renderer in chloeHandRenderers)
				renderer.material.shader = shader;
			foreach (Renderer renderer in maxHandRenderers)
				renderer.material.shader = shader;
		}

		public void SetMeshActive(bool active)
        {
			if (ChloeComponent != null)
            {
				foreach (SkinnedMeshRenderer sMesh in ChloeComponent.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
					if (!chloeHandRenderers.Contains(sMesh) && !maxHandRenderers.Contains(sMesh))
						sMesh.enabled = active;
				if (ChloeMaterial != null)
					ChangeHandShader(active ? Resources.DitherShader : Shader.Find("Custom/LISCharacterDeferred"));
			}
		}
    }
}