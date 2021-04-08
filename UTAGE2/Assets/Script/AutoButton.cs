using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class AutoButton : MonoBehaviour
{
    public GameObject gameObject;
    private bool isAuto = false;
    public int waitForSeconds = 3000;
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
        if (isAuto)
        {
            isAuto = false;
        }
        else
        {
            isAuto = true;
            while (isAuto)
            {
                gameObject.SendMessage("OnClick");
                Thread.Sleep(waitForSeconds);
            }
        }
    }
}
