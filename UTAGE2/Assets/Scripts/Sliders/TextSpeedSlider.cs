using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSpeedSlider : MonoBehaviour
{
    public Slider slider;
    public GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = 8.0f;
        slider.minValue = 2.0f;
        slider.value = 5.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        gameController.captionSpeed = slider.value/100;
    }
}
