using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSpeedSlider : MonoBehaviour
{
    public Slider slider;
    public GameController gameController;
    public SampleText sampleText;
    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = 3.0f;
        slider.minValue = 1.0f;
        slider.value = 2.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        gameController.captionSpeed = 0.1f-(slider.value/100*3-0.01f);
        sampleText.captionSpeed = 0.1f - (slider.value / 100 * 3 - 0.01f);
        sampleText.CallLoop();
    }
}
