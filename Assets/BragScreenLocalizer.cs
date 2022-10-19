using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.UI;

public class BragScreenLocalizer : MonoBehaviour {

    [SerializeField]
    Text tryBeatingMe, smartyPants;

    private void OnEnable()
    {
        tryBeatingMe.text = LocalizationManager.localization.TryBeatingMe;
        smartyPants.text = LocalizationManager.localization.SmartyPants;

        tryBeatingMe.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            tryBeatingMe.GetComponent<ArabicText>().enabled = true;
            tryBeatingMe.GetComponent<ArabicText>().Text = tryBeatingMe.text;
            tryBeatingMe.GetComponent<ArabicText>().Refresh();
        }

        smartyPants.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            smartyPants.GetComponent<ArabicText>().enabled = true;
            smartyPants.GetComponent<ArabicText>().Text = smartyPants.text;
            smartyPants.GetComponent<ArabicText>().Refresh();
        }
    }
}
