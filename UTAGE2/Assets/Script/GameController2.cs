using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController2 : MonoBehaviour
{
    private const char SEPARATE_SUBSCENE = '#';
    private const char SEPARATE_PAGE = '&';
    private const char SEPARATE_COMMAND = '!';
    private const char SEPARATE_MAIN_START = '「';
    private const char SEPARATE_MAIN_END = '」';
   // private const char SEPARATE_NARRATION = ' ';
   // ​
    private const char COMMAND_SEPARATE_PARAM = '=';
    private const char COMMAND_SEPARATE_ANIM = '%';
    
    private const string COMMAND_WAIT_TIME = "wait";
    private const string COMMAND_CHANGE_SCENE = "scene";
    
    private const string COMMAND_TEXT = "_text";
    [SerializeField]
    private string textFile = "Texts/Scenario";
    
    [SerializeField]
    private Text mainText;
    [SerializeField]
    private Text nameText;
    public GameObject log;
    public GameObject close;
    public GameObject auto;
    public GameObject skip;
    public GameObject qSave;
    public GameObject qLoad;
    public GameObject menu;
    [SerializeField]
    private float captionSpeed = 0.2f;
    private float _waitTime = 0;
    private string _text = "";
    private Dictionary<string, Queue<string>> _subScenes = new Dictionary<string, Queue<string>>();
    private Queue<string> _pageQueue;
    private Queue<char> _charQueue;
    private List<Image> _charaImageList = new List<Image>();
    private List<Button> _selectButtonList = new List<Button>();
    private List<AudioSource> _seList = new List<AudioSource>();
    private bool isHide = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    // Update is called once per frame
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
    
    /**
    * 初期化する
    */
    private void Init()
    {
        _text = LoadTextFile(textFile);
        Queue<string> subScenes = SeparateString(_text, SEPARATE_SUBSCENE);
        foreach (string subScene in subScenes)
        {
            if (subScene.Equals("")) continue;
            Queue<string> pages = SeparateString(subScene, SEPARATE_PAGE);
            _subScenes[pages.Dequeue()] = pages;
        }
        _pageQueue = _subScenes.First().Value;
        ShowNextPage();
    }
    
    /**
    * テキストファイルを読み込む
    */
    private string LoadTextFile(string fname)
    {
        TextAsset textasset = Resources.Load<TextAsset>(fname);
        return textasset.text.Replace("\n", "").Replace("\r", "");
    }
    
    /**
    * クリックしたときの処理
    */
    private void OnClick()
    {
        if (!mainText.transform.parent.gameObject.activeSelf) return;
        if (_charQueue.Count > 0) OutputAllChar();
        else
        {
            if (_selectButtonList.Count > 0) return;
            if (!ShowNextPage())
                EditorApplication.isPlaying = false;
        }
    }
    
    /**
     * 文字送りするコルーチン
     */
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
    
    /**
    * 次の読み込みを待機するコルーチン
    */
    private IEnumerator WaitForCommand()
    {
        yield return new WaitForSeconds(_waitTime);
        _waitTime = 0;
        ShowNextPage();
        yield break;
    }
    
    /**
    * 1文字を出力する
    */
    private bool OutputChar()
    {
        if (_charQueue.Count <= 0)
        {
            //nextPageIcon.SetActive(true);
            return false;
        }
        mainText.text += _charQueue.Dequeue();
        return true;
    }
    
    /**
     * 全文を表示する
     */
    private void OutputAllChar()
    {
        StopCoroutine(ShowChars(captionSpeed));
        while (OutputChar()) ;
        _waitTime = 0;
        //nextPageIcon.SetActive(true);
    }
    
    /**
     * 次のページを表示する
     */
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0) return false;
        // nextPageIcon.SetActive(false);
        ReadLine(_pageQueue.Dequeue());
        return true;
    }
    
    /**
     * 文字列を指定した区切り文字ごとに区切り、キューに格納したものを返す
     */
    private Queue<string> SeparateString(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }
    
    /**
     * 文を1文字ごとに区切り、キューに格納したものを返す
     */
    private Queue<char> SeparateString(string str)
    {
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        foreach (char c in chars) charQueue.Enqueue(c);
        return charQueue;
    }
    
    /**
     * 1行を読み出す
     */
    private void ReadLine(string text)
    {
        if (text[0].Equals(SEPARATE_COMMAND))
        {
            ReadCommand(text);
            if (_selectButtonList.Count > 0) return;
            if (_waitTime > 0)
            {
                StartCoroutine(WaitForCommand());
                return;
            }
            ShowNextPage();
            return;
        }
        string[] ts = text.Split(SEPARATE_MAIN_START);
        string name = ts[0];
        string main = ts[1].Remove(ts[1].LastIndexOf(SEPARATE_MAIN_END));
        nameText.text = name;
        if (name.Equals("")) nameText.transform.parent.gameObject.SetActive(false);
        else nameText.transform.parent.gameObject.SetActive(true);
        mainText.text = "";
        _charQueue = SeparateString(main);
        StartCoroutine(ShowChars(captionSpeed));
    }
    
    /**
     * コマンドの読み出し
     */
    private void ReadCommand(string cmdLine)
    {
        cmdLine = cmdLine.Remove(0, 1);
        Queue<string> cmdQueue = SeparateString(cmdLine, SEPARATE_COMMAND);
        foreach (string cmd in cmdQueue)
        {
            string[] cmds = cmd.Split(COMMAND_SEPARATE_PARAM);
            if (cmds[0].Contains(COMMAND_WAIT_TIME))
                SetWaitTime(cmds[1]);
            if (cmds[0].Contains(COMMAND_CHANGE_SCENE))
                ChangeNextScene(cmds[1]);
        }
    }
    /**
 * 対応するシーンに切り替える
 */
    private void ChangeNextScene(string parameter)
    {
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        SceneManager.LoadSceneAsync(parameter);
    }
    /**
  * 待機時間を設定する
  */
    private void SetWaitTime(string parameter)
    {
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        _waitTime = float.Parse(parameter);
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
}