using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoneFrameData
{
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();
}