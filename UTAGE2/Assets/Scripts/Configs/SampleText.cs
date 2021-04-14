using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SampleText : MonoBehaviour
{
    public Text text;
    public float captionSpeed;
    public float wait;
    private string sampletext = "どこか遠くで、カモメの声が聞こえる。";
    private Queue<char> _charQueue;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallLoop()
    {
        StartCoroutine(MakeLoop());
    }

    private IEnumerator MakeLoop()
    {
        while (true)
        {
            ReadText();
            yield return new WaitForSeconds(wait);
        }
    }

    private void ReadText()
    {
        text.text = "";
        _charQueue = SeparateString(sampletext);
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
    private IEnumerator ShowChars(float time)
    {
        while (true)
        {
            if (!OutputChar())
            {
                break;
            }
            yield return new WaitForSeconds(time);
        }
        yield break;
    }

    private bool OutputChar()
    {
        // キューに何も格納されていなければfalseを返す
        if (_charQueue.Count <= 0) return false;
        text.text += _charQueue.Dequeue();
        return true;
    }
}
