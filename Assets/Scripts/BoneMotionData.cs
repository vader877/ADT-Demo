using System;
using System.Collections.Generic;

[Serializable]
public class BoneMotionData
{
    public string filename;
    public List<BoneFrameData> frames = new List<BoneFrameData>();
    public List<int> FlagPositions = new List<int>();
    public float TimeScale;
}