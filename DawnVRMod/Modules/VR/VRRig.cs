using System.Linq;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRRig : MonoBehaviour
    {
        public static VRRig Instance;

		public Renderer[] ActiveHandRenderers;
		public T_C3DD66D9 ChloeComponent;
		public VRCamera Camera;
        public VRInput Input;
		public VRCalibration Calibrator;
		public VRCutsceneHandler CutsceneHandler;

		private Renderer[] chloeHandRenderers;
		private Renderer[] maxHandRenderers;
		private float heightOffset;
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
                    Camera.ResetHolderPosition();
					SetMeshActive(false);
					T_A6E913D1.Instance.m_followCamera.m_isInteractionBlocked = false;
                    T_F8FE3E1C.s_hideUI = false;
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
                    transform.localPosition = new Vector3(0f, heightOffset, 0f);

				transform.eulerAngles = Vector3.zero;
            }
        }

		public void SetHeightOffset(float offset)
        {
			heightOffset = offset;
			Vector3 position = transform.position;
			position.y += heightOffset;
			transform.position = position;
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
				ChangeHandShader(active ? Resources.DitherShader : Shader.Find("Custom/LISCharacterDeferred"));
			}
		}
    }
}