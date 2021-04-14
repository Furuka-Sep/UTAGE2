using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioSource SEPrefab;
    public AudioSource OpenSE;
    public AudioSource CloseSE;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1.0f;
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        SEPrefab.volume = slider.value;
        OpenSE.volume = slider.value;
        CloseSE.volume = slider.value;
        OpenSE.Play();
    }
}
