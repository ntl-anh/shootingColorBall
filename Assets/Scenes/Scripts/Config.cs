using UnityEngine;

public class Config
{
    // LevelPlay (Unity) app key for the Android app "Shooting Color"
    public static string AndroidAppKey = "2965241dd";

    // Ad Unit IDs from LevelPlay dashboard -> Setup -> Ad units. Leave empty to disable that format.
    public static string AndroidBannerAdUnitId = "9ey45yqa17kgva9h";
    public static string AndroidRewardedAdUnitId = "sftexku2kgqwokjw";
    public static string AndroidInterstitialAdUnitId = "vqqhzk5dyka09azo";

    public const int InitialUndoCount = 3;
    public const int UndoRewardAmount = 3;
}
