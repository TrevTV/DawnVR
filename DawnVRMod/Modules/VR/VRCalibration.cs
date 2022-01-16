using System.Linq;
using UnityEngine;
using Valve.VR;

namespace DawnVR.Modules.VR
{
	internal class VRCalibration : MonoBehaviour
	{
		private GameObject uiRoot;
		private GameObject irlHeightObj;
		private GameObject calibrateObj;

		private CalibrationStep currentStep;
		private float standingHeight;

		public void SetupCalibrator()
        {
			T_E7B3064D.Singleton.CloseWindow("MainMenuWindow");
			uiRoot = GameObject.Instantiate(Resources.CalibrationUI, new Vector3(-24.1f, -26.5f, -68.9f), Quaternion.Euler(0, 90, 0));
			irlHeightObj = uiRoot.transform.Find("FindRealHeight").gameObject;
			calibrateObj = uiRoot.transform.Find("FinalizeOffset").gameObject;
			currentStep = CalibrationStep.FindIRLHeight;
			irlHeightObj.SetActive(true);
		}

		public void Reset()
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
						T_E7B3064D.Singleton.OpenWindow("MainMenuWindow");
						Reset();
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