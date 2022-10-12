using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject Scoreboardpanel;
    public Transform scoreboardContent;
    public Button leaderboardButton;

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
        FirebaseAuthenticationsHandler.instance.OnScoreboardButton();
    }
}
