using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Anatomist
{
    public class Screen_Home : Screen
    {

        //[SerializeField]
        //Popup_StartQuiz startQuizPopup;


        

        private void Start()
        {
            CategoryButton.CategorySelected += CategoryButton_CategorySelected;
        }

        
        private void CategoryButton_CategorySelected(string category, Sprite icon)
        {
            Debug.Log("Category selected");

            //if (!Purchaser.Instance.GetPurchasedState(Purchaser.PRODUCT_FULL_APP) && (category.ToUpper().Contains("ECG") || category.ToUpper().Contains("MEDICINE")))
            //{

            //    Debug.Log("App not purchased and is bonus section");
            //    ScreenManager.Instance.OpenPopupWindow("Popup - PurchaseFullApp", category, closeOthers: false);
            //}
            //else
            {
                Debug.Log("not bonus or app is purchased");
                ScreenManager.Instance.OpenPopupWindow("Popup - StartQuiz", category, icon: icon);
            }
        }

        public static bool showAdOnOpen = false;

        public GameObject whoButton;

        public Text bonusECG, bonusMedicine;

        private void Update()
        {
            
            if (LocalizationManager.CurrentLanguage != null)
            {
                whoButton.SetActive(LocalizationManager.CurrentLanguage.tag == "en");

                bonusMedicine.text = bonusECG.text = LocalizationManager.localization.bonusSection;
                bonusMedicine.GetComponent<ArabicText>().Text = bonusMedicine.text;
               // bonusMedicine.GetComponent<ArabicText>().Refresh();
            }
        }

        public override void Open(params object[] args)
        {
            //The home screen doesn't require any args, so we won't try to process them at all
            

            base.Open(args);

            if(showAdOnOpen) AdsManager.Instance.ShowInterstitialAD();

            MusicManager.Instance.PlayMusic("menu_music");
        }

        public override void Close()
        {
            base.Close();
        }

        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
    }
}