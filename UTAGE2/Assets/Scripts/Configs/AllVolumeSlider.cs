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
    public Toggle allToggle;
    //public Toggle bgmToggle;
    //public Toggle seToggle;
    //public Toggle voiceToggle;
    // Start is called before the first frame update
    void Start()
    {
        AllSlider.value = 0.5f;
    }
    void Update()
    {
        
        
    }
    // Update is called once per frame
    public void ChangeSlider()
    {
        BGMSlider.value = AllSlider.value;
        SESlider.value = AllSlider.value;
        VoiceSlider.value = AllSlider.value;
        if (AllSlider.value == 0)
        {
            allToggle.isOn = true;
        }
        else
        {
            allToggle.isOn = false;
            //bgmToggle.isOn = false;
            //seToggle.isOn = false;
            //voiceToggle.isOn = false;
        }
    }
}
