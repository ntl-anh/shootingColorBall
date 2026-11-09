using UnityEngine;

public class Config
{
    // LevelPlay (Unity) app key for the Android app "Shooting Color"
    public static string AndroidAppKey = "290a18c4d";

    // Ad Unit IDs from LevelPlay dashboard -> Setup -> Ad units. Leave empty to disable that format.
    public static string AndroidBannerAdUnitId = "ibebs6y50k1lyvz5";
    public static string AndroidRewardedAdUnitId = "9as9qcfcclg2y3eb";
    public static string AndroidInterstitialAdUnitId = "ypfl6bko5zyw0m20"; 

    public const int InitialUndoCount = 3;
    public const int UndoRewardAmount = 3;
}
