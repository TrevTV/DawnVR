using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HandPoseMaker : MonoBehaviour
{
    public Transform handRoot;
    public Handpose currentPose;
    public Transform[] thumbBones;
    public Transform[] indexBones;
    public Transform[] middleBones;
    public Transform[] ringBones;
    public Transform[] pinkyBones;

    public void MakeHandPose()
    {
        if (currentPose != null && handRoot != null)
        {
            PoseChanger changer = handRoot.GetComponent<PoseChanger>();
            if (changer != null)
            {
                thumbBones = changer.thumbBones;
                indexBones = changer.indexBones;
                middleBones = changer.middleBones;
                ringBones = changer.ringBones;
                pinkyBones = changer.pinkyBones;
            }

            //GetFingerBones();
            TransferBoneRotations(currentPose);
#if UNITY_EDITOR
            EditorUtility.SetDirty(currentPose);
#endif
        }
    }

    public void CreateJson()
    {
        string json = Valve.Newtonsoft.Json.JsonConvert.SerializeObject(currentPose, new Vector3Converter());
        Debug.Log(json);
    }

    private void TransferBoneRotations(Handpose newPose)
    {
        newPose.thumbRots = new Quaternion[thumbBones.Length];
        for (int i = 0; i < thumbBones.Length; i++)
        {
            newPose.thumbRots[i] = thumbBones[i].localRotation;
        }

        newPose.indexRots = new Quaternion[indexBones.Length];
        for (int i = 0; i < indexBones.Length; i++)
        {
            newPose.indexRots[i] = indexBones[i].localRotation;
        }

        newPose.middleRots = new Quaternion[middleBones.Length];
        for (int i = 0; i < middleBones.Length; i++)
        {
            newPose.middleRots[i] = middleBones[i].localRotation;
        }

        newPose.ringRots = new Quaternion[ringBones.Length];
        for (int i = 0; i < ringBones.Length; i++)
        {
            newPose.ringRots[i] = ringBones[i].localRotation;
        }

        newPose.pinkyRots = new Quaternion[pinkyBones.Length];
        for (int i = 0; i < pinkyBones.Length; i++)
        {
            newPose.pinkyRots[i] = pinkyBones[i].localRotation;
        }
    }

    public void ReverseTransferBoneRotations()
    {
        PoseChanger changer = handRoot.GetComponent<PoseChanger>();
        if (changer != null)
        {
            thumbBones = changer.thumbBones;
            indexBones = changer.indexBones;
            middleBones = changer.middleBones;
            ringBones = changer.ringBones;
            pinkyBones = changer.pinkyBones;
        }

        BackwardsTransferBoneRotations(currentPose);
    }

    private void BackwardsTransferBoneRotations(Handpose oldPose)
    {
        for (int i = 0; i < thumbBones.Length; i++)
        {
            Quaternion quat = oldPose.thumbRots[i];
            quat.y *= -1;
            quat.z *= -1;
            thumbBones[i].localRotation = quat;
        }

        for (int i = 0; i < indexBones.Length; i++)
        {
            Quaternion quat = oldPose.indexRots[i];
            quat.y *= -1;
            quat.z *= -1;
            indexBones[i].localRotation = quat;
        }

        for (int i = 0; i < middleBones.Length; i++)
        {
            Quaternion quat = oldPose.middleRots[i];
            quat.y *= -1;
            quat.z *= -1;
            middleBones[i].localRotation = quat;
        }

        for (int i = 0; i < ringBones.Length; i++)
        {
            Quaternion quat = oldPose.ringRots[i];
            quat.y *= -1;
            quat.z *= -1;
            ringBones[i].localRotation = quat;
        }

        for (int i = 0; i < pinkyBones.Length; i++)
        {
            Quaternion quat = oldPose.pinkyRots[i];
            quat.y *= -1;
            quat.z *= -1;
            pinkyBones[i].localRotation = quat;
        }
    }
}
