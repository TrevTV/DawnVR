using System.Linq;
using UnityEngine;
#if !REMASTER
using CharController = T_C3DD66D9;
using UI3DCamera = T_D4EA31BB;
using DawnMainCamera = T_34182F31;
using GameMaster = T_A6E913D1;
using FreeRoamWindow = T_F8FE3E1C;
#endif

namespace DawnVR.Modules.VR
{
    internal class VRRig : MonoBehaviour
    {
#if REMASTER
		public VRRig(System.IntPtr ptr) : base(ptr) { }
#endif

		public static VRRig Instance;

		public Renderer[] ActiveHandRenderers;
		public CharController ChloeComponent;
		public VRCamera Camera;
        public VRInput Input;
		public VRCalibration Calibrator;
		public VRCutsceneHandler CutsceneHandler;

		private Renderer[] chloeHandRenderers;
		private Renderer[] maxHandRenderers;
		private Vector3 lastPos;

		private GameObject chloeReference;
		private GameObject vrRigReference;

		private void Start()
        {
            DontDestroyOnLoad(gameObject);
			Transform cameraHolder = transform.Find("CameraHolder");
			Camera = cameraHolder.Find("Camera").gameObject.AddComponent<VRCamera>();
			Calibrator = gameObject.AddComponent<VRCalibration>();
			CutsceneHandler = gameObject.AddComponent<VRCutsceneHandler>();
            Input = new VRInput();

			SetupHandposes(cameraHolder);

			chloeHandRenderers = new Renderer[]
			{
				cameraHolder.Find("Controller (left)/CustomModel (Chloe)").GetComponentInChildren<SkinnedMeshRenderer>(),
				cameraHolder.Find("Controller (left)/CustomModel (Chloe)/RachelBracelet").GetComponent<MeshRenderer>(),
				cameraHolder.Find("Controller (right)/CustomModel (Chloe)").GetComponentInChildren<SkinnedMeshRenderer>()
			};

			maxHandRenderers = new Renderer[]
			{
				cameraHolder.Find("Controller (left)/CustomModel (Max)").GetComponentInChildren<SkinnedMeshRenderer>(),
				cameraHolder.Find("Controller (right)/CustomModel (Max)").GetComponentInChildren<SkinnedMeshRenderer>()
			};

			ActiveHandRenderers = chloeHandRenderers;

			if (Preferences.UseSmoothTurning.Value)
				Input.GetThumbstickVector(VRInput.GetOtherHand(Preferences.MovementThumbstick.Value)).onAxis += OnThumbstickAxis;
            else if (Preferences.UseSnapTurning.Value)
            {
				Input.GetThumbstickLeft(VRInput.GetOtherHand(Preferences.MovementThumbstick.Value)).onStateDown += OnThumbstickLeft;
				Input.GetThumbstickRight(VRInput.GetOtherHand(Preferences.MovementThumbstick.Value)).onStateDown += OnThumbstickRight;
			}

			if (Preferences.EnablePlayerCollisionVisualization.Value)
            {
                chloeReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                chloeReference.GetComponent<Collider>().enabled = false;
                chloeReference.transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
                chloeReference.GetComponent<MeshRenderer>().material.color = Color.red;
                GameObject.DontDestroyOnLoad(chloeReference);

                vrRigReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                vrRigReference.GetComponent<Collider>().enabled = false;
                vrRigReference.transform.localScale *= 0.15f;
                vrRigReference.GetComponent<MeshRenderer>().material.color = Color.blue;
                GameObject.DontDestroyOnLoad(vrRigReference);
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
				ObfuscationTools.SetFieldValue(ChloeComponent, "m_targetRot", transform.rotation);
			}
			catch { }
		}

		private void OnThumbstickLeft(Valve.VR.SteamVR_Action_Boolean fromAction, Valve.VR.SteamVR_Input_Sources fromSource)
		{
			try
            {
				Camera.Holder.RotateAround(Camera.transform.position, Vector3.up, -Preferences.SnapTurnAngle.Value);
				ObfuscationTools.SetFieldValue(ChloeComponent, "m_targetRot", transform.rotation);
			}
			catch { }
		}

		private void OnThumbstickRight(Valve.VR.SteamVR_Action_Boolean fromAction, Valve.VR.SteamVR_Input_Sources fromSource)
		{
			try
            {
				Camera.Holder.RotateAround(Camera.transform.position, Vector3.up, Preferences.SnapTurnAngle.Value);
				ObfuscationTools.SetFieldValue(ChloeComponent, "m_targetRot", transform.rotation);
			}
			catch { }
		}

		#endregion

		private void FixedUpdate()
        {
			// todo: collision is still somewhat jank
			// somewhat related to https://github.com/TrevTV/DawnVR/issues/2
			if (ChloeComponent != null && transform.parent == ChloeComponent.m_rotateTrans && ChloeComponent.m_navAgent.enabled)
            {
                Vector3 offset = Camera.transform.localPosition - lastPos;
				offset = Camera.Holder.localRotation * offset;
				offset.y = 0;
                ChloeComponent.m_rotateTrans.localPosition += offset;
                Camera.Holder.localPosition -= offset;
                lastPos = Camera.transform.localPosition;
            }

			if (Preferences.EnablePlayerCollisionVisualization.Value)
            {
                chloeReference.transform.position = ChloeComponent?.m_rotateTrans?.position ?? Vector3.zero;
                vrRigReference.transform.position = transform.position;
            }
		}

        public void UpdateRigParent(eGameMode gameMode)
        {
			// prevents double renders of ui elements, both in headset, and on screen
			if (UI3DCamera.s_ui3DCamera?.m_camera != null)
            {
				UI3DCamera.s_ui3DCamera.m_camera.stereoTargetEye = StereoTargetEyeMask.None;
				UI3DCamera.s_ui3DCamera.m_camera.targetDisplay = 10;
			}
			// disable unused camera, improves performance
			DawnMainCamera.main.enabled = false;
			// backup if setting enabled fails for some reason
			DawnMainCamera.main.depth = -100;
			DawnMainCamera.main.stereoTargetEye = StereoTargetEyeMask.None;

			int currentEpisode = GameMaster.Instance?.m_gameDataManager?.currentEpisodeNumber ?? -1;
			if (currentEpisode == 4) ChangeHandModel(maxHandRenderers);
			else ChangeHandModel(chloeHandRenderers);

			Camera.BlockVision(false);

			switch (gameMode)
			{
				case eGameMode.kCutscene:
					SetMeshActive(true);
					CutsceneHandler.SetupCutscene();
                    break;
				case eGameMode.kPosterView:
					Camera.BlockVision(true);
					break;
				case eGameMode.kDialog:
				case eGameMode.kCustomization:
                    CutsceneHandler.SetupCutscene();
                    ChloeComponent.Camera.enabled = true;
                    break;
				case eGameMode.kFreeRoam:
					CutsceneHandler.EndCutscene();
					SetParent(ChloeComponent.transform);

					// not that pretty but i don't care
					Transform possiblyMeshParent = CharController.s_mainCharacterMesh?.transform?.parent;
					if (possiblyMeshParent != null)
                    {
						foreach (Transform child in possiblyMeshParent)
                        {
							if (child.name.Contains("Bracelet") && child.gameObject.activeInHierarchy)
								chloeHandRenderers[1].gameObject.SetActive(true);
							else
								chloeHandRenderers[1].gameObject.SetActive(false);
						}
					}

					Camera.ResetHolderPosition();
					SetMeshActive(false);
					GameMaster.Instance.m_followCamera.m_isInteractionBlocked = false;
					FreeRoamWindow.s_hideUI = false;
                    break;
				case eGameMode.kLoading:
					CutsceneHandler.EndCutscene();
					break;
				case eGameMode.kMainMenu:
					CutsceneHandler.EndCutscene();
					Camera.BlockVision(true);
					ChangeHandModel(chloeHandRenderers);
					ChangeHandShader(Resources.DitherShader);
					transform.rotation = Quaternion.Euler(0, 85, 0);
					transform.position = new Vector3(-29.2f, -27.3f, -68.9f);
					Camera.BlockVision(false);
					break;
				case eGameMode.kNone:
					CutsceneHandler.EndCutscene();
					SetParent(null);
					break;
				case eGameMode.kVideo:
					CutsceneHandler.EndCutscene();
					// mainly handled in CutsceneFixes::OnMovieWillRenderObject
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
                    transform.localPosition = new Vector3(0f, Calibrator.HeightOffset, 0f);

				transform.eulerAngles = Vector3.zero;
            }
        }

		public void SetHeightOffset(float offset)
        {
			Vector3 position = transform.position;
			position.y += offset;
			transform.position = position;
		}

		public void UpdateCachedChloe(CharController newChloe, bool updateParent = true)
        {
            ChloeComponent = newChloe;
            if (updateParent)
                UpdateRigParent(GameMaster.Instance.m_gameModeManager.CurrentMode);
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
					if (!sMesh.transform.parent.name.StartsWith("CustomModel"))
						sMesh.enabled = active;
				ChangeHandShader(active ? Resources.DitherShader : Shader.Find("Custom/LISCharacterDeferred"));
			}
		}
    }
}