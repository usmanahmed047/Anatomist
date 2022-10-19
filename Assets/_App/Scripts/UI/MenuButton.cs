using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuButton : MonoBehaviour
{
    public static MenuButton Instance;
    public static event System.Action<string> ButtonSelected;

    // public string ButtonName;

    [SerializeField]
    public GameObject buttonsContainer;

    public static bool isMuted = false;

    void Start()
    {
        Instance = this;
        buttonsContainer.SetActive(false);
    }

    public void OnButtonSelected(string ButtonName)
    {
        if (ButtonName == "CloseMenu")
        {
            buttonsContainer.SetActive(false);
        }
        else if (ButtonName == "OpenMenu")
        {
            buttonsContainer.SetActive(true);
        }
        else if (ButtonName == "Mute")
        {
           // isMuted = !isMuted;
            UpdateMute();
        }
        else
        {
            if (ButtonSelected != null && !string.IsNullOrEmpty(ButtonName))
            {
                ButtonSelected(ButtonName);
                buttonsContainer.SetActive(false);
            }
        }
        MusicManager.Instance.PlaySound("swipe");
    }

    void UpdateMute()
    {
        MusicManager.Instance.ToggleSound();
    }

    public void SignOut()
    {
        FirebaseAuthenticationsHandler.instance.SignOut();
    }

}

