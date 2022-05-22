using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandPose", menuName = "HandPose")]
public class Handpose : ScriptableObject
{
    public Quaternion[] thumbRots;
    public Quaternion[] indexRots;
    public Quaternion[] middleRots;
    public Quaternion[] ringRots;
    public Quaternion[] pinkyRots;
    
}
