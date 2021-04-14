using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    public Slider BGMSlider;
    public AudioSource BGMSource;
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        BGMSlider.value = 1.0f;

    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        BGMSource.volume = BGMSlider.value;
        if (BGMSlider.value == 0)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }
}
