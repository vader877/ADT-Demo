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

    bool stopAnim = false;

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

        if (Input.GetKeyDown(KeyCode.G))
        {
            stopAnim = true;
        }


        if (isRecording)
        {
            RecordFrame();
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
        if(stopAnim)
        {
            animator.enabled = false;
        }

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
            yield return null; // Wait for the next frame
        }
        isReplaying = false;
    }
}
