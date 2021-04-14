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
        if (gameController.captionSpeed == 0.08f)
        {
            if (gameController.waitTime < 5.5f - slider.value + 0.8f)
            {
                gameController.waitTime += 0.8f;
            }
        }
        else
        {
            gameController.waitTime = 5.5f - slider.value;
        }
    }
    public void ChangeSlider()
    {    
        gameController.waitTime = 5.5f - slider.value;
        sampleText.wait = 5.5f - slider.value;
        sampleText.CallLoop();
    }
}
