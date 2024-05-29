using System;
using System.Collections.Generic;

[Serializable]
public class BoneMotionData
{
    public List<BoneFrameData> frames = new List<BoneFrameData>();
    public List<int> FlagPositions = new List<int>();
}