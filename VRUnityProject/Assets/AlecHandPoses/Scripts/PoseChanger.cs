using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PoseChanger : MonoBehaviour
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
    [Range(0f, 1f)] public float indexCurl;
    [Range(0f, 1f)]public float middleCurl;
    [Range(0f, 1f)]public float ringCurl;
    [Range(0f, 1f)]public float pinkyCurl;
    [Range(0f, 1f)]public float thumbCurl;
    public float lerpSpeed = 0.5f;
    public SteamVR_Input_Sources source;

    public bool debug = false;
    void Start()
    {
        maxPose = Valve.Newtonsoft.Json.JsonConvert.DeserializeObject<Handpose>(maxPoseJson);
        minPose = Valve.Newtonsoft.Json.JsonConvert.DeserializeObject<Handpose>(minPoseJson);
    }

    // Update is called once per frame
    void Update()
    {
        if (!debug)
        {

            indexCurl = SteamVR_Actions.default_V1_Trigger[source].axis;
            middleCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            ringCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            pinkyCurl = SteamVR_Actions.default_V1_Grip[source].axis;
            thumbCurl = SteamVR_Actions.default_V2_Thumbstick[source].axis.magnitude > 0.5f ? 1 : 0;
        }
        for (int i = 0; i < indexBones.Length; i++)
        {
            indexBones[i].localRotation = Quaternion.Lerp(indexBones[i].localRotation, Quaternion.Lerp(maxPose.indexRots[i], minPose.indexRots[i], indexCurl), Time.deltaTime * lerpSpeed);
        }

        for (int i = 0; i < middleBones.Length; i++)
        {
            middleBones[i].localRotation = Quaternion.Lerp(middleBones[i].localRotation, Quaternion.Lerp(maxPose.middleRots[i], minPose.middleRots[i], middleCurl), Time.deltaTime * lerpSpeed);
        }

        for (int i = 0; i < ringBones.Length; i++)
        {
            ringBones[i].localRotation = Quaternion.Lerp(ringBones[i].localRotation, Quaternion.Lerp(maxPose.ringRots[i], minPose.ringRots[i], ringCurl), Time.deltaTime * lerpSpeed);
        }

        for (int i = 0; i < pinkyBones.Length; i++)
        {
            pinkyBones[i].localRotation = Quaternion.Lerp(pinkyBones[i].localRotation, Quaternion.Lerp(maxPose.pinkyRots[i], minPose.pinkyRots[i], pinkyCurl), Time.deltaTime * lerpSpeed);
        }

        for (int i = 0; i < thumbBones.Length; i++)
        {
            thumbBones[i].localRotation = Quaternion.Lerp(thumbBones[i].localRotation, Quaternion.Lerp(maxPose.thumbRots[i], minPose.thumbRots[i], thumbCurl), Time.deltaTime * lerpSpeed);
        }


    }
}
