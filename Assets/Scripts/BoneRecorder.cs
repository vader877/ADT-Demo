using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoneRecorder : MonoBehaviour
{
    public GameObject RecordTarget;
    public GameObject BoolFlag;
    public GameObject FileListItemPrefab;
    public TMP_Text FileName;

    private List<Transform> bones = new(); // List to store bones

    private bool isRecording = false;
    private bool isReplaying = false;
    private BoneMotionData recordedData = new BoneMotionData();
    private int currentFrame = 0;
    private int totalFrames = 0;
    private int sessionFileId = 1;
    private int playspeed = 1;
    public string selectedFileName;

    public Slider Tracker;
    public RectTransform contentPanel;
    public Button BoolTrigger;

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
        Tracker.value = 0;
        RefreshFileList();
    }


    private void FixedUpdate()
    {
        if (isRecording)
        {
            RecordFrame();
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        StartRecording();
    //        Debug.Log("Started Recording...");
    //    }
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        StopRecording(); 
    //        Debug.Log("Stopped Recording...");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        StartPauseReplay();
    //        Debug.Log("Started Replay...");
    //    }

    //    if (Input.GetKey(KeyCode.Comma))
    //    {
    //        SetFramePosition(currentFrame++);
    //        Debug.Log(bones.Find(x => x.name == "mixamorig:Head").transform.position);
    //        //Debug.Log("Forward " + currentFrame);
    //    }

    //    if (Input.GetKey(KeyCode.Period))
    //    {
    //        SetFramePosition(currentFrame--);
    //        Debug.Log("Backward" + currentFrame);
    //    }

    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        PlayModeStarted();
    //        Debug.Log("Started Replay...");
    //    }


    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        Debug.Log("Saving");
    //        SaveRecording(GenerateFileName(sessionFileId, "json"));
    //        sessionFileId++;
    //    }

    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        LoadRecording(selectedFileName);
    //        sessionFileId++;
    //    }
    //}

    private void FindBones(Transform parent)
    {
        foreach (Transform child in parent)
        {
            bones.Add(child);
            FindBones(child); // Recursively find all bones
        }
    }

    public void ToggleRecord(bool val)
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            recordedData.filename = GenerateFileName(sessionFileId, "json");
            FileName.text = recordedData.filename;
        }
    }



    public void StartPauseReplay()
    {
        if (recordedData.frames.Count > 0 && !isReplaying)
        {
            totalFrames = recordedData.frames.Count;
            playspeed = 1;
            RecordTarget.GetComponent<IRecordable>().PrepforReplay();
            StartCoroutine(Replay());
        }
        else if (recordedData.frames.Count > 0 && isReplaying)
        {
            isReplaying = false;
            //playspeed = 0;        
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
        while (currentFrame >= 0 && currentFrame < recordedData.frames.Count)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].localPosition = recordedData.frames[currentFrame].positions[i];
                bones[i].localRotation = recordedData.frames[currentFrame].rotations[i];
            }

            Debug.Log("frame:" + currentFrame);
            Debug.Log("frametot:" + recordedData.frames.Count);

            if (playspeed != 0)
            {
                float normalizedValue = Mathf.Clamp01((float)currentFrame % recordedData.frames.Count / recordedData.frames.Count);
                Tracker.value = normalizedValue;
            }

            currentFrame += playspeed;

            if (!isReplaying)
            {
                break;
            }

            yield return new WaitForFixedUpdate(); // Wait for the next frame
        }
        isReplaying = false;
    }


    public void FastForward()
    {
        RecordPlayerSingletonData.Instance.PlaySpeed = 3;
    }

    public void Rewind()
    {
        RecordPlayerSingletonData.Instance.PlaySpeed = -4;
    }

    public void NextFrame()
    {
        isReplaying = false;
        SetFramePosition(currentFrame++);
        float normalizedValue = Mathf.Clamp01((float)currentFrame % recordedData.frames.Count / recordedData.frames.Count);
        Tracker.value = normalizedValue;
    }

    public void PreviousFrame()
    {
        isReplaying = false;
        SetFramePosition(currentFrame--);
        float normalizedValue = Mathf.Clamp01((float)currentFrame % recordedData.frames.Count / recordedData.frames.Count);
        Tracker.value = normalizedValue;
    }

    public void OnSliderValueChanged()
    {

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

    public void SaveRecording()
    {
        recordedData.TimeScale = Time.timeScale;
        string json = JsonUtility.ToJson(recordedData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, recordedData.filename), json);
        recordedData = new BoneMotionData();
        sessionFileId++;
        RefreshFileList();
    }

    public void LoadRecording(string fileName)
    {
        RefreshFileList();
        //string filePath = Path.Combine(Application.persistentDataPath, fileName);
        //if (File.Exists(filePath))
        //{
        //    string json = File.ReadAllText(filePath);
        //    recordedData = JsonUtility.FromJson<BoneMotionData>(json);
        //}
        //else
        //{
        //    Debug.LogError("Recording file not found.");
        //}
    }

    string GenerateFileName(int id, string extension)
    {
        DateTime now = DateTime.Now;

        string date = now.ToString("yyyyMMdd");
        string time = now.ToString("HHmmss");

        string fileName = $"{id}_{date}_{time}.{extension}";

        return fileName;
    }

    private int offset = 145;

    public void RefreshFileList()
    {
        var loadedFiles = contentPanel.GetComponentsInChildren<TMP_Text>();
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");

        int offsetPerItem = -45;
        foreach (string filePath in files)
        {
            string json = File.ReadAllText(filePath);
            BoneMotionData motionData = JsonUtility.FromJson<BoneMotionData>(json);

            bool fileExists = false;
            foreach (TMP_Text loadedFile in loadedFiles)
            {
                if(loadedFile.text.Equals(motionData.filename))
                {
                    fileExists = true;
                    break;
                }
            }

            if (fileExists)
            {
                continue;
            }

            GameObject newItem = Instantiate(FileListItemPrefab, contentPanel);

            TMP_Text textComponent = newItem.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
               Debug.Log(textComponent.text);
               textComponent.text = motionData.filename;
            }

            newItem.GetComponentInChildren<Button>().onClick.AddListener(() => { OnClickListButton(motionData); });
            newItem.transform.SetParent(contentPanel, false);
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, offset += offsetPerItem, 0);
        }
    }

    public void OnClickListButton(BoneMotionData boneMotionData = null)
    {
        FileName.text = boneMotionData.filename;
    }

    public void OnClickBoolFlag()
    {

    }
}
