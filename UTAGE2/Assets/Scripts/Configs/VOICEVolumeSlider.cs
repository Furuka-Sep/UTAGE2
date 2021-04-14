using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VOICEVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioSource VoiceSource;
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        VoiceSource.volume = slider.value;
        if (slider.value == 0)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }
}
