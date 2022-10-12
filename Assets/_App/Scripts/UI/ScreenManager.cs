using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Anatomist
{

    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance;

        public Screen[] screens;
        public GameObject[] PopupWindows;

        void Awake()
        {
            Instance = this;
            MenuButton.ButtonSelected += MenuButton_OnMenuButtonSelected;

        }

        private void Start()
        {
            if (PlayerPrefs.GetString("LanguageTag", "notset") == "notset")
                Screen.Find("Language").Open(true);
            else
                Screen.Find("Home").Open();
            
        }

        void MenuButton_OnMenuButtonSelected(string buttonName)
        {
            print("buttonName:" + buttonName);

            if (buttonName.ToUpper() == "HOME" && Screen.current.GetType() == typeof(Screen_Quiz))
                AdsManager.Instance.ShowInterstitialAD();

            Screen.Find(buttonName).Open();
            
            MenuButton.Instance.buttonsContainer.SetActive(false);
        }
        public void OpenPopupWindow(string popupwindowname, string category, GameQuizData quizData = null, Sprite icon = null, bool closeOthers = true)
        {
            if(closeOthers) CloseAllPopupWindow();
            GameObject GO = PopupWindows.FirstOrDefault(x => x.name.ToUpper() == popupwindowname.ToUpper());

            if (icon == null)
                GO?.GetComponent<PopupWindow>().Init(category);
            else
                GO?.GetComponent<PopupWindow>().Init(category, icon: icon);

            GO?.GetComponent<PopupWindow>().Open();
      
        }
        public void ClosePopupWindow(string popupwindowname)
        {
            GameObject GO = PopupWindows.FirstOrDefault(x => x.name.ToUpper() == popupwindowname.ToUpper());
            GO?.GetComponent<PopupWindow>().Close();
        }
        public void CloseAllPopupWindow()
        {
            Debug.Log("CLOSING ALL POPUPS");
            for (int i = 0; i < PopupWindows.Length; i++)
            {
                PopupWindows[i].SetActive(false);
            }
        }


    }
}