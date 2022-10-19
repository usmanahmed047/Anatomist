using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Remoting.Contexts;

namespace Anatomist
{
    public class Screen_LanguageSelection : Screen
    {

        public GameObject coloredBG;

        public ToggleGroup toggleGroup;
        public Button continueButton;
        public Text continueButtonText;

        public Toggle[] languageButtonToggles;

        public Text[] translationDisclaimers;
        public ArabicText[] arabicTextObjects;
        string selectedLanguageCode;

        [SerializeField]
        string continue_english, continue_spanish, continue_arabic, continue_chinese, continue_russian, continue_italian, continue_french, continue_german, continue_portugues;

        public override void Open(params object[] args)
        {

            base.Open(args);

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => { LanguageSelected(); });

            if (LocalizationManager.localization != null)
            {
                foreach (Text t in translationDisclaimers)
                {
                    t.text = LocalizationManager.localization.TranslationDisclaimer;

                    t.GetComponent<ArabicText>().enabled = false;
                    if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
                    {
                        t.GetComponent<ArabicText>().enabled = true;
                        t.GetComponent<ArabicText>().Text = LocalizationManager.localization.TranslationDisclaimer;
                        t.GetComponent<ArabicText>().Refresh();
                    }
                }
            }



            if (args != null && args.Length > 0 && args[0].GetType() == typeof(bool) && (bool)args[0] == true)
            {
                //Show the white background
                coloredBG.SetActive(false);

            }
            else
            {
                coloredBG.SetActive(true);
            }

            for (int i = 0; i < languageButtonToggles.Length; i++)
            {
                if (languageButtonToggles[i].gameObject.name == LocalizationManager.CurrentLanguage.tag)
                {
                    languageButtonToggles[i].isOn = true;
                    toggleGroup.NotifyToggleOn(languageButtonToggles[i]);
                    Debug.Log("Toggled the button for the active language: " + LocalizationManager.CurrentLanguage.tag);
                    break;
                }
                
                
            }

        }

        public override void Close()
        {
            base.Close();
        }

        string selectedTag = "en";

        public void LanguageSelected()
        {
            Anatomist.Screen.Find("Home").Open();

            if (selectedTag == "ar")
            {
                for (int i = 0; i < arabicTextObjects.Length; i++)
                {
                    arabicTextObjects[i].enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < arabicTextObjects.Length; i++)
                {
                    arabicTextObjects[i].enabled = false;
                }
            }
        }

        public void OnToggleSelected(string tag)
        {
            LocalizationManager.ChangeLanguage(tag);
            Text textObj = continueButton.GetComponentInChildren<Text>();
            if (textObj != null)
            {
                LanguageData langData = LocalizationManager.Instance.Languages.FirstOrDefault(x => x.tag.ToUpper() == tag.ToUpper());
                if(langData != null)
                    textObj.font = langData.font == null ? LocalizationManager.Instance.defaultFont : langData.font;
            }


            selectedTag = tag;

            foreach (Text t in translationDisclaimers)
            {
                t.text = LocalizationManager.localization.TranslationDisclaimer;


                t.GetComponent<ArabicText>().enabled = false;
                if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
                {
                    t.GetComponent<ArabicText>().enabled = true;
                    t.GetComponent<ArabicText>().Text = LocalizationManager.localization.TranslationDisclaimer;
                    t.GetComponent<ArabicText>().Refresh();
                }
            }

            switch (tag)
            {
                case "en":
                    continueButtonText.text = continue_english;
                    break;
                case "es":
                    continueButtonText.text = continue_spanish;
                    break;
                case "ru":
                    continueButtonText.text = continue_russian;
                    break;
                case "it":
                    continueButtonText.text = continue_italian;
                    break;
                case "zh-Hans":
                    continueButtonText.text = continue_chinese;
                    break;
                case "ar":
                    continueButtonText.text = continue_arabic;
                    break;
                case "fr":
                    continueButtonText.text = continue_french;
                    break;
                case "de":
                    continueButtonText.text = continue_german;
                    break;
                case "pt":
                    continueButtonText.text = continue_portugues;
                    break;
                default:
                    break;

            }
            continueButtonText.GetComponent<ArabicText>().enabled = false;
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                continueButtonText.GetComponent<ArabicText>().enabled = true;
                continueButtonText.GetComponent<ArabicText>().Text = continueButtonText.text;
                continueButtonText.GetComponent<ArabicText>().Refresh();

            }
            if (string.IsNullOrEmpty(continueButtonText.text))
            {
                continueButtonText.text = "Continue";
            }
        }

        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
    }
}
