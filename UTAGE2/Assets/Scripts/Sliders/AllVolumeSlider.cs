using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllVolumeSlider : MonoBehaviour
{
    public Slider AllSlider;
    public Slider BGMSlider;
    public Slider SESlider;
    public Slider VoiceSlider;
    // Start is called before the first frame update
    void Start()
    {
        AllSlider.value = 1.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        BGMSlider.value = AllSlider.value;
        SESlider.value = AllSlider.value;
        VoiceSlider.value = AllSlider.value;
    }
}
