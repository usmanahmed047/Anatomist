using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Anatomist
{
    public class Screen_About : Screen
    {
        [SerializeField]
        Text version;//, body;


        [SerializeField]
        Text aboutTitle;

        [SerializeField]
        Text body;

        [SerializeField]
        Transform bodyContainer;

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
            aboutTitle.text = LocalizationManager.localization.About.ToUpperInvariant();
            version.text = LocalizationManager.localization.version + "\n" + LocalizationManager.localization.copyright;


            aboutTitle.GetComponent<ArabicText>().Text = aboutTitle.text;
            aboutTitle.GetComponent<ArabicText>().Refresh();

            version.GetComponent<ArabicText>().Text = version.text;
            version.GetComponent<ArabicText>().Refresh();

            foreach (Transform child in bodyContainer)
            {
                if (child != body.transform)
                    Destroy(child.gameObject);
            }


            string[] segments = LocalizationManager.localization.aboutBody.Split(new string[] {"\n\n"}, StringSplitOptions.RemoveEmptyEntries);

            Color transparent = new Color(0, 0, 0, 0);

            for (int i = 0; i < segments.Length; i++)
            {
                GameObject bodyClone = Instantiate(body.gameObject);
                bodyClone.transform.SetParent(bodyContainer);
                bodyClone.transform.localScale = Vector3.one;

                Text t = bodyClone.GetComponent<Text>();
                t.text = segments[i].Replace("$LINKCOLOR", GameManager.GetTag("$LINKCOLOR"));

                if (PlayerPrefs.GetString("LanguageTag") == "ar")
                {
                    t.gameObject.AddComponent<ArabicText>().Text = t.text;
                    t.GetComponent<ArabicText>().Refresh();
                }
                bodyClone.SetActive(true);
                t.enabled = true;

            }

            body.gameObject.SetActive(false);
        }
        
    }

    
}