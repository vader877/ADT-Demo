using UnityEngine;

public class RecordPlayerSingletonData : MonoBehaviour
{
    private static readonly System.Lazy<RecordPlayerSingletonData> lazyInstance =
       new System.Lazy<RecordPlayerSingletonData>(() =>
       {
           Debug.Log("Creating singleton instance");
           var singletonObject = new GameObject("RecordPlayerSingletonData");
           var singletonComponent = singletonObject.AddComponent<RecordPlayerSingletonData>();
           DontDestroyOnLoad(singletonObject);
           return singletonComponent;
       });


    private int currentFrame;

    public int CurrentFrame
    {
        get { return currentFrame; }
        set { currentFrame = value; }
    }

    private int playSpeed = 1;

    public int PlaySpeed
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }

    private int totalFrameLength;

    public int TotalFrameLength
    {
        get { return totalFrameLength; }
        set { totalFrameLength = value; }
    }

    public static RecordPlayerSingletonData Instance
    {
        get
        {
            return lazyInstance.Value;
        }
    }

    private RecordPlayerSingletonData() { }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


}
