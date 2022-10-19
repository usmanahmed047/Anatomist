using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anatomist;

namespace Anatomist
{
    public class DifficultySelectionBtn : MonoBehaviour
    {
        Button btn;
        public Text NameText;
        public GameObject Lock;
        public bool IsLocked;
        GameManager.QuizDifficulty difficulty;
        // Use this for initialization
        void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(StartGame);
        }

        // Update is called once per frame
        public void UpdateDifficulty(bool IsLocked, GameManager.QuizDifficulty difficulty)
        {
            this.IsLocked = IsLocked;
            this.difficulty = difficulty;
            Lock.SetActive(IsLocked);
            NameText.text = difficulty.name;

            NameText.GetComponent<ArabicText>().enabled = false;
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                NameText.GetComponent<ArabicText>().enabled = true;
                NameText.GetComponent<ArabicText>().Text = NameText.text;
                NameText.GetComponent<ArabicText>().Refresh();
            }
        }
        
        public void StartGame()
        {
            if (!IsLocked)
                Popup_QuizSelection.Instance.StartGame(difficulty);
            else
                ScreenManager.Instance.OpenPopupWindow("Popup - PurchaseFullApp", "", closeOthers: false);
        }
    }
}
