
using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class AdRequest : MonoBehaviour
{
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
  /// Creates a 320x50 banner view at top of the screen.
  /// </summary>
  public void CreateBannerView()
  {
      Debug.LogWarning("Creating banner view");

      // If we already have a banner, destroy the old one.
      if (_bannerView != null)
      {
          DestroyAd();
      }

      // Create a 320x50 banner at top of the screen
      _bannerView = new BannerView(_adUnitId, AdSize.Banner, 0, 50);
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
      if(_bannerView == null)
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