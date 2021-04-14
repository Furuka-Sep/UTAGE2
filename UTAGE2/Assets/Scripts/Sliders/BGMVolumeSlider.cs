using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    public Slider BGMSlider;
    public AudioSource BGMSource;
    // Start is called before the first frame update
    void Start()
    {
        BGMSlider.value = 1.0f;

    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        BGMSource.volume = BGMSlider.value;
    }
}
