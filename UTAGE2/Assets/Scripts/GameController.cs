﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


// MonoBehaviourを継承することでオブジェクトにコンポーネントとして
// アタッチすることができるようになる
public class GameController : MonoBehaviour
{
    public Text nameText;
    public Text mainText;
    public GameObject log;
    public GameObject close;
    public GameObject auto;
    public GameObject skip;
    public GameObject qSave;
    public GameObject qLoad;
    public GameObject menu;
    [SerializeField]
    private float captionSpeed = 0.2f;

    // パラメーターを追加
    private const char SEPARATE_MAIN_START = '「';
    private const char SEPARATE_MAIN_END = '」';
    private Queue<char> _charQueue;
    private const char SEPARATE_PAGE = '&';
    private Queue<string> _pageQueue;
    private string[] splitText;
    private bool isHide = false;
    private bool isAuto = false;
    private bool isSkip = false;
    public float waitTime = 3.0f;
    private IEnumerator autoButtonCoroutine;
    public float skipTime = 0.5f;
    private IEnumerator skipButtonCoroutine;
    private string defaultColorCode = "#43FFFE";
    private string changedColorCode = "#7aFF7a";
    private Color defaultColor;
    private Color changedColor;


    // パラメーターを変更
    private string _text = "";

    // メソッドを変更
    private void Start()
    {
        autoButtonCoroutine = AutoMessage();
        skipButtonCoroutine = SkipMessage();
        ColorUtility.TryParseHtmlString(defaultColorCode, out defaultColor);
        ColorUtility.TryParseHtmlString(changedColorCode, out changedColor);
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load("Texts/Scenario",typeof(TextAsset)) as TextAsset;
        string textLine = textAsset.text;
        splitText = textLine.Split(char.Parse("\n"));
        _text = string.Join("", splitText);
        Init();
    }

    /**
    * 1行を読み出す
*/
    private void ReadLine(string text)
    {
        string[] ts = text.Split(SEPARATE_MAIN_START);
        string name;
        if (ts[0].Equals("none"))
        {
            name = " ";
        }
        else
        {
            name = ts[0];
        }
        string main = ts[1].Remove(ts[1].LastIndexOf(SEPARATE_MAIN_END));
        nameText.text = name;
        mainText.text = "";
        _charQueue = SeparateString(main);
        // コルーチンを呼び出す
        StartCoroutine(ShowChars(captionSpeed));
    }

    private Queue<char> SeparateString(string str)
    {
        // 文字列をchar型の配列にする = 1文字ごとに区切る
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        // foreach文で配列charsに格納された文字を全て取り出し
        // キューに加える
        foreach (char c in chars) charQueue.Enqueue(c);
        return charQueue;
    }

    private bool OutputChar()
    {
        // キューに何も格納されていなければfalseを返す
        if (_charQueue.Count <= 0) return false;
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    private IEnumerator ShowChars(float wait)
    {
        while (true)
        {
            if (mainText.transform.parent.gameObject.activeSelf)
            {
                if (!OutputChar()) break;
            }
            yield return new WaitForSeconds(wait);
        }
        yield break;
    }

    private void OutputAllChar()
    {
        // コルーチンをストップ
        StopCoroutine(ShowChars(captionSpeed));
        // キューが空になるまで表示
        while (OutputChar()) ;
    }


    private void OnClick()
    {
        if (_charQueue.Count > 0) OutputAllChar();
        else
        {
            if (!ShowNextPage())
            {

            }
                // UnityエディタのPlayモードを終了する
                
        }
    }

    // MonoBehaviourを継承している場合限定で
    // 毎フレーム呼ばれる 
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) OnClickRight();
        if (Input.GetMouseButtonDown(0))
        {
            if (isHide)
            {
                OnClickRight();
            }
        }
    }

    private Queue<string> SeparateString(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    /**
    * 初期化する
*/
    private void Init()
    {
        _pageQueue = SeparateString(_text, SEPARATE_PAGE);
        ShowNextPage();
    }

    /**
    * 次のページを表示する
*/
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0) return false;
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    private void OnClickRight()
    {
        GameObject mainWindow = mainText.transform.parent.gameObject;
        GameObject nameWindow = nameText.transform.parent.gameObject;
        mainWindow.SetActive(!mainWindow.activeSelf);
        nameWindow.SetActive(mainWindow.activeSelf);
        log.SetActive(mainWindow.activeSelf);
        close.SetActive(mainWindow.activeSelf);
        auto.SetActive(mainWindow.activeSelf);
        skip.SetActive(mainWindow.activeSelf);
        qLoad.SetActive(mainWindow.activeSelf);
        qSave.SetActive(mainWindow.activeSelf);
        menu.SetActive(mainWindow.activeSelf);
        if (isHide)
        {
            isHide = false;
        }
        else
        {
            isHide = true;
        }
    }

    private void AutoButton()
    {
        Button btn = auto.GetComponent<Button>();
        if (isAuto)
        {
            isAuto = false;
            btn.image.color = defaultColor;
        }
        else
        {
            isAuto = true;
            btn.image.color = changedColor;
        }
        if (isAuto)
        {
            StartCoroutine(autoButtonCoroutine);
        }
        else
        {
            StopCoroutine(autoButtonCoroutine);
        }
    }

    IEnumerator AutoMessage()
    {
        OnClick();
        while (isAuto)
        {
            yield return new WaitForSeconds(waitTime);
            OnClick();
        }
    }
    
    private void SkipButton()
    {
        Button btn = skip.GetComponent<Button>();
        if (isSkip)
        {
            isSkip = false;
            btn.image.color = defaultColor;
        }
        else
        {
            isSkip = true;
            btn.image.color = changedColor;
        }
        if(isSkip)
        {
            StartCoroutine(skipButtonCoroutine);
        }
        else
        {
            StopCoroutine(skipButtonCoroutine);
        }
    }

    IEnumerator SkipMessage()
    {
        OnClick();
        while (isSkip)
        {
            yield return new WaitForSeconds(skipTime);
            OnClick();
        }
    }


}
