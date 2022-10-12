using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using UnityEngine.Purchasing;
using Anatomist;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event System.Action FinishedLoadingData;

    public static GameManager Instance;

    public static string CurrentCategory;

    public static QuizQuestion currentQuestion;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string lang = PlayerPrefs.GetString("LanguageTag", "notset");
        if (lang != "notset")
        {
            LocalizationManager.ChangeLanguage(lang);
        }
        LoadGameData();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteKey("Unlocked");
            Popup_QuizSelection.Instance?.UnlockAllCategories(false);
            print("Deleted Unlock");
        }
#endif
    }


    public AnatomistGameData gameData;

    void LoadGameData()
    {
        TextAsset ta = Resources.Load<TextAsset>("QuizData-en");
        gameData = null;
        if (ta != null)
        {
            gameData = JsonUtility.FromJson<AnatomistGameData>(ta.text);
            FinishedLoadingData?.Invoke();
            Screen_Search srch = Anatomist.Screen.Find("Search") as Screen_Search;
            srch.BuildSearchResults();

            Debug.Log("Loaded QuizData with " + gameData.categories.Count + " Category definitions and " + gameData.images.Count + " Image definitions.");
        }
        else
        {
            Debug.LogError("Could not load QuizData from Resources.");
        }
    }

    [System.Serializable]
    public class Tag
    {
        public string tag;
        public string value;
    }

    [SerializeField]
    List<Tag> tags = new List<Tag>();

    internal static string GetTag(string tag)
    {
        Tag val = Instance.tags.FirstOrDefault(x => x.tag == tag);

        if (val == null)
        {
            return ProcessTagValue(tag);
        }

        return ProcessTagValue(val.value);
    }

    internal static string ProcessTagValue(string tagValue)
    {
        switch (tagValue)
        {
            case "$GET_PRICE_FULLAPP":
                return Purchaser.Instance.GetPriceString(Purchaser.PRODUCT_FULL_APP);
            default:
                return tagValue;
        }
    }

    public bool CategoryExists(string categoryName)
    {
        if (gameData != null && gameData.images != null && gameData.categories != null)
        {
            QuizCategory cat = gameData.categories.FirstOrDefault(x => x.name.ToUpper() == categoryName.ToUpper());

            if (cat != null && cat.questions != null && cat.questions.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    public void AssignSpriteToCategory(string categoryName, Sprite spr)
    {
        if (gameData != null && gameData.images != null && gameData.categories != null)
        {
            for (int i = 0; i < gameData.categories.Count; i++)
            {
                if (gameData.categories[i].name == categoryName)
                {
                    gameData.categories[i].CategorySprite = spr;
                    Debug.Log("Assigned" + spr.name + " to " + gameData.categories[i].name);
                }
            }
        }
    }


    public void AttachPinToImage(Image pin, Image image, QuizQuestion question)
    {
        RectTransform imageRect = image.GetComponent<RectTransform>();
        Vector2 size = RectTransformUtility.CalculateRelativeRectTransformBounds(imageRect).size;
        RectTransform pinRect = pin.GetComponent<RectTransform>();

        pinRect.SetParent(imageRect);

        //Debug.Log("Pin: x = " + question.pin.x + " y = " + question.pin.y);
        if (question.pin.x <= 0f || question.pin.y <= 0f)
            pin.gameObject.SetActive(false);
        else
            pin.gameObject.SetActive(true);

        pinRect.anchorMin = new Vector2(question.pin.x, 1 - question.pin.y);
        pinRect.anchorMax = new Vector2(question.pin.x, 1 - question.pin.y);
        pinRect.pivot = new Vector2(0.5f, 0f);

        pinRect.anchoredPosition = Vector2.zero;
    }

    public string[] GetWrongAnswers(string category, string subcategory, QuizQuestion question)
    {
        List<string> wrongAnswers = new List<string>();
        if (gameData != null && gameData.images != null && gameData.categories != null)
        {
            List<QuizQuestion> qs = new List<QuizQuestion>();
            for (int i = 0; i < 3; i++)
            {
                //Try to find only answers that use the same image as the current question.

                QuizCategory c = gameData.categories.First(x => x.name == (question.isPathology && !category.ToUpper().Contains("PATHOLOGY") ? category + " Pathology" : category));



                bool allowOtherImageQuestions = false;
                int reselectCount = 0;

                ReQuery:

                if (allowOtherImageQuestions || question.type == QuizType.Text || reselectCount >= 5)
                {
                    //Couldnt find any more unique answers from the same image, so lets just select a random answer from all questions in the category.
                    qs = c.questions.Where(x => x.AnswerText != question.AnswerText).ToList();
                }
                else
                {
                    //This is probably the first time around, so get the other answers based on the same image if we can.
                    qs = c.questions.Where(x => x.AnswerText != question.AnswerText && x.QuestionText == question.QuestionText).ToList();
                }

                Reselect:
                if (qs != null && qs.Count > 0)
                {
                    int rand = UnityEngine.Random.Range(0, qs.Count);

                    //Debug.Log("QS: " + qs.Count + " rand: " + rand + " reselect: " + reselectCount + " didRequery: " + allowOtherImageQuestions);

                    if (reselectCount < 5 || allowOtherImageQuestions)
                    {
                        if (wrongAnswers.Contains(qs[rand].AnswerText))
                        {
                            reselectCount++;
                            goto Reselect;
                        }

                        wrongAnswers.Add(qs[rand].AnswerText);
                    }
                    else if (!allowOtherImageQuestions)
                    {

                        allowOtherImageQuestions = true;
                        goto ReQuery;

                    }
                }
                else
                {
                    allowOtherImageQuestions = true;
                    goto ReQuery;
                }


            }

        }
        return wrongAnswers.ToArray();
    }


    public QuizQuestion GetNewQuestion(string category, string subcategory, List<QuizQuestion> alreadyPicked, int targetQuestionCount)
    {
        Debug.LogError("GetNewQuestion");
        if (gameData != null && gameData.images != null && gameData.categories != null)
        {
            Debug.Log("GETTING NEW QUESTION FOR: " + category);

            //Define c here by selecting the category that was passed in, it will be overwritten if there is a subcategory, or if the selected category is NONpathology and the pathology chance procs
            QuizCategory c = gameData.categories.FirstOrDefault(x => x.name == category);

            if (!string.IsNullOrEmpty(subcategory))
            {
                QuizCategory a = c;
                //A Subcategory was selected, lets remove questions from our list that dont fall into that subcategory
                c = new QuizCategory();
                c.name = a.name;
                c.CategorySprite = a.CategorySprite;
                c.questions = new List<QuizQuestion>();
                c.questions = a.questions.FindAll(x => x.subCategory == subcategory);
                Debug.LogWarning("Added " + c.questions.Count + " subcategory questions for " + subcategory);
            }
            else if (string.IsNullOrEmpty(subcategory) && alreadyPicked != null && alreadyPicked.Count >= 1 && !category.ToUpper().Contains("PATHOLOGY") && CategoryExists(category + " Pathology"))
            {
                //We want to sometimes mix in some pathology questions, but not if we're playing a subcategory
                int pathologyChance = 10;
                int pathologyRandom = UnityEngine.Random.Range(0, 100);

                if (pathologyRandom < pathologyChance)
                {
                    QuizCategory pathologyCategory = gameData.categories.FirstOrDefault(x => x.name == category + " Pathology");

                    if (pathologyCategory != null)
                    {
                        c = pathologyCategory;
                        Debug.LogWarning("Picking a new question from pathology category: " + pathologyCategory.name);
                    }
                }
            }


            if (c != null)
            {

                PickAgain:
                int rand = UnityEngine.Random.Range(0, c.questions.Count);

                //Only enforce duplicate prevention if we have enough questions to pick from.
                bool atLeastOneDuplicate = alreadyPicked.Contains(c.questions[rand]);
                bool categoryHasEnough = c.questions.Count > targetQuestionCount;
                bool repeatingLastDrawAgain = alreadyPicked == null || alreadyPicked.Count == 0 ? false : alreadyPicked[alreadyPicked.Count - 1] == c.questions[rand];

                //Did we pick this question once or more already?
                if (atLeastOneDuplicate)
                {
                    //If so, if we can afford to be picky, pick again
                    if (categoryHasEnough)
                    {
                        goto PickAgain;
                    }
                    else
                    {
                        //Otherwise, only be a little bit picky and make sure we don't pick the same one twice in a row.
                        if (repeatingLastDrawAgain)
                        {
                            goto PickAgain;
                        }
                    }
                }

                QuizQuestion selectedQuestion = c.questions[rand];

                Debug.LogWarning("Picked Question: " + selectedQuestion.question + " subcat? " + subcategory + " q.subcat? " + selectedQuestion.subCategory);

                return selectedQuestion;
            }
            else
            {
                Debug.LogError("c == null when trying to pick NextQuestion");
            }
        }
        else
        {
            Debug.LogError("CANNOT SELECT NEXT QUESTION BECAUSE GAME DATA IS NULL!");
        }

        return null;
    }


    [System.Serializable]
    public class QuizDifficulty
    {
        public string name;
        public int questionCount;
        public string category;
        public string subcategory;
        public bool unlocked
        {
            get
            {
                //TO DO: REPLACE THIS WITH A CHECK TO SEE IF THE USER OWNS THE RIGHT IN APP PURCHASE PRODUCT
                return false;
            }
        }
    }

    public static string CategoryToLeaderboardID(string category)
    {
        return category.Substring(5).Replace("Pathology", "").Replace("pathology", "").Replace("PATHOLOGY", "").Replace(" ", "");
    }

    public static string FriendlyDifficultyName(string name)
    {
        return name.ToUpper().Contains("PATHOLOGY") ? "Pathology Only" : name.Substring(5);//(name[2] == ' ' ? name.Substring(5) : name);
    }

    /// <summary>
    /// Use this for generating the difficulty selection list for the difficulty popup window
    /// </summary>
    /// <param name="Category">The category that you need a difficulty list for</param>
    /// <returns></returns>
    public QuizDifficulty[] GenerateDifficulties(string Category)
    {
        List<QuizDifficulty> d = new List<QuizDifficulty>();

        d.Add(new QuizDifficulty()
        {
            name = LocalizationManager.localization.btn_EasyCategory,
            category = Category,
            subcategory = "",
            questionCount = 10
        });

        d.Add(new QuizDifficulty()
        {
            name = LocalizationManager.localization.btn_MediumCategory,
            category = Category,
            subcategory = "",
            questionCount = 50
        });

        d.Add(new QuizDifficulty()
        {
            name = LocalizationManager.localization.btn_HardCategory,
            category = Category,
            subcategory = "",
            questionCount = 100
        });

        d.Add(new QuizDifficulty()
        {
            name = LocalizationManager.localization.btn_ExpertCategory,
            category = Category,
            subcategory = "",
            questionCount = 150
        });

        string pathologyCategory = Category + " Pathology";

        if (CategoryExists(pathologyCategory))
        {
            QuizCategory pathology = gameData.categories.First(x => x.name.ToUpper() == pathologyCategory.ToUpper());
            d.Add(new QuizDifficulty()
            {
                name = LocalizationManager.localization.btn_PathologyOnly,
                category = pathology.name,
                subcategory = "",
                questionCount = pathology.questions.Count
            });
        }

        QuizDifficulty[] subcats = GetSubCategories(Category);
        if (subcats != null && subcats.Length > 0)
            d.AddRange(subcats);

        return d.ToArray();
    }

    private QuizDifficulty[] GetSubCategories(string Category)
    {
        QuizCategory cat = gameData.categories.FirstOrDefault(x => x.name == Category);

        if (cat != null)
        {
            List<string> subCategories = new List<string>();
            List<string> subCategories_English = new List<string>();

            foreach (QuizQuestion question in cat.questions)
            {
                switch (LocalizationManager.CurrentLanguage.tag)
                {
                    case "es":
                        if (!subCategories.Contains(question.subCategory_SPANISH) && !string.IsNullOrEmpty(question.subCategory_SPANISH))
                        {
                            subCategories.Add(question.subCategory_SPANISH);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "ru":
                        if (!subCategories.Contains(question.subCategory_RUSSIAN) && !string.IsNullOrEmpty(question.subCategory_RUSSIAN))
                        {
                            subCategories.Add(question.subCategory_RUSSIAN);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "zh-Hans":
                        if (!subCategories.Contains(question.subCategory_CHINESE) && !string.IsNullOrEmpty(question.subCategory_CHINESE))
                        {
                            subCategories.Add(question.subCategory_CHINESE);
                            Debug.Log("Added subcategory: " + question.subCategory_CHINESE);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "it":
                        if (!subCategories.Contains(question.subCategory_ITALIAN) && !string.IsNullOrEmpty(question.subCategory_ITALIAN))
                        {
                            subCategories.Add(question.subCategory_ITALIAN);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "ar":
                        if (!subCategories.Contains(question.subCategory_ARABIC) && !string.IsNullOrEmpty(question.subCategory_ARABIC))
                        {
                            subCategories.Add(question.subCategory_ARABIC);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "fr":
                        if (!subCategories.Contains(question.subCategory_FRENCH) && !string.IsNullOrEmpty(question.subCategory_FRENCH))
                        {
                            subCategories.Add(question.subCategory_FRENCH);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                    case "en":
                    default:
                        if (!subCategories.Contains(question.subCategory) && !string.IsNullOrEmpty(question.subCategory))
                        {
                            subCategories.Add(question.subCategory);
                            subCategories_English.Add(question.subCategory);
                        }
                        break;
                }
            }

            List<QuizDifficulty> d = new List<QuizDifficulty>();

            for (int i = 0; i < subCategories.Count; i++)
            {
                string subcat = subCategories_English[i];
                string displayname = subCategories[i];
                d.Add(new QuizDifficulty()
                {
                    name = displayname,
                    category = Category,
                    subcategory = subcat,
                    questionCount = 25
                });
            }

            d = d.OrderBy(x => x.name).ToList();

            return d.ToArray();
        }

        return null;

    }

    public string GetSource(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
            return "";

        QuizImage img = gameData.images.FirstOrDefault(x => x.imageName == imageName);

        if (img != null)
        {
            return img.source;
        }

        return "";
    }


    [ContextMenu("EXPORT SINGLE LINE DATA FILE")]
    public void ExPORT()
    {
        string json = JsonUtility.ToJson(gameData, false);

        File.WriteAllText(Application.dataPath + "/_App/Resources/ONELINER.txt", json);
    }

    [ContextMenu("CHECK FOR MISSING IMAGES")]
    public void CheckImagesExist()
    {
        List<QuizQuestion> badqs = new List<QuizQuestion>();

        foreach (var c in gameData.categories)
        {
            int i = 0;
            foreach (var q in c.questions.Where(x => x.type == QuizType.Image).ToList())
            {
                if (q.QuestionText == null)
                {
                    Debug.LogError("NO QUESTION TEXT SET: " + c.name + " # " + i.ToString("n0"));

                    badqs.Add(q);
                }
                else
                {
                    Texture2D questionTexture = q.Texture;

                    if (questionTexture == null)
                    {
                        Debug.LogError(q.QuestionText + " image not found...");
                        badqs.Add(q);
                    }
                }
                i++;
            }
        }

        Debug.LogWarning("BAD QUESTION LIST: " + JsonUtility.ToJson(badqs, true));
    }
}
