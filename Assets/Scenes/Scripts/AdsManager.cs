using UnityEngine;
using Unity.Services.LevelPlay;

public class AdsManager : MonoBehaviour
{
    public BannerAdExample bannerAdExample;
    public RewardedAdsButton rewardedAds;
    public static AdsManager Ins;

    LevelPlayInterstitialAd _interstitial;
    int _levelsSinceAd;
    int _levelsUntilAd;
    bool _initialized;
    bool _bannerRequested;

    private void Awake()
    {
        if (Ins && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        _levelsUntilAd = Random.Range(2, 4);
        DontDestroyOnLoad(gameObject);
        InitializeAds();
    }

    public void InitializeAds()
    {
        if (string.IsNullOrEmpty(Config.AndroidAppKey))
        {
            Debug.LogWarning("LevelPlay app key is empty. Ads are disabled.");
            return;
        }
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        LevelPlay.Init(Config.AndroidAppKey);
    }

    void OnInitSuccess(LevelPlayConfiguration configuration)
    {
        Debug.Log("LevelPlay initialization complete.");
        _initialized = true;
        if (rewardedAds != null)
            rewardedAds.LoadAd();
        LoadInterstitial();
        if (_bannerRequested)
            showBanner();
    }

    void OnInitFailed(LevelPlayInitError error)
    {
        Debug.Log($"LevelPlay initialization failed: {error.ErrorCode} - {error.ErrorMessage}");
    }

    public void showBanner()
    {
        _bannerRequested = true;
        if (_initialized && bannerAdExample != null)
            bannerAdExample.ShowBannerAd();
    }

    // Returns false when no ad could be shown so the caller can tell the player.
    public bool showRewarded()
    {
        return _initialized && rewardedAds != null && rewardedAds.ShowAd();
    }

    void LoadInterstitial()
    {
        string adUnitId = Config.AndroidInterstitialAdUnitId;
        if (string.IsNullOrEmpty(adUnitId)) return;
        if (_interstitial == null)
        {
            _interstitial = new LevelPlayInterstitialAd(adUnitId);
            _interstitial.OnAdClosed += info => _interstitial.LoadAd();
            _interstitial.OnAdLoadFailed += error =>
            {
                Debug.Log($"Interstitial load failed: {error.ErrorCode} - {error.ErrorMessage}");
                Invoke(nameof(LoadInterstitial), 20f);
            };
            _interstitial.OnAdDisplayFailed += (info, error) => Debug.Log($"Interstitial show failed: {error.ErrorCode} - {error.ErrorMessage}");
        }
        _interstitial.LoadAd();
    }

    // Call once each time a level ends (win or lose). Shows an ad every 2-3 finished levels.
    public void OnLevelEnded()
    {
        _levelsSinceAd++;
        if (_levelsSinceAd < _levelsUntilAd) return;
        if (!_initialized || _interstitial == null || !_interstitial.IsAdReady()) return;
        _levelsSinceAd = 0;
        _levelsUntilAd = Random.Range(2, 4);
        _interstitial.ShowAd();
    }

    void OnDestroy()
    {
        if (Ins != this)
            return;
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
        if (_interstitial != null)
            _interstitial.DestroyAd();
    }
}
