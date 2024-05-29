using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasMouseHandler : MonoBehaviour
{
    public Slider yourSlider;
    public Text sliderValueText;
    public Toggle RecordButton;

    void Start()
    {


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            DetectMouseClick();
        }
    }

    void DetectMouseClick()
    {
        Vector2 localMousePosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform,
            Input.mousePosition,
            null,
            out localMousePosition
        );

        Debug.Log("Mouse Position: " + localMousePosition);
    }

  
    void OnSliderValueChanged(float value)
    {
        Debug.Log("Slider Value Changed: " + value);
        if (sliderValueText != null)
        {
            sliderValueText.text = "Slider Value: " + value.ToString("0.00");
        }
    }
}
