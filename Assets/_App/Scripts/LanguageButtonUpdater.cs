using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonUpdater : MonoBehaviour {

    [SerializeField]
    Image icon;

    private void Start()
    {
        icon = GetComponent<Image>();
    }

    private void OnEnable()
    {
        icon.sprite = LocalizationManager.CurrentLanguage.sprite;
    }
}
