using System.Linq;
using UnityEngine;
using Valve.VR;
#if !REMASTER
using CWindowManager = T_E7B3064D;
#endif

namespace DawnVR.Modules.VR
{
	internal class VRCalibration : MonoBehaviour
	{
#if REMASTER
		public VRCalibration(System.IntPtr ptr) : base(ptr) { }
#endif

		private GameObject uiRoot;
		private GameObject irlHeightObj;
		private GameObject calibrateObj;

		private CalibrationStep currentStep;
		private float standingHeight;

		public void SetupCalibrator()
        {
			CWindowManager.Singleton.CloseWindow("MainMenuWindow");
#if REMASTER
			uiRoot = GameObject.Instantiate(Resources.CalibrationUI, new Vector3(-24.1f, -26.5f, -68.9f), Quaternion.Euler(0, 90, 0)).Cast<GameObject>();
#else
			uiRoot = (GameObject)GameObject.Instantiate(Resources.CalibrationUI, new Vector3(-24.1f, -26.5f, -68.9f), Quaternion.Euler(0, 90, 0));
#endif
			irlHeightObj = uiRoot.transform.Find("FindRealHeight").gameObject;
			calibrateObj = uiRoot.transform.Find("FinalizeOffset").gameObject;
			currentStep = CalibrationStep.FindIRLHeight;
			irlHeightObj.SetActive(true);
		}

		public void ResetUI()
        {
			currentStep = CalibrationStep.None;
			Destroy(uiRoot);
        }

		private void Update()
        {
            switch (currentStep)
            {
                case CalibrationStep.FindIRLHeight:
					if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.stateDown))
					{
						standingHeight = VRRig.Instance.Camera.transform.localPosition.y;
						irlHeightObj.SetActive(false);
						calibrateObj.SetActive(true);
						currentStep = CalibrationStep.Calibrate;
					}
					break;
                case CalibrationStep.Calibrate:
					if (SteamVR_Input.actionsBoolean.Any((a) => a != SteamVR_Actions.default_HeadsetOnHead && a.stateDown))
                    {
						VRRig.Instance.SetHeightOffset(standingHeight - VRRig.Instance.Camera.transform.localPosition.y);
						CWindowManager.Singleton.OpenWindow("MainMenuWindow");
						ResetUI();
					}
                    break;
            }
        }

		private enum CalibrationStep
        {
			None,
			FindIRLHeight,
			Calibrate
        }
	}
}