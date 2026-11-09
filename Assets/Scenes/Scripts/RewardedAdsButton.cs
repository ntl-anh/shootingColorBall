using UnityEngine;
using Unity.Services.LevelPlay;

public class RewardedAdsButton : MonoBehaviour
{
    LevelPlayRewardedAd _rewardedAd;

    public void LoadAd()
    {
        string adUnitId = Config.AndroidRewardedAdUnitId;
        if (string.IsNullOrEmpty(adUnitId))
        {
            Debug.LogWarning("Rewarded ad unit id is empty.");
            return;
        }
        if (_rewardedAd == null)
        {
            _rewardedAd = new LevelPlayRewardedAd(adUnitId);
            _rewardedAd.OnAdRewarded += OnAdRewarded;
            _rewardedAd.OnAdClosed += info => _rewardedAd.LoadAd();
            _rewardedAd.OnAdLoadFailed += error =>
            {
                Debug.Log($"Rewarded load failed: {error.ErrorCode} - {error.ErrorMessage}");
                Invoke(nameof(LoadAd), 20f);
            };
            _rewardedAd.OnAdDisplayFailed += (info, error) => Debug.Log($"Rewarded show failed: {error.ErrorCode} - {error.ErrorMessage}");
        }
        _rewardedAd.LoadAd();
    }

    public bool ShowAd()
    {
        if (_rewardedAd != null && _rewardedAd.IsAdReady())
        {
            _rewardedAd.ShowAd();
            return true;
        }
        Debug.Log("Rewarded ad is not ready yet.");
        LoadAd();
        return false;
    }

    void OnAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
    {
        int count = PlayerPrefs.GetInt("MaxUndoCount", Config.InitialUndoCount) + Config.UndoRewardAmount;
        if (GameManager.instance) GameManager.instance.MaxUndoCount = count;
        PlayerPrefs.SetInt("MaxUndoCount", count);
        PlayerPrefs.Save();
        if (PanelTopBtns.instance) PanelTopBtns.instance.SetTextValue(count);
        if (CalmGameUI.Instance) CalmGameUI.Instance.ShowMessage("+" + Config.UndoRewardAmount + " lượt hoàn tác!");
    }

    void OnDestroy()
    {
        if (_rewardedAd != null)
            _rewardedAd.DestroyAd();
    }
}
