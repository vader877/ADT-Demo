using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneRecorder : MonoBehaviour
{
    public GameObject RecordTarget;
    private List<Transform> bones = new List<Transform>(); // List to store bones
    private List<Dictionary<Transform, Vector3>> recordedPositions = new List<Dictionary<Transform, Vector3>>();
    private List<Dictionary<Transform, Quaternion>> recordedRotations = new List<Dictionary<Transform, Quaternion>>();
    private bool isRecording = false;
    private bool isReplaying = false;
    private bool replayMode = false;
    private Animator animator;


    void Start()
    {
        // Find all bones in the armature
        Transform armature = RecordTarget.transform.Find("Armature");
        if (armature != null)
        {
            FindBones(armature);
        }
        else
        {
            Debug.LogError("Armature not found. Make sure the armature is named 'Armature'.");
        }


        animator = RecordTarget.GetComponent<Animator>();

    }


    private void FixedUpdate()
    {
        if (isRecording)
        {
            RecordFrame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
            Debug.Log("Started Recording...");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StopRecording(); 
            Debug.Log("Stopped Recording...");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartReplay();
            Debug.Log("Started Replay...");
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            StepForward();
            Debug.Log(bones.Find(x => x.name == "mixamorig:Head").transform.position);
            //Debug.Log("Forward " + currentFrame);
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            StepBackward();
            Debug.Log("Backward" + currentFrame);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayModeStarted();
            Debug.Log("Started Replay...");
        }



    }

    private void FindBones(Transform parent)
    {
        foreach (Transform child in parent)
        {
            bones.Add(child);
            FindBones(child); // Recursively find all bones
        }
    }

    public void StartRecording()
    {
        recordedPositions.Clear();
        recordedRotations.Clear();
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartReplay()
    {
        foreach (Transform bone in bones)
        {
            var rb =bone.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.isKinematic = true;
            }
        }

        if (recordedPositions.Count > 0 && !isReplaying)
        {
            StartCoroutine(Replay());
        }
    }

    private void RecordFrame()
    {
        Dictionary<Transform, Vector3> currentPositions = new Dictionary<Transform, Vector3>();
        Dictionary<Transform, Quaternion> currentRotations = new Dictionary<Transform, Quaternion>();

        foreach (Transform bone in bones)
        {
            currentPositions[bone] = bone.localPosition;
            currentRotations[bone] = bone.localRotation;
        }

        recordedPositions.Add(currentPositions);
        recordedRotations.Add(currentRotations);
    }

    private IEnumerator Replay()
    {
        isReplaying = true;
        for (int frame = 0; frame < recordedPositions.Count; frame++)
        {
            foreach (Transform bone in bones)
            {
                bone.localPosition = recordedPositions[frame][bone];
                bone.localRotation = recordedRotations[frame][bone];
                
            }
            yield return new WaitForFixedUpdate(); // Wait for the next frame
        }
        isReplaying = false;
    }

    private int currentFrame = 0;

    private void SetFramePosition(int frame)
    {
        foreach (Transform bone in bones)
        {
            bone.localPosition = recordedPositions[frame][bone];
            bone.localRotation = recordedRotations[frame][bone];
        }
    }

    private void StepForward()
    {
        if (recordedPositions.Count>0 && currentFrame < recordedPositions.Count)
        {
            SetFramePosition(currentFrame++);
        }
    }

    private void StepBackward()
    {
        if (recordedPositions.Count > 0 && currentFrame > 0 )
        {
            SetFramePosition(currentFrame--);
        }
    }



    private void PlayModeStarted()
    {
        foreach (Transform bone in bones)
        {
            var rb = bone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

}
