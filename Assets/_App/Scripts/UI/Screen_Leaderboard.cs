using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Anatomist
{
    public class Screen_Leaderboard : Screen
    {
        [SerializeField]
        Text title, yourScore;
        [SerializeField]
        Text userName, userScore;


        public override void Open(params object[] args)
        {
            base.Open(args);

            Localize();
        }

        void Localize()
        {
            title.GetComponent<ArabicText>().enabled = false;
            title.text = LocalizationManager.localization.Leaderboard.ToUpperInvariant();
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                title.GetComponent<ArabicText>().enabled = true;
                title.GetComponent<ArabicText>().Text = title.text;
                title.GetComponent<ArabicText>().Refresh();

            }

            yourScore.GetComponent<ArabicText>().enabled = false;
            yourScore.text = LocalizationManager.localization.YourScore.ToUpperInvariant();
            if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
            {
                yourScore.GetComponent<ArabicText>().enabled = true;
                yourScore.GetComponent<ArabicText>().Text = yourScore.text;
                yourScore.GetComponent<ArabicText>().Refresh();

            }
        }

        public override void Close()
        {
            base.Close();
        }
        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
        public void saveuserName(GameObject userNamedb)
        {
            
        }
        public void saveuserScore()
        {

        }
    }
   
}