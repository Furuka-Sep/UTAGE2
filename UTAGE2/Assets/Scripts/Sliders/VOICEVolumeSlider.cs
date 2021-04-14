using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VOICEVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioSource VoiceSource;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        VoiceSource.volume = slider.value;
    }
}
