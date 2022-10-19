using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anatomist;
using System.Linq;
using System.Runtime.Remoting.Contexts;

public class Popup_QuizSelection : PopupWindow
{
    public static Popup_QuizSelection Instance;

    [SerializeField]
    GameObject difficultyEntryPrefab;

    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    GameObject Content;

    [SerializeField]
    Text difficultyDialogText;

    public string category;

    [SerializeField]
    Toggle timerEnabledToggle;

    [SerializeField]
    Text timerEnabledLabel;

    void Awake()
    {
        Instance = this;
        timerEnabledToggle.onValueChanged.RemoveAllListeners();
        timerEnabledToggle.onValueChanged.AddListener((isOn) =>
        {
            Screen_Quiz.timerEnabled = isOn;
        });
        timerEnabledToggle.isOn = true;
    }


    private void Start()
    {
        Localize();
        LocalizationManager.OnLanguageChanged += Localize;
        Purchaser.PurchaseComplete += UnlockAllCategories;
    }


    public override void Init(string category, bool Unlocked = false, Sprite icon = null)
    {
        this.category = category;
        GameManager manager = GameManager.Instance;

        GameManager.QuizDifficulty[] difficulties = manager.GenerateDifficulties(category);

        //if (Unlocked == 0) 
        //    Unlocked = PlayerPrefs.GetInt("Unlocked", 0);

        if (difficulties.Length > 0)
        {
            foreach (Transform tr in Content.transform)
            {
                Destroy(tr.gameObject);
            }

            for (int i = 0; i < difficulties.Length; i++)
            {
                GameObject GO = Instantiate(difficultyEntryPrefab, Content.transform);
                ///Unlock levels based on purchase
                //Going to unlock just one for testing
                if (i == 0)
                    GO.GetComponent<DifficultySelectionBtn>().UpdateDifficulty(false, difficulties[i]);
                else
                    GO.GetComponent<DifficultySelectionBtn>().UpdateDifficulty(!Unlocked, difficulties[i]);

            }
        }

        scrollRect.verticalNormalizedPosition = 1;

    }

    public void UnlockAllCategories(bool Unlocked)
    {
        if (!string.IsNullOrEmpty(category))
            Init(category, Unlocked);
        Debug.Log("Unlocking all categories");
    }
    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    void Localize()
    {
        difficultyDialogText.text = LocalizationManager.localization.difficultyDialogText;

        difficultyDialogText.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            difficultyDialogText.GetComponent<ArabicText>().enabled = true;
            difficultyDialogText.GetComponent<ArabicText>().Text = difficultyDialogText.text;
            difficultyDialogText.GetComponent<ArabicText>().Refresh();
        }

        timerEnabledLabel.text = LocalizationManager.localization.timerEnabled;

        timerEnabledLabel.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            timerEnabledLabel.GetComponent<ArabicText>().enabled = true;
            timerEnabledLabel.GetComponent<ArabicText>().Text = timerEnabledLabel.text;
            timerEnabledLabel.GetComponent<ArabicText>().Refresh();
        }
    }

    public void StartGame(GameManager.QuizDifficulty difficulty)
    {
        GameManager manager = GameManager.Instance;
        List<GameQuizData> quizdata = new List<GameQuizData>();

        List<QuizQuestion> addedQuestions = new List<QuizQuestion>();

        if (manager.CategoryExists(category))
        {
            if (difficulty.category.ToUpper().Contains("PATHOLOGY") && !difficulty.category.ToUpper().Contains("ECG"))
            {
                addedQuestions.AddRange(manager.gameData.categories.FirstOrDefault(x => x.name.ToUpper() == difficulty.category.ToUpper()).questions);

                //WHEN PATHOLOGY IS SELECTED AS A DIFFICULTY
                    
                //Shuffle the questions up so they're always in a different order each time this category is played.
                var count = addedQuestions.Count;
                var last = count - 1;
                for (var i = 0; i < last; ++i)
                {
                    var r = UnityEngine.Random.Range(i, count);
                    var tmp = addedQuestions[i];
                    addedQuestions[i] = addedQuestions[r];
                    addedQuestions[r] = tmp;
                }

                foreach (QuizQuestion q in addedQuestions)
                {
                    quizdata.Add(new GameQuizData(difficulty.category, difficulty.subcategory, q, manager.GetWrongAnswers(difficulty.category, difficulty.subcategory, q)));
                }
                
            }
            else
            {
                for (int i = 0; i < difficulty.questionCount; i++)
                {
                    QuizQuestion ques = manager.GetNewQuestion(difficulty.category, difficulty.subcategory, addedQuestions, difficulty.questionCount);
                    quizdata.Add(new GameQuizData(difficulty.category, difficulty.subcategory, ques, manager.GetWrongAnswers(difficulty.category, difficulty.subcategory, ques)));
                    addedQuestions.Add(ques);
                }
            }
        }
        else
        {
            Debug.Log("Could not find category: " + category);
        }
        //QuizScene.GetComponent<Screen_Quiz>().UpdateQuizInfo(quizdata.ToArray());
        Anatomist.Screen.Find("Quiz").Open(new object[] { quizdata.ToArray() });

        ScreenManager.Instance.CloseAllPopupWindow();
    }

}

