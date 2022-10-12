using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public InterstitialAd Interstitial;

#if UNITY_ANDROID || UNITY_EDITOR
    const string appId = "ca-app-pub-6155656656665833~8100225300";
    const string adUnitID = "ca-app-pub-6155656656665833/2053691702";
#elif UNITY_IOS
    const string appId = "ca-app-pub-6155656656665833~2801216107";
    const string adUnitID = "ca-app-pub-6155656656665833/8729998500";
#endif

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        MobileAds.Initialize(appId);
        PrepareInterstitialAd();
    }



    public void PrepareInterstitialAd()
    {

        Debug.Log("Preparing Interstitial Ad");
        Interstitial = new InterstitialAd(adUnitID);

        Debug.Log("Interstitial = " + Interstitial.ToString());

        AdRequest request = new AdRequest.Builder().Build();
        Interstitial.LoadAd(request);
    }

    
    private static System.Action OnAdWasClosed = null;

    public void ShowInterstitialAD(System.Action OnAdClosed = null)
    {
        OnAdWasClosed = OnAdClosed;

        if (Purchaser.Instance.GetPurchasedState(Purchaser.PRODUCT_FULL_APP))
        {
            Debug.Log("Not showing ad because full app is unlocked.");

            //Go ahead and fire off the OnAdClosed event if it was not null incase there is code that waits for the ad to close before proceeding.
            if (OnAdWasClosed != null)
            {
                OnAdWasClosed.Invoke();
                OnAdWasClosed = null;
            }
            return;
        }

        Debug.Log("Is interstitial Loaded? " + Interstitial.IsLoaded().ToString());
        Debug.Log("<color=Cyan>Is interstitial Loaded? " + Interstitial.IsLoaded().ToString() + "</color>");


        //We never have to remove the listeners we define below because Interstital gets re-initialized with a new Interstitial object each time we call Prepare.

        // Called when an ad request has successfully loaded.
        Interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        Interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        Interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        Interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        Interstitial.OnAdLeavingApplication += HandleOnAdLeftApplication;


        if (Interstitial.IsLoaded())
        {
            
            Debug.Log("<color=Orange>Showing interstitial Ad now!!!</color>");
            Interstitial.Show();

#if UNITY_EDITOR
            OnAdWasClosed?.Invoke();
#endif
        }
        else
        {
            Debug.LogWarning("NO AD LOADED");
            OnAdWasClosed?.Invoke();
        }

        PrepareInterstitialAd();
    }


    #region AdEventHandlers

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: " + args.Message);

        if (OnAdWasClosed != null)
        {
            OnAdWasClosed();
        }
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
        if (OnAdWasClosed != null)
        {
            OnAdWasClosed();
        }
    }

    public void HandleOnAdLeftApplication(object sender, EventArgs args)
    {
        print("HandleAdLeftApplication event received");
    }

    #endregion

}