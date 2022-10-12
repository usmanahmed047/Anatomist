using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_leaderboard : MonoBehaviour
{
    public static Popup_leaderboard Instance;
    [SerializeField]
    Transform GlobalContent;
    [SerializeField]
    Transform FriendsContent;
    [SerializeField]
    GameObject EntryObject;
    [SerializeField]
    Text btn_FriendRanking;
    [SerializeField]
    Text btn_GlobalRanking;
    [SerializeField]
    Text areYouNumber1;

    [SerializeField]
    Texture defaultAvatar;

    public List<LeaderboardEntry> GlobalLeaderboardData = new List<LeaderboardEntry>();
    public List<LeaderboardEntry> FriendsLeaderboardData = new List<LeaderboardEntry>();


    public Text myScore;


    private void Start()
    {
        Localize();
        LocalizationManager.OnLanguageChanged += Localize;
    }

    void GetLeaderboards()
    {
      //  GSManager.GetLeaderboard(false, (entries) => { StartCoroutine(FillLeaderboard("Global", entries)); });
      //  GSManager.GetLeaderboard(true, (entries) => { StartCoroutine(FillLeaderboard("Friends", entries)); });
    }

    void OnEnable()
    {
        ClearChild(GlobalContent);
        ClearChild(FriendsContent);

        //GSManager.GetPersonalRanking((System.Action<LeaderboardEntry>)((entry) => 
        //{
        //    myScore.text = entry.totalScore.ToString("n0") + " (#" + entry.rank.ToString("n0") + ")";
        //}));

        GetLeaderboards();
    }

    public IEnumerator FillLeaderboard(string BoardToFill, List<LeaderboardEntry> LeaderboardData)
    {
        //Wait for just a moment to try and provide enough time for it to download and assign all the avatar images to the leaderboard data objects.
        //while (!FB.IsLoggedIn)
        //    yield return null;


        Transform Content = null;

        if (BoardToFill == "Global")
            Content = GlobalContent;
        else if (BoardToFill == "Friends")
            Content = FriendsContent;

        ClearChild(Content);
        int displayRank = 1;
        for (int i = 0; i < LeaderboardData.Count; i++)
        {
            if (LeaderboardData[i].userName.ToUpper().Contains("ANONYMOUS"))
                continue;

            Transform t = Instantiate(EntryObject, Content).transform;
            t.gameObject.name = "Entry " + LeaderboardData[i].rank;
            t.Find("Rank").GetComponent<Text>().text = LeaderboardData[i].rank.ToString("n0");//displayRank.ToString("n0");
            t.Find("Name").GetComponent<Text>().text = LeaderboardData[i].userName.ToString();
            t.Find("Score").GetComponent<Text>().text = LeaderboardData[i].score.ToString("n0");

            RawImage ri = t.Find("Avatar").GetComponent<RawImage>();
            ri.texture = defaultAvatar;

            if (LeaderboardData[i].externalIds != null && LeaderboardData[i].externalIds.ContainsKey("FB"))
            {
                Debug.Log("User has FB in externalIds, retrieving profile image from Facebook...");

              //  SocialManager.Instance.GetFacebookProfileImage(LeaderboardData[i].externalIds.GetString("FB"), ri, null);
            }
            else
            {
                ri.texture = defaultAvatar;
            }

            displayRank++;
            //yield return new WaitForSeconds(0.25f);
            //yield return new WaitForEndOfFrame();
            yield return null;
        }

    }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ClearChild(Transform tr)
    {
        foreach (Transform TR in tr)
        {
            //  TR.gameObject.SetActive(false);
            Destroy(TR.gameObject);
        }
    }
    void Localize()
    {
        btn_FriendRanking.text = LocalizationManager.localization.btn_FriendRanking;
        btn_GlobalRanking.text = LocalizationManager.localization.btn_GlobalRanking;
        areYouNumber1.text = LocalizationManager.localization.areYouNumber1;
    }
}
