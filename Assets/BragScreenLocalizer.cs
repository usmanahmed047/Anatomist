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

        tryBeatingMe.GetComponent<ArabicText>().Text = tryBeatingMe.text;
        tryBeatingMe.GetComponent<ArabicText>().Refresh();

        smartyPants.GetComponent<ArabicText>().Text = smartyPants.text;
        smartyPants.GetComponent<ArabicText>().Refresh();
    }
}
