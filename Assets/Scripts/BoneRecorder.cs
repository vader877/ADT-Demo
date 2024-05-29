using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoneRecorder : MonoBehaviour
{
    public GameObject RecordTarget;
    private List<Transform> bones = new List<Transform>(); // List to store bones

    private bool isRecording = false;
    private bool isReplaying = false;
    private BoneMotionData recordedData = new BoneMotionData();
    private int currentFrame = 0;
    private int sessionFileId = 1;
    public string selectedFileName;

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
            StartPauseReplay();
            Debug.Log("Started Replay...");
        }

        if (Input.GetKey(KeyCode.Comma))
        {
            SetFramePosition(currentFrame++);
            Debug.Log(bones.Find(x => x.name == "mixamorig:Head").transform.position);
            //Debug.Log("Forward " + currentFrame);
        }

        if (Input.GetKey(KeyCode.Period))
        {
            SetFramePosition(currentFrame--);
            Debug.Log("Backward" + currentFrame);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayModeStarted();
            Debug.Log("Started Replay...");
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Saving");
            SaveRecording(GenerateFileName(sessionFileId, "json"));
            sessionFileId++;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            LoadRecording(selectedFileName);
            sessionFileId++;
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
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartPauseReplay()
    {
        if (recordedData.frames.Count > 0 && !isReplaying)
        {
            PlayModeStarted();
            StartCoroutine(Replay());
        }
        else if (recordedData.frames.Count > 0 && isReplaying)
        {
            isReplaying = false;        
        }
    }

    private void RecordFrame()
    {
        BoneFrameData frameData = new BoneFrameData();


        foreach (Transform bone in bones)
        {
            frameData.positions.Add(bone.localPosition);
            frameData.rotations.Add(bone.localRotation);
        }
        
        recordedData.frames.Add(frameData);
    }

    private IEnumerator Replay()
    {
        isReplaying = true;
        for (int frame = 0; frame < recordedData.frames.Count; frame++)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].localPosition = recordedData.frames[frame].positions[i];
                bones[i].localRotation = recordedData.frames[frame].rotations[i];
            }

            while (!isReplaying)
            {
                yield return null;
            }

            yield return new WaitForFixedUpdate(); // Wait for the next frame
        }
        isReplaying = false;
    }

    public void FastForward()
    {

    }

    public void Rewind()
    {

    }

    public void NextFrame()
    {
        if (isReplaying)
        {
            StopCoroutine(Replay());
        }
        SetFramePosition(currentFrame++);
    }

    public void PreviousFrame()
    {
        if (isReplaying)
        {
            StopCoroutine(Replay());
        }
        SetFramePosition(currentFrame--);
    }

    private void SetFramePosition(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex >= recordedData.frames.Count)
        {
            return;
        }

        BoneFrameData frameData = recordedData.frames[frameIndex];

        for (int i = 0; i < bones.Count; i++)
        {
            bones[i].localPosition = frameData.positions[i];
            bones[i].localRotation = frameData.rotations[i];
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
                rb.useGravity = false;
            }
        }
    }

    private void SaveRecording(string fileName)
    {
        string json = JsonUtility.ToJson(recordedData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
    }

    private void LoadRecording(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            recordedData = JsonUtility.FromJson<BoneMotionData>(json);
        }
        else
        {
            Debug.LogError("Recording file not found.");
        }
    }

    string GenerateFileName(int id, string extension)
    {
        DateTime now = DateTime.Now;

        string date = now.ToString("yyyyMMdd");
        string time = now.ToString("HHmmss");

        string fileName = $"{id}_{date}_{time}.{extension}";

        return fileName;
    }
}
