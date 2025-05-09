using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class AdRequest : MonoBehaviour
{
    public static AdRequest Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string _adUnitId = "";
#else
    private string _adUnitId = "unused";
#endif

    BannerView _bannerView;

    public void Start()
    {
        AdManager.Instance.adRequest = this;
    }

    /// <summary>
    /// Creates a banner view at the bottom center of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.LogWarning("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a banner at the bottom center of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

        // 또는 직접 좌표를 지정하는 경우:
        // int x = (Screen.width - 320) / 2;  // 320은 배너의 기본 너비
        // int y = 0;  // 하단에 위치
        // _bannerView = new BannerView(_adUnitId, AdSize.Banner, x, y);
    }

    /// <summary>
    /// Destroys the current banner ad if it exists.
    /// </summary>
    private void DestroyAd()
    {
        Debug.LogWarning("Destroying banner view");
        _bannerView.Destroy();
        _bannerView = null;
    }
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new GoogleMobileAds.Api.AdRequest();
        // send the request to load the ad.
        Debug.LogWarning("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
}