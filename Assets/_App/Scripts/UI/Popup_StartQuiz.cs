using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_StartQuiz : PopupWindow
{
    [SerializeField]
    GameObject star1, star2, star3, star4, friendRanks;

    [SerializeField]
    Text title;

    [SerializeField]
    Image categoryIcon;

    [SerializeField]
    Sprite dummyCategoryIcon;

    [SerializeField]
    private Button startQuizButton;

    [SerializeField]
    Popup_QuizSelection difficultyPopup;

    public GameObject GifPlayer;

    public RectTransform friendRankSection;
    public CanvasGroup friendRankCanvasGroup;

    public GameObject[] friendRankObjects;

    bool gotFriendRanks = false;

    [SerializeField]
    Texture defaultAvatar;

    string category;

    //private void OnEnable()
    //{

    //    string lbids = "";

    //    GameManager.Instance.gameData.categories.ForEach((c) => 
    //    {
    //        string cat = GameManager.CategoryToLeaderboardID(c.name);
    //        if(!lbids.Contains(cat))
    //            lbids += cat + "\n";
    //    });

    //    Debug.Log("HERES A LIST OF ALL THE leaderboard ID's you need to make: \n" + lbids);
    //}

    //private void ShowRanks(string category)
    //{
    //    friendRankCanvasGroup.alpha = 0f;
    //    friendRankSection.sizeDelta = new Vector2(friendRankSection.sizeDelta.x, 0f);

    //    string leaderboardID = "CategoryLeaderboard.category." + GameManager.CategoryToLeaderboardID(category);

    //    Debug.Log("About to try and get leaderboard for " + leaderboardID);
    //    if (FB.IsLoggedIn && GS.Available && GS.Authenticated && !gotFriendRanks)
    //    {
    //        Debug.LogWarning("We are authenticated and ready to get leaderboard for " + leaderboardID);
    //        GSManager.GetLeaderboard(false, leaderboardID, (list) =>
    //        {
    //            if (this.category != category) //Just make sure that the user didnt close the popup and open a different by the time this message came back from the server.
    //                return; 

    //            var entries = list.OrderBy(x => x.rank).ToList();
    //            entries.RemoveAll(x => x.userName.ToUpper() == "ANONYMOUS");

    //            if (entries != null && entries.Count > 0)
    //            {
    //                int i = 0;
    //                foreach (var friendObj in friendRankObjects)
    //                {
    //                    if (i >= entries.Count)
    //                    {
    //                        friendObj.SetActive(false);
    //                    }
    //                    else
    //                    {
    //                        Text nameObj = friendObj.transform.Find("Name").GetComponent<Text>();
    //                        nameObj.resizeTextForBestFit = false;
    //                        nameObj.fontSize = 17;

    //                        string namestring = entries[i].userName;

    //                        namestring = namestring.Length > 17 ? namestring.Substring(0, 15) + ".." : namestring; 


    //                        nameObj.text = namestring;
    //                        friendObj.transform.Find("Score").GetComponent<Text>().text = entries[i].score.ToString("n0");
    //                        if (entries[i].externalIds.ContainsKey("FB"))
    //                        {
    //                            string fbID = entries[i].externalIds.GetString("FB");
    //                            SocialManager.Instance.GetFacebookProfileImage(fbID, friendObj.transform.Find("RawImage").GetComponent<RawImage>(), null);
    //                        }
    //                        else
    //                        {
    //                            friendObj.transform.Find("RawImage").GetComponent<RawImage>().texture = defaultAvatar;
    //                        }
    //                    }
    //                    i++;
    //                }

    //                gotFriendRanks = true;

    //                friendRankCanvasGroup.DOFade(1f, 0.1f);
    //                friendRankSection.DOSizeDelta(new Vector2(friendRankSection.sizeDelta.x, 140f), 1.5f);
    //            }

    //        });
    //    }
    //}

    public override void Init(string category, bool Unlocked = false, Sprite icon = null)
    {

        this.category = category;
        Debug.Log(category);
        gotFriendRanks = false;
        GifPlayer.GetComponent<GifPlayer>().gifs = Resources.Load<GifClass>(category);
        //if (quizData.userProgress > 0)
        //{
        //    UpdateStars(Mathf.Clamp01((float)quizData.userProgress));
        //}
        //else
        //{
        //    UpdateStars(0f);
        //}
        var c = GameManager.Instance.gameData.categories.FirstOrDefault(x => x.name.ToUpper() == category.ToUpper());
        if (c == null)
        {
            Debug.LogError("Category \"" + category + "\" not found.");
            return;
        }
        string categoryTitle = c.GetLocalizedName();

        title.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            title.GetComponent<ArabicText>().enabled = true;
            title.text = categoryTitle;
            title.GetComponent<ArabicText>().Text = title.text;
            title.GetComponent<ArabicText>().Refresh();
        }

        if (icon != null)
            categoryIcon.sprite = icon;


        Text startQuiz = startQuizButton.GetComponentInChildren<Text>();
        startQuiz.text = LocalizationManager.Instance.currentLocalization.startQuiz;

        startQuiz.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            startQuiz.GetComponent<ArabicText>().enabled = true;
            startQuiz.GetComponent<ArabicText>().Text = startQuiz.text;
            startQuiz.GetComponent<ArabicText>().Refresh();
        }
        startQuizButton.onClick.RemoveAllListeners();
        startQuizButton.onClick.AddListener(() =>
        {
            //Unlocked = Purchaser.Instance.GetPurchasedState(Purchaser.PRODUCT_FULL_APP);
            difficultyPopup.Init(category, true);
            difficultyPopup.Open();
        });

        UpdateStars(GetProgress(category));

        //  ShowRanks(category);
    }

    float GetProgress(string category)
    {
        List<QuizCategory> categories = GameManager.Instance.gameData.categories.FindAll(x => x.name.ToUpper().Replace(" PATHOLOGY", "") == category.ToUpper());

        QuizQuestion q;

        int totalQuestionCount = 0;
        int completedCount = 0;

        for (int x = 0; x < categories.Count; x++)
        {
            for (int y = 0; y < categories[x].questions.Count; y++)
            {
                q = categories[x].questions[y];
                completedCount += bool.Parse(PlayerPrefs.GetString(q.ClassificationText, "false")) == true ? 1 : 0;
                totalQuestionCount++;
            }
        }

        return (float)completedCount / (float)totalQuestionCount;
    }

    void UpdateStars(float userProgress)
    {
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        star4.SetActive(false);

        if (userProgress >= 0.25f)
        {
            star1.SetActive(true);
        }

        if (userProgress >= 0.5f)
        {
            star2.SetActive(true);
        }

        if (userProgress >= 0.75f)
        {
            star3.SetActive(true);
        }

        if (userProgress >= 1f)
        {
            star4.SetActive(true);
        }
    }

    public override void Open()
    {
        base.Open();

    }

    public override void Close()
    {
        base.Close();
    }
}
