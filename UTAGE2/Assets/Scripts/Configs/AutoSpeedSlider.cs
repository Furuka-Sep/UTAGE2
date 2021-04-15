using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSpeedSlider : MonoBehaviour
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
    void Update()
    {
        
    }
    public void ChangeSlider()
    {    
        gameController.waitTime = 4.0f - slider.value;
        sampleText.wait = 4.0f - slider.value;
    }
}
