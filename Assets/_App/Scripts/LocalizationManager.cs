using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Localization
{
    public string startQuiz;
    public string chooseLanguage;
    public string timerEnabled;
    public string bonusSection;
    public string searchTop;
    public string searchFieldEmpty;
    public string aboutBody;
    public string feedbackBody1;
    public string feedbackPositive_android;
    public string feedbackPositive_ios;
    public string feedbackBody2;
    public string feedbackNegative;
    public string feedbackBody3;
    public string version;
    public string copyright;
    public string difficultyDialogText;
    public string question;
    public string score;
    public string bestScore;
    public string areYouNumber1;
    public string bigDeal;
    public string btn_ResumeQuiz;
    public string btn_QuitQuiz;
    public string btn_StartQuiz;
    public string btn_GlobalRanking;
    public string btn_FriendRanking;
    public string btn_EasyCategory = "Easy";
    public string btn_MediumCategory = "Medium";
    public string btn_HardCategory = "Hard";
    public string btn_ExpertCategory = "Expert";
    public string btn_PathologyOnly = "Pathology Only";
    public string pause;
    public string buyFullAppTitle;
    public string buyFullAppBody;
    public string btn_BuyPremium;
    public string Leaderboard;
    public string Congrats;
    public string About;
    public string Feedback;
    public string YourScore;
    public string AmountCorrect;
    public string Next;
    public string TryBeatingMe;
    public string SmartyPants;
    public string Correct;
    public string Quiz;
    public string ReviewList;
    public string RestorePurchases;
    public string TranslationDisclaimer;
}

[System.Serializable]
public class LanguageData
{
    public string name;
    public string tag;
    public Sprite sprite;
    public Font font;
    public Sprite removeAdsBanner;
    public bool isRightToLeft;
}

public class LocalizationManager : MonoBehaviour {

    public static event System.Action OnLanguageChanged;
    public static event System.Action<Font> OnFontChanged;

    public static Localization localization;

    public Localization currentLocalization;

    public List<LanguageData> Languages = new List<LanguageData>();

    private static LanguageData currentLanguage;

    public List<Text> simpleTexts = new List<Text>();

    public static LanguageData CurrentLanguage
    {
        get
        {
            if (currentLanguage == null)
            {
                currentLanguage = Instance.Languages.Find(x => x.tag == "en");
            }

            return currentLanguage;
        }
        set
        {
            currentLanguage = value;
        }
    }

    public static LocalizationManager Instance;

    public Font defaultFont;

    public Image removeAdsBannerImage;
    

    public static bool isRightToLeftLanguage
    {
        get
        {
            return CurrentLanguage.isRightToLeft;
        }
    }

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        LoadLocalization();
    }
    
    private static void LoadLocalization()
    {
        string languageTag = PlayerPrefs.GetString("LanguageTag", "en");
        string filename = "strings-" + languageTag;

        TextAsset ta = Resources.Load<TextAsset>("localization/" + filename);

        if (ta != null)
        {
            localization = JsonUtility.FromJson<Localization>(ta.text);
            Instance.currentLocalization = localization;
            OnLanguageChanged?.Invoke();
        }
        Debug.Log("Language Changed " + languageTag);
        
       // LocalizationManager.Instance.StartCoroutine(ChnageLanguage());
       
    }
    static IEnumerator ChnageLanguage()
    {
        for (int i = 0; i < LocalizationManager.Instance.simpleTexts.Count; i++)
        {
            if (LocalizationManager.Instance.simpleTexts[i].GetComponent<ArabicText>() != null)
            {
                Destroy(LocalizationManager.Instance.simpleTexts[i].GetComponent<ArabicText>());
            }
        }

        yield return new WaitForSeconds(1.5f);
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
        for (int i = 0; i < LocalizationManager.Instance.simpleTexts.Count;)
        {
                string a = LocalizationManager.Instance.simpleTexts[i].text;
                Debug.Log(a);
            LocalizationManager.Instance.simpleTexts[i].gameObject.AddComponent<ArabicText>().Text = a;
            i++;
        }
    }

        

    }
    public static void ChangeLanguage(string languageTag)
    {
        PlayerPrefs.SetString("LanguageTag", languageTag);

        CurrentLanguage = Instance.Languages.Find(x => x.tag == languageTag);

        //If the language has a custom font assigned, change to it now
        OnFontChanged?.Invoke(CurrentLanguage.font != null ? CurrentLanguage.font : Instance.defaultFont);

        UpdateAdBanner();

        LoadLocalization();
    }

    static void UpdateAdBanner()
    {
        Instance.removeAdsBannerImage.sprite = CurrentLanguage.removeAdsBanner;
    }

    [ContextMenu("Switch to English")]
    public void English()
    {
        LocalizationManager.ChangeLanguage("en");
    }
    [ContextMenu("Switch to Spanish")]
    public void Spanish()
    {
        LocalizationManager.ChangeLanguage("es");
    }
}
