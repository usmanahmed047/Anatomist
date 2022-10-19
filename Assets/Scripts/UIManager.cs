using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject Scoreboardpanel;
    public Transform scoreboardContent;
    //public Button leaderboardButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {

    }

    public void OnLeaderboardButton()
    {
#if !UNITY_EDITOR
        FirebaseAuthenticationsHandler.instance.OnScoreboardButton();
#endif
    }
}
