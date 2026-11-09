using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

// Reflection keeps tests separate from the project's predefined Assembly-CSharp.
public class UpgradeTests
{
    static Type Runtime(string name) => AppDomain.CurrentDomain.GetAssemblies()
        .Select(a => a.GetType(name)).First(t => t != null);
    static object Get(string type, string property) => Runtime(type).GetProperty(property).GetValue(null);
    static void Call(string type, string method, params object[] args) => Runtime(type).GetMethod(method).Invoke(null, args);

    [Test]
    public void BuildContainsEveryLevelInSequence()
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToArray();
        Assert.AreEqual(101, scenes.Length);
        Assert.AreEqual("Assets/Scenes/Levels/MainMenu.unity", scenes[0].path);
        for (int i = 1; i <= 100; i++) Assert.AreEqual("Assets/Scenes/Levels/" + i + ".unity", scenes[i].path);
    }
    [Test]
    public void SavedLevelClampsCorruptAndLegacyProgress()
    {
        bool existed = PlayerPrefs.HasKey("level"); int before = PlayerPrefs.GetInt("level");
        try
        {
            PlayerPrefs.SetInt("level", -100); Assert.AreEqual(1, Get("LevelProgression", "Saved"));
            PlayerPrefs.SetInt("level", 999); Assert.AreEqual(100, Get("LevelProgression", "Saved"));
            PlayerPrefs.SetInt("level", 73); Assert.AreEqual(73, Get("LevelProgression", "Saved"));
        }
        finally { if (existed) PlayerPrefs.SetInt("level", before); else PlayerPrefs.DeleteKey("level"); }
    }
    [Test]
    public void HapticStrengthClampsAndPersistsIndependentlyOfEnableSwitch()
    {
        bool hadStrength = PlayerPrefs.HasKey("HapticStrength"), hadToggle = PlayerPrefs.HasKey("Haptics");
        float strength = PlayerPrefs.GetFloat("HapticStrength"); int toggle = PlayerPrefs.GetInt("Haptics");
        try
        {
            Call("GamePreferences", "SetStrength", -1f); Assert.AreEqual(0f, Get("GamePreferences", "Strength"));
            Call("GamePreferences", "SetStrength", 2f); Assert.AreEqual(1f, Get("GamePreferences", "Strength"));
            Call("GamePreferences", "SetStrength", .6f); Call("GamePreferences", "SetHaptics", false);
            Assert.AreEqual(.6f, PlayerPrefs.GetFloat("HapticStrength"), .001f);
            Assert.AreEqual(false, Get("GamePreferences", "Haptics"));
        }
        finally
        {
            if (hadStrength) PlayerPrefs.SetFloat("HapticStrength", strength); else PlayerPrefs.DeleteKey("HapticStrength");
            if (hadToggle) PlayerPrefs.SetInt("Haptics", toggle); else PlayerPrefs.DeleteKey("Haptics");
            PlayerPrefs.Save();
        }
    }
    [Test]
    public void GlobalVibrationSwitchBlocksUiAndGameplayPulsesAndRetainsStrength()
    {
        bool hadToggle = PlayerPrefs.HasKey("Haptics"), hadStrength = PlayerPrefs.HasKey("HapticStrength");
        int toggle = PlayerPrefs.GetInt("Haptics"); float strength = PlayerPrefs.GetFloat("HapticStrength");
        var lastPulse = Runtime("GameHaptics").GetField("lastPulse", BindingFlags.Static | BindingFlags.NonPublic);
        float previousPulse = (float)lastPulse.GetValue(null);
        try
        {
            Call("GamePreferences", "SetStrength", .6f);
            Call("GamePreferences", "SetHaptics", false);
            Assert.AreEqual(0, PlayerPrefs.GetInt("Haptics"));
            Assert.AreEqual(false, Get("GameHaptics", "Enabled"));
            foreach (float weight in new[] { .3f, .35f, .75f, 1f }) Call("GameHaptics", "Pulse", weight);
            Assert.AreEqual(-1f, lastPulse.GetValue(null), "Disabled pulses must not reach the vibration backend or consume the cooldown");
            Call("GamePreferences", "SetHaptics", true);
            Assert.AreEqual(true, Get("GameHaptics", "Enabled"));
            Assert.AreEqual(.6f, Get("GamePreferences", "Strength"));
            Call("GamePreferences", "SetStrength", 0f);
            Assert.AreEqual(false, Get("GameHaptics", "Enabled"));
            Assert.AreEqual(true, Get("GamePreferences", "Haptics"), "Zero strength must not erase the on/off preference");
        }
        finally
        {
            if (hadToggle) PlayerPrefs.SetInt("Haptics", toggle); else PlayerPrefs.DeleteKey("Haptics");
            if (hadStrength) PlayerPrefs.SetFloat("HapticStrength", strength); else PlayerPrefs.DeleteKey("HapticStrength");
            lastPulse.SetValue(null, previousPulse);
            PlayerPrefs.Save();
        }
    }
    [Test]
    public void ThemeIndexCannotEscapeAvailablePalettes()
    {
        bool existed = PlayerPrefs.HasKey("Theme"); int before = PlayerPrefs.GetInt("Theme");
        try
        {
            Call("GamePreferences", "SetTheme", -5); Assert.AreEqual(0, Get("GamePreferences", "Theme"));
            Call("GamePreferences", "SetTheme", 99); Assert.AreEqual((int)Get("JellyPopAssets", "ThemeCount") - 1, Get("GamePreferences", "Theme"));
        }
        finally { if (existed) PlayerPrefs.SetInt("Theme", before); else PlayerPrefs.DeleteKey("Theme"); PlayerPrefs.Save(); }
    }
}
