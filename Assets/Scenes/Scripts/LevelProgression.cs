using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelProgression
{
    public const int OriginalCount = 50;
    public const int TotalCount = 100;
    public static int Current => int.TryParse(SceneManager.GetActiveScene().name, out int level) ? level : 0;
    public static int Saved => Mathf.Clamp(PlayerPrefs.GetInt("level", 1), 1, TotalCount);
    public static int Rounds => Current <= 50 ? 1 : Current <= 75 ? 2 : 3;
    public static void Continue() { CalmGameUI.Load(Saved.ToString()); }
    public static void Next()
    {
        int next = Current + 1;
        if (next > TotalCount) { CalmGameUI.Load("MainMenu"); return; }
        PlayerPrefs.SetInt("level", Mathf.Max(Saved, next));
        PlayerPrefs.Save();
        CalmGameUI.Load(next.ToString());
    }
    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("MaxUndoCount", Config.InitialUndoCount);
        PlayerPrefs.DeleteKey("UndoStackSize");
        for (int i = 1; i <= TotalCount; i++) PlayerPrefs.DeleteKey("Best_" + i);
        PlayerPrefs.Save();
        GameEconomy.Reset();
    }
}
