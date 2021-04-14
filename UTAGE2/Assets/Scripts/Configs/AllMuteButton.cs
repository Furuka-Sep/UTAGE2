using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllMuteButton : MonoBehaviour
{
    public Slider AllSlider;
    public Slider BGMSlider;
    public Slider SESlider;
    public Slider VoiceSlider;
    public Toggle allToggle;
    public Toggle bgmToggle;
    public Toggle seToggle;
    public Toggle voiceToggle;
    private float nowValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (allToggle.isOn)
        {
            nowValue = AllSlider.value;
            AllSlider.value = 0;
            if (nowValue == 0.0f)
            {
                nowValue = 0.01f;
            }
        }
        else
        {
            
            bgmToggle.isOn = false;
            seToggle.isOn = false;
            voiceToggle.isOn = false;
            AllSlider.value = nowValue;
            BGMSlider.value = nowValue;
            SESlider.value = nowValue;
            VoiceSlider.value = nowValue;
        }
    }
}
