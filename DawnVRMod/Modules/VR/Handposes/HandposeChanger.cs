using UnityEngine;
using Valve.VR;
namespace DawnVR.Modules.VR.Handposes
{
    public class HandposeChanger : MonoBehaviour
    {
        // Start is called before the first frame update
        public Transform[] indexBones;
        public Transform[] middleBones;
        public Transform[] ringBones;
        public Transform[] pinkyBones;
        public Transform[] thumbBones;

        private Handpose maxPose;
        private Handpose minPose;
        public string maxPoseJson;
        public string minPoseJson;

        public float indexCurl;
        public float middleCurl;
        public float ringCurl;
        public float pinkyCurl;
        public float thumbCurl;

        public float lerpSpeed = 7f;
        public SteamVR_Input_Sources source;

        private bool isReady;

        public void BeginSetup()
        {
            string prefix = source == SteamVR_Input_Sources.LeftHand ? "Left" : "Right";
            Transform baseTransform = transform.Find(prefix + "ForearmRoll/" + prefix + "Hand");
            for (int i = 0; i < baseTransform.childCount; i++)
            {
                Transform child = baseTransform.GetChild(i);
                Transform[] fingers = child.GetComponentsInChildren<Transform>();

                if (child.name.Contains("Index")) indexBones = fingers;
                else if (child.name.Contains("Middle")) middleBones = fingers;
                else if (child.name.Contains("Ring")) ringBones = fingers;
                else if (child.name.Contains("Pinky")) pinkyBones = fingers;
                else if (child.name.Contains("Thumb")) thumbBones = fingers;
            }

            SetupHandposeFromJson();
        }

        private void SetupHandposeFromJson()
        {
            maxPose = Newtonsoft.Json.JsonConvert.DeserializeObject<Handpose>(maxPoseJson);
            minPose = Newtonsoft.Json.JsonConvert.DeserializeObject<Handpose>(minPoseJson);
            isReady = true;
        }

        private void Update()
        {
            if (!isReady)
                return;

            indexCurl = SteamVR_Actions.default_V1_Trigger[source].axis;
            middleCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            ringCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            pinkyCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            thumbCurl = SteamVR_Actions.default_V2_Thumbstick[source].axis.magnitude > 0.5f ? 1 : 0;

            for (int i = 0; i < indexBones.Length; i++)
                indexBones[i].localRotation = Quaternion.Lerp(indexBones[i].localRotation, Quaternion.Lerp(maxPose.indexRots[i], minPose.indexRots[i], indexCurl), Time.deltaTime * lerpSpeed);

            for (int i = 0; i < middleBones.Length; i++)
                middleBones[i].localRotation = Quaternion.Lerp(middleBones[i].localRotation, Quaternion.Lerp(maxPose.middleRots[i], minPose.middleRots[i], middleCurl), Time.deltaTime * lerpSpeed);

            for (int i = 0; i < ringBones.Length; i++)
                ringBones[i].localRotation = Quaternion.Lerp(ringBones[i].localRotation, Quaternion.Lerp(maxPose.ringRots[i], minPose.ringRots[i], ringCurl), Time.deltaTime * lerpSpeed);

            for (int i = 0; i < pinkyBones.Length; i++)
                pinkyBones[i].localRotation = Quaternion.Lerp(pinkyBones[i].localRotation, Quaternion.Lerp(maxPose.pinkyRots[i], minPose.pinkyRots[i], pinkyCurl), Time.deltaTime * lerpSpeed);

            for (int i = 0; i < thumbBones.Length; i++)
                thumbBones[i].localRotation = Quaternion.Lerp(thumbBones[i].localRotation, Quaternion.Lerp(maxPose.thumbRots[i], minPose.thumbRots[i], thumbCurl), Time.deltaTime * lerpSpeed);
        }
    }
}

