using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontChangeListener : MonoBehaviour {

    private void Awake()
    {
        LocalizationManager.OnFontChanged += LocalizationManager_OnFontChanged;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnFontChanged -= LocalizationManager_OnFontChanged;
    }

    private void LocalizationManager_OnFontChanged(Font newFont)
    {
        if (newFont != null)
        {
            Text textComponent = GetComponent<Text>();
            if(textComponent != null)
                textComponent.font = newFont;
        }
    }
}
