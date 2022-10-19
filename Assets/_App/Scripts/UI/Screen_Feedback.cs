using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Anatomist
{
    public class Screen_Feedback : Screen
    {
        [SerializeField]
        Text feedbackTitle;
        [SerializeField]
        Text feedbackBody1;
        [SerializeField]
        Text feedbackPositive;
        [SerializeField]
        Text feedbackBody2;
        [SerializeField]
        Text feedbackNegative;
        [SerializeField]
        Text feedbackBody3;
        [SerializeField]
        Text version;

        private void Start()
        {
            Localize();
            LocalizationManager.OnLanguageChanged += Localize;
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
        }

        public override void Close()
        {
            base.Close();
        }

        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
        void Localize()
        {
            feedbackTitle.GetComponent<ArabicText>().enabled = false;
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackTitle.GetComponent<ArabicText>().enabled = true;
                feedbackTitle.text = LocalizationManager.localization.Feedback;
                feedbackTitle.GetComponent<ArabicText>().Text = feedbackTitle.text;
                feedbackTitle.GetComponent<ArabicText>().Refresh();
            }
            feedbackBody1.GetComponent<ArabicText>().enabled = false;
            feedbackBody1.text = LocalizationManager.localization.feedbackBody1.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackBody1.GetComponent<ArabicText>().enabled = true;
                feedbackBody1.GetComponent<ArabicText>().Text = feedbackBody1.text;
                feedbackBody1.GetComponent<ArabicText>().Refresh();
            }
#if UNITY_ANDROID
            feedbackPositive.GetComponent<ArabicText>().enabled = false;
            feedbackPositive.text = LocalizationManager.localization.feedbackPositive_android.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackPositive.GetComponent<ArabicText>().enabled = true;
                feedbackPositive.GetComponent<ArabicText>().Text = feedbackPositive.text;
                feedbackPositive.GetComponent<ArabicText>().Refresh();
            }
#elif UNITY_IOS
            feedbackPositive.text = LocalizationManager.localization.feedbackPositive_ios.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
             feedbackPositive.GetComponent<ArabicText>().Text = feedbackPositive.text;
            feedbackPositive.GetComponent<ArabicText>().Refresh();
#endif
            feedbackBody2.text = LocalizationManager.localization.feedbackBody2.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
            feedbackNegative.text = LocalizationManager.localization.feedbackNegative.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
            feedbackBody3.text = LocalizationManager.localization.feedbackBody3.Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));
            version.text = LocalizationManager.localization.version + "\n" + LocalizationManager.localization.copyright;





            feedbackBody2.GetComponent<ArabicText>().enabled = false;

            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackBody2.GetComponent<ArabicText>().enabled = true;
                feedbackBody2.GetComponent<ArabicText>().Text = feedbackBody2.text;
                feedbackBody2.GetComponent<ArabicText>().Refresh();
            }

            feedbackNegative.GetComponent<ArabicText>().enabled = false;

            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackNegative.GetComponent<ArabicText>().enabled = true;
                feedbackNegative.GetComponent<ArabicText>().Text = feedbackNegative.text;
                feedbackNegative.GetComponent<ArabicText>().Refresh();
            }

            feedbackBody3.GetComponent<ArabicText>().enabled = false;

            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                feedbackBody3.GetComponent<ArabicText>().enabled = true;
                feedbackBody3.GetComponent<ArabicText>().Text = feedbackBody3.text;
                feedbackBody3.GetComponent<ArabicText>().Refresh();
            }

            version.GetComponent<ArabicText>().enabled = false;
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                version.GetComponent<ArabicText>().enabled = true;
                version.GetComponent<ArabicText>().Text = version.text;
                version.GetComponent<ArabicText>().Refresh();
            }
        }
    }
}
