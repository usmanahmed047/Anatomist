﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.UI;

public class Popup_PurchaseFullApp : PopupWindow
{
    public Text title, body, btnText, btnRestoreText;

    public VerticalLayoutGroup lg;

    private void Start()
    {
        Purchaser.PurchaseComplete += OnPurchaseComplete;
        Purchaser.FinishedRestoring += OnFinishedRestoring;
    }

    public void RestorePurchases()
    {
        Purchaser.Instance.RestorePurchases();
    }

    public override void Open()
    {
        title.text = LocalizationManager.localization.buyFullAppTitle;

        body.text = LocalizationManager.localization.buyFullAppBody.Replace("$GET_PRICE_FULLAPP", GameManager.GetTag("$GET_PRICE_FULLAPP"));
        btnText.text = LocalizationManager.localization.btn_BuyPremium;
        btnRestoreText.text = LocalizationManager.localization.RestorePurchases;


        title.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            title.GetComponent<ArabicText>().enabled = true;
            title.GetComponent<ArabicText>().Text = title.text;
            title.GetComponent<ArabicText>().Refresh();
        }

        body.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            body.GetComponent<ArabicText>().enabled = true;
            body.GetComponent<ArabicText>().Text = body.text;
            body.GetComponent<ArabicText>().Refresh();
        }
        btnRestoreText.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            btnRestoreText.GetComponent<ArabicText>().enabled = true;
            btnRestoreText.GetComponent<ArabicText>().Text = btnRestoreText.text;
            btnRestoreText.GetComponent<ArabicText>().Refresh();
        }

        btnText.GetComponent<ArabicText>().enabled = false;
        if (PlayerPrefs.GetString("LanguageTag", "en") == "ar")
        {
            btnText.GetComponent<ArabicText>().enabled = true;
            btnText.GetComponent<ArabicText>().Text = btnText.text;
            btnText.GetComponent<ArabicText>().Refresh();
        }

        base.Open();
        lg.enabled = false;
        lg.enabled = true;
    }

    public override void Close()
    {
        base.Close();
    }

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(lg.GetComponent<RectTransform>());

    }

    public void OnFinishedRestoring()
    {
        Close();
    }

    public void OnPurchaseComplete(bool success)
    {
        Close();
    }
}
