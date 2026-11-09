using UnityEngine;
using Unity.Services.LevelPlay;

public class BannerAdExample : MonoBehaviour
{
    LevelPlayBannerAd _bannerAd;

    public void LoadBanner()
    {
        if (_bannerAd != null)
            return;
        string adUnitId = Config.AndroidBannerAdUnitId;
        if (string.IsNullOrEmpty(adUnitId))
        {
            Debug.LogWarning("Banner ad unit id is empty.");
            return;
        }
        _bannerAd = new LevelPlayBannerAd(adUnitId);
        _bannerAd.OnAdLoadFailed += error => Debug.Log($"Banner load failed: {error.ErrorCode} - {error.ErrorMessage}");
        _bannerAd.LoadAd();
    }

    public void ShowBannerAd()
    {
        if (_bannerAd == null)
            LoadBanner();
        else
            _bannerAd.ShowAd();
    }

    public void HideBannerAd()
    {
        if (_bannerAd != null)
            _bannerAd.HideAd();
    }

    void OnDestroy()
    {
        if (_bannerAd != null)
            _bannerAd.DestroyAd();
    }
}
