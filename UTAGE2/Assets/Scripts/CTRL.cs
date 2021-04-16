using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTRL : MonoBehaviour
{
    public static int SaveSlotNo;
    [SerializeField] Transform btns;
    void Start()
    {
        SetBtns();
    }
    public void PushBtn(int n)
    {
        PlayerPrefs.SetInt("radio", n);
        Debug.Log("SaveSlot" + n);
        SaveSlotNo = n+1;
        SetBtns();
    }
    void SetBtns()
    {
        int r = PlayerPrefs.GetInt("radio"), cnt = 0;
        foreach (Transform b in btns) { b.GetComponent<Button>().interactable = (cnt != r); cnt++; }
    }
}
