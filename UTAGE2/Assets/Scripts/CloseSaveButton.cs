﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSaveButton : MonoBehaviour
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
        gameObject.SendMessage("CloseSavePanel");
    }
}
