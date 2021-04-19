using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


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
    public GameObject bgPanel;
    public GameObject leftChar;
    public GameObject centerChar;
    public GameObject rightChar;

    public GameObject BackLog;
    public GameObject Content;
    public GameObject BackLogNode;
    public GameObject menuPanel;
    public GameObject savePanel;
    public GameObject loadPanel;
    public GameObject ScenarioPanel;
    public GameObject TitlePanel;
    public GameObject ConfigPanel;
    public GameObject TitleConfirmPanel;
    public AudioSource OpenButtonAudio;
    public AudioSource CloseButtonAudio;
    public GameObject TitleBG;
    public Slider BGMSlider;
    public GameObject choicePanel;

    [SerializeField]
    public float captionSpeed = 0.2f;
    private const float CELLHEIGHT = 110.0f;

    // パラメーターを追加
    private const char SEPARATE_SUBSCENE = '#';
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
    private string defaultColorCode = "#67fcff";
    private string changedColorCode = "#7aFF7a";
    private Color defaultColor;
    private Color changedColor;
    private const char SEPARATE_COMMAND = '!';
    private const char COMMAND_SEPARATE_PARAM = '=';
    private const string COMMAND_BACKGROUND = "background";
    private const string COMMAND_SPRITE = "_sprite";
    private const string COMMAND_COLOR = "_color";
    private const string COMMAND_ACTIVE = "_active";
    private const string COMMAND_BGM = "bgm";
    private const string COMMAND_SE = "se";

    private const string COMMAND_DELETE = "_delete";
    private const string COMMAND_PLAY = "_play";
    private const string COMMAND_MUTE = "_mute";
    private const string COMMAND_SOUND = "_sound";
    private const string COMMAND_VOLUME = "_volume";
    private const string COMMAND_PRIORITY = "_priority";
    private const string COMMAND_LOOP = "_loop";
    private const string COMMAND_FADE = "_fade";
    private const string COMMAND_JUMP = "jump_to";
    private const string COMMAND_SELECT = "select";
    private const string COMMAND_TEXT = "_text";
    private const string SELECT_BUTTON_PREFAB = "choiceButton";

    private const string SE_AUDIOSOURCE_PREFAB = "SEAudioSource";

    [SerializeField]
    private string spritesDirectory = "BgImages/";
    [SerializeField]
    private string charDirectory = "CharImages/";
    private const string COMMAND_CHARACTER_IMAGE = "charaimg";
    private const string COMMAND_SIZE = "_size";
    private const string COMMAND_POSITION = "_pos";
    private const string COMMAND_ROTATION = "_rotate";
    private const string CHARACTER_IMAGE_PREFAB = "CharacterImage";

    [SerializeField]
    private string prefabsDirectory = "Prefabs/";
    [SerializeField]
    private string audioClipsDirectory = "AudioClips/";
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField]
    private GameObject seAudioSources;
    private List<AudioSource> _seList = new List<AudioSource>();
    [SerializeField]
    private GameObject selectButtons;
    private List<Button> _selectButtonList = new List<Button>();

    private List<Image> _charaImageList = new List<Image>();
    private Dictionary<string, GameObject> _charaImageMap = new Dictionary<string, GameObject>();
    private List<string> posStrings = new List<string>() { "left", "center", "right" };
    private List<GameObject> charaObjects = new List<GameObject>();
    private Dictionary<string, Queue<string>> _subScenes = new Dictionary<string, Queue<string>>();

    public int sc = 0;
    public bool isLoad = false;
    private GameObject currentObject;
    private string main;
    private bool isFinish = false;
    private bool isLoading = false;



    // パラメーターを変更
    private string _text = "";
    private Text text1;
    private Text text2;
    private Image image1;

    // メソッドを変更
    private void Start()
    {
        charaObjects.Add(leftChar);
        charaObjects.Add(centerChar);
        charaObjects.Add(rightChar);

        Image titleImage = TitleBG.GetComponent<Image>();
        titleImage.sprite = Instantiate(Resources.Load<Sprite>("TitleResources/titleImage"));

        OpenButtonAudio.clip = LoadAudioClip("openButton");
        CloseButtonAudio.clip = LoadAudioClip("closeButton");
        bgmAudioSource.clip = LoadTitleAudioClip();
        bgmAudioSource.Play();
        bgmAudioSource.volume = 1 * BGMSlider.value;
        bgmAudioSource.loop = true;
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load("Texts/Scenario", typeof(TextAsset)) as TextAsset;
        string textLine = textAsset.text;
        splitText = textLine.Split(char.Parse("\n"));
        _text = string.Join("", splitText);
    }

    /**
    * 1行を読み出す
*/
    private void ReadLine(string text)
    {
        // 最初が「!」だったら
        if (text[0].Equals(SEPARATE_COMMAND))
        {
            ReadCommand(text);
            if (_selectButtonList.Count > 0) return;
            ShowNextPage();
            return;
        }
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
        main = ts[1].Remove(ts[1].LastIndexOf(SEPARATE_MAIN_END));
        nameText.text = name;
        mainText.text = "";
        _charQueue = SeparateString(main);
        StartCoroutine(ShowChars(captionSpeed));
        CreateBackLog(name, main);
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
                isAuto = false;
                isSkip = false;
                isFinish = true;
                GameObject mainWindow = mainText.transform.parent.gameObject;
                GameObject nameWindow = nameText.transform.parent.gameObject;
                mainWindow.SetActive(false);
                nameWindow.SetActive(false);
                log.SetActive(false);
                close.SetActive(false);
                auto.SetActive(false);
                skip.SetActive(false);
                qLoad.SetActive(false);
                qSave.SetActive(false);
                menu.SetActive(false);
                //EditorApplication.isPlaying = false;
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
        if (isFinish)
        {

            if (Input.GetMouseButtonDown(0))
            {
                ScenarioPanel.SetActive(false);
                TitlePanel.SetActive(true);
                Button autobtn = auto.GetComponent<Button>();
                Sprite autosp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
                Image autobuttonImage = autobtn.GetComponent<Image>();
                Button skipbtn = skip.GetComponent<Button>();
                Sprite skipsp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
                Image skipbuttonImage = skipbtn.GetComponent<Image>();
                skipbuttonImage.sprite = skipsp;
                autobuttonImage.sprite = autosp;
                isFinish = false;
                bgmAudioSource.clip = LoadTitleAudioClip();
                bgmAudioSource.Play();
                bgmAudioSource.volume = 1 * BGMSlider.value;
                bgmAudioSource.loop = true;
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
        GameObject mainWindow = mainText.transform.parent.gameObject;
        GameObject nameWindow = nameText.transform.parent.gameObject;
        autoButtonCoroutine = AutoMessage();
        skipButtonCoroutine = SkipMessage();
        mainWindow.SetActive(true);
        nameWindow.SetActive(true);
        log.SetActive(true);
        close.SetActive(true);
        auto.SetActive(true);
        skip.SetActive(true);
        qLoad.SetActive(true);
        qSave.SetActive(true);
        menu.SetActive(true);
        _pageQueue = SeparateString(_text, SEPARATE_PAGE);


        if (isLoad == true)
        {
            int sc2 = PlayerPrefs.GetInt("Qsave");
            Debug.Log("ロードしました　ページ番号" + sc2);
            isLoading = true;
            while (sc < sc2)
            {

                ShowNextPage();
                if (sc == sc2 - 1)
                {
                    isLoading = false;
                }
            }
        }
        else
        {
            Queue<string> subScenes = SeparateString(_text, SEPARATE_SUBSCENE);
            foreach (string subScene in subScenes)
            {
                if (subScene.Equals("")) continue;
                Queue<string> pages = SeparateString(subScene, SEPARATE_PAGE);
                _subScenes[pages.Dequeue()] = pages;
            }
            while (sc <= 0)
            {
                ShowNextPage();
            }
        }

    }


    /**
    * 次のページを表示する
*/

    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0) return false;
        ReadLine(_pageQueue.Dequeue());
        sc++;
        Debug.Log(sc);
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
        isAuto = false;
        isSkip = false;
        Button autobtn = auto.GetComponent<Button>();
        Sprite autosp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image autobuttonImage = autobtn.GetComponent<Image>();
        Button skipbtn = skip.GetComponent<Button>();
        Sprite skipsp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image skipbuttonImage = skipbtn.GetComponent<Image>();
        skipbuttonImage.sprite = skipsp;
        autobuttonImage.sprite = autosp;
        if (isHide)
        {
            isHide = false;
            OpenButtonAudio.Play();
        }
        else
        {
            isHide = true;
            CloseButtonAudio.Play();
        }
    }

    private void AutoButton()
    {
        Button btn = auto.GetComponent<Button>();
        if (isAuto)
        {
            isAuto = false;
            Sprite sp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
            Image buttonImage = btn.GetComponent<Image>();
            buttonImage.sprite = sp;
            //btn.image.color = defaultColor;
            CloseButtonAudio.Play();
        }
        else
        {
            isAuto = true;
            Sprite sp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button"));
            Image buttonImage = btn.GetComponent<Image>();
            buttonImage.sprite = sp;
            //btn.image.color = changedColor;
            OpenButtonAudio.Play();
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
        while (isAuto)
        {
            yield return new WaitForSeconds(_charQueue.Count * captionSpeed);
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
            Sprite sp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
            Image buttonImage = btn.GetComponent<Image>();
            buttonImage.sprite = sp;
            //btn.image.color = defaultColor;
            CloseButtonAudio.Play();
        }
        else
        {
            isSkip = true;
            Sprite sp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button"));
            Image buttonImage = btn.GetComponent<Image>();
            buttonImage.sprite = sp;
            //btn.image.color = changedColor;
            OpenButtonAudio.Play();
        }
        if (isSkip)
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

    private void ReadCommand(string cmdLine)
    {
        // 最初の「!」を削除する
        cmdLine = cmdLine.Remove(0, 1);
        Queue<string> cmdQueue = SeparateString(cmdLine, SEPARATE_COMMAND);
        foreach (string cmd in cmdQueue)
        {
            // 「=」で分ける
            string[] cmds = cmd.Split(COMMAND_SEPARATE_PARAM);
            // もし背景コマンドの文字列が含まれていたら
            if (cmds[0].Contains(COMMAND_BACKGROUND))
                SetBackgroundImage(cmds[0], cmds[1]);
            if (cmds[0].Contains(COMMAND_CHARACTER_IMAGE))
            {
                SetCharactorImage(cmds[0], cmds[1], cmds[2]);
            }
            if (cmds[0].Contains(COMMAND_BGM))
                SetBackgroundMusic(cmds[0], cmds[1]);
            if (cmds[0].Contains(COMMAND_SE))
            {
                if (!isLoading)
                {
                    SetSoundEffect(cmds[1], cmds[0], cmds[2]);
                }
            }
            if (cmds[0].Contains(COMMAND_JUMP))
                JumpTo(cmds[1]);
            if (cmds[0].Contains(COMMAND_SELECT))
            {
                SetSelectButton(cmds[1], cmds[0], cmds[2]);
                choicePanel.SetActive(true);
                isAuto = false;
                isSkip = false;
                Button autobtn = auto.GetComponent<Button>();
                Sprite autosp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
                Image autobuttonImage = autobtn.GetComponent<Image>();
                Button skipbtn = skip.GetComponent<Button>();
                Sprite skipsp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
                Image skipbuttonImage = skipbtn.GetComponent<Image>();
                skipbuttonImage.sprite = skipsp;
                autobuttonImage.sprite = autosp;
            }
        }
    }

    private void SetBackgroundImage(string cmd, string parameter)
    {
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        Sprite sp = Instantiate(Resources.Load<Sprite>(spritesDirectory + parameter));
        Image bgImageComponent = bgPanel.GetComponent<Image>();
        bgImageComponent.sprite = sp;
    }

    private Vector3 ParameterToVector3(string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        return new Vector3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
    }

    private void SetCharactorImage(string cmd, string pos, string parameter)
    {
        cmd = cmd.Replace(" ", "");
        cmd = cmd.Replace(COMMAND_CHARACTER_IMAGE, "");
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        pos = pos.Substring(pos.IndexOf('"') + 1, pos.LastIndexOf('"') - pos.IndexOf('"') - 1);
        switch (cmd)
        {
            case COMMAND_SPRITE:
                Sprite sp = Instantiate(Resources.Load<Sprite>(charDirectory + parameter));
                Image charImageComponent = charaObjects[posStrings.IndexOf(pos)].GetComponent<Image>();
                charImageComponent.sprite = sp;
                break;
            case COMMAND_SIZE:
                charaObjects[posStrings.IndexOf(pos)].GetComponent<RectTransform>().sizeDelta = ParameterToVector3(parameter);
                break;
            case COMMAND_POSITION:
                charaObjects[posStrings.IndexOf(pos)].GetComponent<RectTransform>().anchoredPosition = ParameterToVector3(parameter);
                break;
            case COMMAND_ACTIVE:
                charaObjects[posStrings.IndexOf(pos)].SetActive(ParameterToBool(parameter));
                break;
        }

    }

    private bool ParameterToBool(string parameter)
    {
        string p = parameter.Replace(" ", "");
        return p.Equals("true") || p.Equals("TRUE");
    }

    private void CreateBackLog(string backLogName, string backLogText)
    {
        GameObject cell = Instantiate(BackLogNode);
        cell.transform.SetParent(Content.transform, false);
        Text logName = cell.transform.Find("charaName").GetComponent<Text>();
        logName.text = backLogName;
        Text logText = cell.transform.Find("dialogText").GetComponent<Text>();
        logText.text = backLogText;
    }
    private void ViewLog()
    {
        BackLog.SetActive(true);
        OpenButtonAudio.Play();
        isAuto = false;
        isSkip = false;
        Button autobtn = auto.GetComponent<Button>();
        Sprite autosp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image autobuttonImage = autobtn.GetComponent<Image>();
        Button skipbtn = skip.GetComponent<Button>();
        Sprite skipsp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image skipbuttonImage = skipbtn.GetComponent<Image>();
        skipbuttonImage.sprite = skipsp;
        autobuttonImage.sprite = autosp;
    }
    private void CloseLog()
    {
        BackLog.SetActive(false);
        CloseButtonAudio.Play();
    }
    private void ViewMenuPanel()
    {
        menuPanel.SetActive(true);
        OpenButtonAudio.Play();
        isAuto = false;
        isSkip = false;
        Button autobtn = auto.GetComponent<Button>();
        Sprite autosp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image autobuttonImage = autobtn.GetComponent<Image>();
        Button skipbtn = skip.GetComponent<Button>();
        Sprite skipsp = Instantiate(Resources.Load<Sprite>("ConfigImages/Button 2"));
        Image skipbuttonImage = skipbtn.GetComponent<Image>();
        skipbuttonImage.sprite = skipsp;
        autobuttonImage.sprite = autosp;
    }
    private void CloseMenuPanel()
    {
        menuPanel.SetActive(false);
        CloseButtonAudio.Play();
    }
    private void ViewSavePanel()
    {
        if (ScenarioPanel.activeSelf)
        {
            currentObject = ScenarioPanel;
        }
        else
        {
            currentObject = TitlePanel;
        }
        savePanel.SetActive(true);
        ScenarioPanel.SetActive(false);
        TitlePanel.SetActive(false);
        OpenButtonAudio.Play();
    }
    private void CloseSavePanel()
    {
        savePanel.SetActive(false);
        currentObject.SetActive(true);
        CloseButtonAudio.Play();
    }
    private void ViewTitlePanel()
    {
        TitlePanel.SetActive(true);
        ScenarioPanel.SetActive(false);
        bgmAudioSource.clip = LoadTitleAudioClip();
        bgmAudioSource.Play();
        bgmAudioSource.volume = 1 * BGMSlider.value;
        bgmAudioSource.loop = true;
    }
    private void CloseTitlePanel()
    {
        TitlePanel.SetActive(false);
        ScenarioPanel.SetActive(true);
    }
    private void ViewConfigPanel()
    {
        if (ScenarioPanel.activeSelf)
        {
            currentObject = ScenarioPanel;
        }
        else
        {
            currentObject = TitlePanel;
        }
        ConfigPanel.SetActive(true);
        TitlePanel.SetActive(false);
        ScenarioPanel.SetActive(false);
        OpenButtonAudio.Play();
    }
    private void CloseConfigPanel()
    {
        ConfigPanel.SetActive(false);
        currentObject.SetActive(true);
        CloseButtonAudio.Play();
    }

    private void PushCloseButton()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
        TitleConfirmPanel.SetActive(true);
    }

    private void GameClose()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private void GameCloseCansel()
    {
        TitleConfirmPanel.SetActive(false);
    }

    /**
    * BGMの設定
    */
    private void SetBackgroundMusic(string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_BGM, "");
        SetAudioSource(cmd, parameter, bgmAudioSource);
    }

    /**
     * 効果音の設定
     */
    private void SetSoundEffect(string name, string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_SE, "");
        name = name.Substring(name.IndexOf('"') + 1, name.LastIndexOf('"') - name.IndexOf('"') - 1);
        AudioSource audio = _seList.Find(n => n.name == name);
        if (audio == null)
        {
            audio = Instantiate(Resources.Load<AudioSource>(prefabsDirectory + SE_AUDIOSOURCE_PREFAB), seAudioSources.transform);
            audio.name = name;
            _seList.Add(audio);
        }
        SetAudioSource(cmd, parameter, audio);

    }

    /**
     * 音声の設定
     */
    private void SetAudioSource(string cmd, string parameter, AudioSource audio)
    {
        cmd = cmd.Replace(" ", "");
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        switch (cmd)
        {
            case COMMAND_PLAY:
                audio.Play();
                break;
            case COMMAND_MUTE:
                audio.mute = ParameterToBool(parameter);
                break;
            case COMMAND_SOUND:
                audio.clip = LoadAudioClip(parameter);
                break;
            case COMMAND_VOLUME:
                audio.volume = float.Parse(parameter) * BGMSlider.value;
                break;
            case COMMAND_PRIORITY:
                audio.priority = int.Parse(parameter);
                break;
            case COMMAND_LOOP:
                audio.loop = ParameterToBool(parameter);
                break;
            case COMMAND_FADE:
                FadeSound(audio, parameter);
                break;
            case COMMAND_ACTIVE:
                audio.gameObject.SetActive(ParameterToBool(parameter));
                break;
            case COMMAND_DELETE:
                _seList.Remove(audio);
                Destroy(audio.gameObject);
                break;
        }
    }

    /**
    * 音声ファイルを読み出し、インスタンス化する
    */
    private AudioClip LoadAudioClip(string name)
    {
        return Instantiate(Resources.Load<AudioClip>(audioClipsDirectory + name));
    }
    private AudioClip LoadTitleAudioClip()
    {
        return Instantiate(Resources.Load<AudioClip>("TitleResources/titleBGM"));
    }
    /**
     * 音声にフェードをかける
     */
    private void FadeSound(AudioSource audio, string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        StartCoroutine(FadeSound(audio, int.Parse(ps[0]), int.Parse(ps[1])));
    }

    /**
    * 音のフェードを行うコルーチン
    */
    private IEnumerator FadeSound(AudioSource audio, float time, float volume)
    {
        float vo = (volume - audio.volume) / (time / Time.deltaTime);
        bool isOut = audio.volume > volume;
        while ((!isOut && audio.volume < volume) || (isOut && audio.volume > volume))
        {
            audio.volume += vo;
            yield return null;
        }
        audio.volume = volume;
    }


    public void Qsave()
    {
        PlayerPrefs.SetInt("Qsave", sc);
        Debug.Log("セーブしました　ページ番号" + sc);
        OpenButtonAudio.Play();
    }

    public void Qload()
    {
        sc = 0;
        isLoad = true;
        Init();
        OpenButtonAudio.Play();
    }

    private void NewGame()
    {
        sc = 0;
        menuPanel.SetActive(false);
        TitlePanel.SetActive(false);
        ScenarioPanel.SetActive(true);
        isLoad = false;
        Init();
    }
    public void Nsave()
    {
        AssetDatabase.Refresh();
        Debug.Log("SL:" + CTRL.SaveSlotNo);
        //日付ページ番号保存
        this.text1 = GameObject.Find("Time&Chapter" + CTRL.SaveSlotNo).GetComponent<Text>(); // textコンポーネント
        this.text1.text = "page:" + sc + "  " + System.DateTime.Now.ToString("yyyy/MM/dd/HH:mm");// <---- 追加4
        PlayerPrefs.SetInt("Page" + CTRL.SaveSlotNo, sc);
        this.text2 = GameObject.Find("saveText" + CTRL.SaveSlotNo).GetComponent<Text>(); // textコンポーネント
        this.text2.text = main;
        PlayerPrefs.SetInt("Nsave_saveText" + CTRL.SaveSlotNo, sc);
        File.Copy("Assets/Resources/SaveThumbnails/screenshottmp.png", "Assets/Resources/SaveThumbnails/screenshot" + CTRL.SaveSlotNo + ".png", true);
        //保存対象のオブジェクトを指定
        this.image1 = GameObject.Find("saveThumbnail" + CTRL.SaveSlotNo).GetComponent<Image>(); // Imageコンポーネント
        Debug.Log(this.image1.name);
        Sprite image1 = Resources.Load<Sprite>("Assets/Resources/SaveThumbnails/screenshot" + CTRL.SaveSlotNo);
        AssetDatabase.Refresh();
    }
    public void SaveThumbnailimage()
    {
        ScreenCapture.CaptureScreenshot("Assets/Resources/SaveThumbnails/screenshottmp.png");
    }

    /**
    * 対応するラベルまでジャンプする
    */
    private void JumpTo(string parameter)
    {
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        _pageQueue = _subScenes[parameter];
    }

    /**
    * 選択肢の設定
    */
    private void SetSelectButton(string name, string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_SELECT, "");
        Debug.Log("SetSelectButton() cmd = " + cmd);
        name = name.Substring(name.IndexOf('"') + 1, name.LastIndexOf('"') - name.IndexOf('"') - 1);
        Debug.Log("SetSelectButton() name = " + name);
        Button button = _selectButtonList.Find(n => n.name == name);
        Debug.Log("SetSelectButton() button = " + button);
        if (button == null)
        {
            button = Instantiate(Resources.Load<Button>(prefabsDirectory + SELECT_BUTTON_PREFAB), selectButtons.transform);
            button.name = name;
            button.onClick.AddListener(() => SelectButtonOnClick(name));
            _selectButtonList.Add(button);
        }
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        button.image.GetComponentInChildren<Text>().text = parameter;
    }

    /**
    * 選択肢がクリックされた
    */
    private void SelectButtonOnClick(string label)
    {
        foreach (Button button in _selectButtonList) Destroy(button.gameObject);
        _selectButtonList.Clear();
        JumpTo('"' + label + '"');
        choicePanel.SetActive(false);
        ShowNextPage();
    }
}