using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;
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
        if (toggle.isOn)
        {
            nowValue = slider.value;
            if (nowValue == 0.0f)
            {
                nowValue = 0.01f;
            }
            slider.value = 0;
        }
        else
        {
            slider.value = nowValue;
        }
    }

    
}
