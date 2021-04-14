using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class AutoButton : MonoBehaviour
{
    public GameObject gameObject;
   
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
        gameObject.SendMessage("AutoButton");
    }
}
