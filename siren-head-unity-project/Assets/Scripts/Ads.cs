// #define TEST_ADS

using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class Ads : MonoBehaviour
{
    private BannerView _bannerView;
    private InterstitialAd _interstitial;

    private static Ads _instance;

    public void Awake()
    {
        _instance = this;
        
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
        LoadAds();
    }

    public static void LoadAds()
    {
        _instance.RequestInterstitial();
        _instance.RequestBanner();
    }

    private void RequestInterstitial()
    {
#if TEST_ADS
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-4220591610119162/5936884745";
#elif UNITY_IPHONE
        string adUnitId = "unexpected_platform";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        _interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        _interstitial.LoadAd(request);
    }

    private void RequestBanner()
    {
#if TEST_ADS
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-4220591610119162/6309555305";
#elif UNITY_IPHONE
            string adUnitId = "unexpected_platform";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        _bannerView.LoadAd(request);
    }

    public static void ShowInterstitial(Action<bool> onAdClosed)
    {
        _instance._interstitial.OnAdFailedToLoad += (sender, args) => onAdClosed(false);
        if (_instance._interstitial.IsLoaded())
        {
            _instance._interstitial.OnAdClosed += (sender, args) => onAdClosed(true);
            _instance._interstitial.Show();
        }
#if TEST_ADS && UNITY_EDITOR
        onAdClosed(true);
#endif
    }

    public static void ShowBanner()
    {
        _instance._bannerView.Show();
    }

    public static void HideBanner()
    {
        _instance._bannerView.Hide();
    }

    public static bool IsLoaded()
    {
        return _instance._interstitial.IsLoaded();
    }
}