using System;
using UnityEngine;

// Coins, stars, daily reward, daily quests and unlocked styles. Everything lives in PlayerPrefs.
public static class GameEconomy
{
    public static event Action Changed;

    // ---- Coins -------------------------------------------------------------------------------
    public static int Coins => Mathf.Max(0, PlayerPrefs.GetInt("Coins", 0));
    public static void AddCoins(int amount)
    {
        PlayerPrefs.SetInt("Coins", Mathf.Max(0, Coins + amount));
        Save();
    }
    public static bool Spend(int amount)
    {
        if (amount < 0 || Coins < amount) return false;
        PlayerPrefs.SetInt("Coins", Coins - amount);
        Save();
        return true;
    }

    // ---- Stars (1-3 per level, best result kept) ---------------------------------------------
    public static int Stars(int level) => Mathf.Clamp(PlayerPrefs.GetInt("Stars_" + level, 0), 0, 3);
    public static int TotalStars()
    {
        int total = 0;
        for (int i = 1; i <= LevelProgression.TotalCount; i++) total += Stars(i);
        return total;
    }
    // Returns the coins earned for finishing the level and updates quests.
    public static int CompleteLevel(int level, int stars)
    {
        stars = Mathf.Clamp(stars, 1, 3);
        if (stars > Stars(level)) PlayerPrefs.SetInt("Stars_" + level, stars);
        int coins = 5 + stars * 5;
        PlayerPrefs.SetInt("Coins", Coins + coins);
        AddQuestProgress(Quest.Wins, 1);
        if (stars == 3) AddQuestProgress(Quest.ThreeStars, 1);
        Save();
        return coins;
    }

    // ---- Styles ------------------------------------------------------------------------------
    static readonly int[] StylePrices = { 0, 0, 300, 400, 400, 500, 500 };
    public static int StylePrice(int style) => StylePrices[Mathf.Clamp(style, 0, StylePrices.Length - 1)];
    public static bool StyleOwned(int style) => StylePrice(style) == 0 || PlayerPrefs.GetInt("StyleOwned_" + style, 0) == 1;
    public static bool BuyStyle(int style)
    {
        if (StyleOwned(style)) return true;
        if (!Spend(StylePrice(style))) return false;
        PlayerPrefs.SetInt("StyleOwned_" + style, 1);
        Save();
        return true;
    }

    // ---- Daily reward (7-day cycle) ----------------------------------------------------------
    public static readonly int[] DailyRewards = { 20, 30, 40, 50, 60, 80, 150 };
    static string Today => DateTime.Now.ToString("yyyyMMdd");
    static string Yesterday => DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
    public static bool DailyAvailable => PlayerPrefs.GetString("DailyLast", "") != Today;
    // Index (0-6) of the reward that the next claim gives.
    public static int DailyDay
    {
        get
        {
            string last = PlayerPrefs.GetString("DailyLast", "");
            int streak = PlayerPrefs.GetInt("DailyStreak", 0);
            if (last == Today) return Mathf.Clamp(streak - 1, 0, 6) ;
            if (last == Yesterday) return streak % 7;
            return 0;
        }
    }
    public static int ClaimDaily()
    {
        if (!DailyAvailable) return 0;
        int day = DailyDay;
        PlayerPrefs.SetInt("DailyStreak", day + 1);
        PlayerPrefs.SetString("DailyLast", Today);
        int reward = DailyRewards[day];
        PlayerPrefs.SetInt("Coins", Coins + reward);
        Save();
        return reward;
    }

    // ---- Daily quests ------------------------------------------------------------------------
    public enum Quest { Wins = 0, ThreeStars = 1, Shots = 2 }
    public static readonly string[] QuestNames = { "Thắng 3 màn", "Đạt 3 sao ở 1 màn", "Bắn 30 phát" };
    public static readonly int[] QuestTargets = { 3, 1, 30 };
    public static readonly int[] QuestRewards = { 30, 40, 20 };
    static void RollQuests()
    {
        if (PlayerPrefs.GetString("QuestDay", "") == Today) return;
        PlayerPrefs.SetString("QuestDay", Today);
        for (int i = 0; i < QuestTargets.Length; i++) { PlayerPrefs.SetInt("QuestProgress_" + i, 0); PlayerPrefs.SetInt("QuestClaimed_" + i, 0); }
    }
    public static int QuestProgress(int quest) { RollQuests(); return Mathf.Min(QuestTargets[quest], PlayerPrefs.GetInt("QuestProgress_" + quest, 0)); }
    public static bool QuestComplete(int quest) => QuestProgress(quest) >= QuestTargets[quest];
    public static bool QuestClaimed(int quest) { RollQuests(); return PlayerPrefs.GetInt("QuestClaimed_" + quest, 0) == 1; }
    public static void AddQuestProgress(Quest quest, int amount)
    {
        RollQuests();
        int i = (int)quest;
        PlayerPrefs.SetInt("QuestProgress_" + i, PlayerPrefs.GetInt("QuestProgress_" + i, 0) + amount);
        // Shots happen constantly; the next real write persists them.
        if (quest != Quest.Shots) Save();
    }
    public static bool ClaimQuest(int quest)
    {
        if (!QuestComplete(quest) || QuestClaimed(quest)) return false;
        PlayerPrefs.SetInt("QuestClaimed_" + quest, 1);
        PlayerPrefs.SetInt("Coins", Coins + QuestRewards[quest]);
        Save();
        return true;
    }
    public static bool AnyQuestReady()
    {
        for (int i = 0; i < QuestTargets.Length; i++) if (QuestComplete(i) && !QuestClaimed(i)) return true;
        return false;
    }

    public static void Reset()
    {
        foreach (var key in new[] { "Coins", "DailyLast", "DailyStreak", "QuestDay" }) PlayerPrefs.DeleteKey(key);
        for (int i = 1; i <= LevelProgression.TotalCount; i++) PlayerPrefs.DeleteKey("Stars_" + i);
        Save();
    }

    static void Save() { PlayerPrefs.Save(); Changed?.Invoke(); }
}
