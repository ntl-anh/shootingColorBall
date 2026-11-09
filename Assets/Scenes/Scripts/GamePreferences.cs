using System;
using UnityEngine;

public static class GamePreferences
{
    public static event Action Changed;
    // 0 = Jelly Pop (pastel art), 1.. = procedural skins (see CalmGameUI.StyleNames). Applied when a scene loads.
    public const int StyleCount = 7;
    public static int Style => Mathf.Clamp(PlayerPrefs.GetInt("UiStyle", 0), 0, StyleCount - 1);
    // True when the current scene was built with a procedural skin, so UI and effects never mix styles.
    public static bool SkinActive { get; set; }
    public static void SetStyle(int value) { PlayerPrefs.SetInt("UiStyle", Mathf.Clamp(value, 0, StyleCount - 1)); Save(false); }
    public static int Theme => Mathf.Clamp(PlayerPrefs.GetInt("Theme", 0), 0, JellyPopAssets.ThemeCount - 1);
    public static bool Haptics => PlayerPrefs.GetInt("Haptics", 1) == 1;
    public static float Strength => Mathf.Clamp01(PlayerPrefs.GetFloat("HapticStrength", .45f));
    public static bool Effects => PlayerPrefs.GetInt("Effects", 1) == 1;
    public static bool Sound => PlayerPrefs.GetInt("SoundOn", 1) == 1;
    public static void SetTheme(int value) { PlayerPrefs.SetInt("Theme", Mathf.Clamp(value, 0, JellyPopAssets.ThemeCount - 1)); Save(); }
    public static void SetHaptics(bool value)
    {
        PlayerPrefs.SetInt("Haptics", value ? 1 : 0);
        if (!value) GameHaptics.Stop();
        Save(false);
    }
    public static void SetStrength(float value)
    {
        PlayerPrefs.SetFloat("HapticStrength", Mathf.Clamp01(value));
        if (Strength <= 0) GameHaptics.Stop();
        Save(false);
    }
    public static void SetEffects(bool value) { PlayerPrefs.SetInt("Effects", value ? 1 : 0); Save(); }
    public static void SetSound(bool value)
    {
        PlayerPrefs.SetInt("SoundOn", value ? 1 : 0);
        if (soundmanager.instance && soundmanager.instance._As) soundmanager.instance._As.mute = !value;
        Save(false);
    }
    static void Save(bool notify = true) { PlayerPrefs.Save(); if (notify) Changed?.Invoke(); }
}

public static class GameHaptics
{
    static float lastPulse = -1;
    public static bool Enabled => GamePreferences.Haptics && GamePreferences.Strength > 0;
    public static void Stop()
    {
        lastPulse = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                if (vibrator != null) vibrator.Call("cancel");
        }
        catch (Exception ex) { Debug.LogWarning("Haptics stop unavailable: " + ex.Message); }
#endif
    }
    public static void Pulse(float weight = 1f)
    {
        if (!Enabled || weight <= 0 || Time.unscaledTime - lastPulse < .07f) return;
        lastPulse = Time.unscaledTime;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                if (vibrator == null || !vibrator.Call<bool>("hasVibrator")) return;
                float strength = Mathf.Clamp01(GamePreferences.Strength * weight);
                long duration = (long)Mathf.Lerp(12, 65, strength);
                if (version.GetStatic<int>("SDK_INT") >= 26)
                {
                    using (var effectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                    using (var effect = effectClass.CallStatic<AndroidJavaObject>("createOneShot", duration, Mathf.Clamp(Mathf.RoundToInt(strength * 255), 1, 255)))
                        vibrator.Call("vibrate", effect);
                }
                else vibrator.Call("vibrate", duration);
            }
        }
        catch (Exception ex) { Debug.LogWarning("Haptics unavailable: " + ex.Message); }
#elif UNITY_IOS && !UNITY_EDITOR
        // The built-in iOS API has no amplitude control; use the system pulse.
        Handheld.Vibrate();
#endif
    }
}
