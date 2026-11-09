using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> _pattern = new List<GameObject>();
    public List<GameObject> _cubes = new List<GameObject>();
    public int matchCount;
    public bool once;
    public static GameManager instance;
    readonly Stack<Snapshot> undoStack = new Stack<Snapshot>();
    readonly List<CubeState> initialBoard = new List<CubeState>();
    readonly List<ShootBall> activeShots = new List<ShootBall>();
    int maxUndoCount = Config.InitialUndoCount;
    int combo;
    bool roundStarted;
    bool appPaused;
    public float TimeRemaining { get; private set; }
    public bool Timed => LevelProgression.Current > 50;
    float RoundSeconds => Mathf.Max(60, 120 - (LevelProgression.Current - 51) * 1.25f,
        FindObjectsByType<Shootcolor>(FindObjectsSortMode.None).Length * 9);
    public int Round { get; private set; } = 1;
    int cannonCount, shotsFired;
    // 3 stars for a clean solution; every shot beyond one per cannon and round (wasted by undo) costs a star.
    public int StarsEarned
    {
        get
        {
            int extra = shotsFired - cannonCount * LevelProgression.Rounds;
            return extra <= 0 ? 3 : extra <= 2 ? 2 : 1;
        }
    }
    public bool ShotInFlight => activeShots.Count > 0;
    public int MaxUndoCount { get => maxUndoCount; set => maxUndoCount = Mathf.Max(0, value); }

    void Awake() { instance = this; StartCoroutine(DisplayBannerWithDelay()); }
    IEnumerator DisplayBannerWithDelay()
    {
        yield return new WaitForSeconds(1);
        if (AdsManager.Ins) AdsManager.Ins.showBanner();
    }
    void Start()
    {
        var pattern = GameObject.Find("Pattern");
        var cubes = GameObject.Find("cubes");
        if (!pattern || !cubes || pattern.transform.childCount == 0) return;
        _pattern.Clear(); _cubes.Clear();
        foreach (Transform item in pattern.transform.GetChild(0)) _pattern.Add(item.gameObject);
        foreach (Transform item in cubes.transform) _cubes.Add(item.gameObject);
        LoadUndoData();
        cannonCount = FindObjectsByType<Shootcolor>(FindObjectsSortMode.None).Length;
        ChangeCubes();
        foreach (var cube in _cubes) initialBoard.Add(State(cube, ""));
        TimeRemaining = RoundSeconds;
        UpdateUndoUI();
    }
    void Update()
    {
        if (appPaused || !Timed || !roundStarted || once || CalmGameUI.InputBlocked) return;
        TimeRemaining = Mathf.Max(0, TimeRemaining - Time.deltaTime);
        if (TimeRemaining > 0) return;
        once = true;
        foreach (var shot in activeShots.ToArray()) if (shot) Destroy(shot.gameObject);
        if (CalmGameUI.Instance) CalmGameUI.Instance.ShowTimedOut();
    }
    void OnApplicationPause(bool paused) { appPaused = paused; }
    CubeState State(GameObject cube, string cannon) => new CubeState(cube.transform.position,
        cube.GetComponent<MeshRenderer>().material.color, cube.name, cube.transform.rotation, cannon);
    public void SaveState(Shootcolor cannon)
    {
        shotsFired++;
        GameEconomy.AddQuestProgress(GameEconomy.Quest.Shots, 1);
        // Shots fired while another is still flying belong to the same undo step,
        // otherwise the snapshot would capture a half-painted board.
        if (ShotInFlight && undoStack.Count > 0) { undoStack.Peek().cannons.Add(cannon); return; }
        var snapshot = new Snapshot();
        snapshot.cannons.Add(cannon);
        foreach (var cube in _cubes) snapshot.cubeStates.Add(State(cube, cannon ? cannon.name : ""));
        undoStack.Push(snapshot);
    }
    public void RegisterShot(ShootBall shot) { activeShots.Add(shot); roundStarted = true; }
    public void FinishShot(ShootBall shot)
    {
        if (!activeShots.Remove(shot) || activeShots.Count > 0) return;
        StartCoroutine(CheckAfterPhysics());
    }
    IEnumerator CheckAfterPhysics() { yield return null; CheckLevelComplete(); }
    public void RegisterHit(GameObject cube)
    {
        int index = _cubes.IndexOf(cube);
        if (index < 0 || index >= _pattern.Count) return;
        if (cube.name == _pattern[index].name)
        {
            combo++;
            if (combo == 1 || combo % 3 == 0) GameHaptics.Pulse(Mathf.Min(1, .35f + combo * .08f));
        }
        else combo = 0;
    }
    public void UndoLastAction()
    {
        if (once || ShotInFlight || CalmGameUI.InputBlocked) return;
        if (maxUndoCount <= 0)
        {
            if (!AdsManager.Ins || !AdsManager.Ins.showRewarded())
                if (CalmGameUI.Instance) CalmGameUI.Instance.ShowMessage("Quảng cáo chưa sẵn sàng, thử lại sau nhé.");
            return;
        }
        if (undoStack.Count == 0) return;
        var snapshot = undoStack.Pop();
        for (int i = 0; i < _cubes.Count; i++) Restore(_cubes[i], snapshot.cubeStates[i]);
        foreach (var cannon in snapshot.cannons) if (cannon) cannon.canShoot = true;
        maxUndoCount--; combo = 0;
        SaveUndoData(); UpdateUndoUI();
    }
    void Restore(GameObject cube, CubeState state)
    {
        cube.transform.position = state.position;
        cube.GetComponent<MeshRenderer>().material.color = state.color;
        cube.name = state.name;
        cube.transform.rotation = state.rotation;
    }
    void ChangeCubes()
    {
        if (_cubes.Count == 0 || !InGameUi.instance || InGameUi.instance.newModels.Length == 0) return;
        int model = (Mathf.Max(1, LevelProgression.Current) - 1) % InGameUi.instance.newModels.Length;
        foreach (var cube in _cubes)
        {
            cube.GetComponent<MeshFilter>().mesh = InGameUi.instance.newModels[model];
            cube.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
        _cubes[0].transform.parent.position += Vector3.forward * .7f;
    }
    public void CheckLevelComplete()
    {
        if (once || ShotInFlight || _cubes.Count == 0 || _pattern.Count != _cubes.Count) return;
        matchCount = 0;
        for (int i = 0; i < _cubes.Count; i++) if (_cubes[i].name == _pattern[i].name) matchCount++;
        if (matchCount != _cubes.Count)
        {
            bool remaining = false;
            foreach (var cannon in FindObjectsByType<Shootcolor>(FindObjectsSortMode.None)) remaining |= cannon.canShoot;
            if (!remaining && CalmGameUI.Instance) StartCoroutine(StuckAfterDelay());
            return;
        }
        if (Round < LevelProgression.Rounds) { once = true; StartCoroutine(NextRound()); return; }
        once = true;
        GameHaptics.Pulse(1);
        for (int i = 0; i < _cubes.Count; i += Mathf.Max(1, _cubes.Count / 6))
            GameFeel.Burst(_cubes[i].transform.position, _cubes[i].GetComponent<Renderer>().material.color, true);
        if (soundmanager.instance) soundmanager.instance.playmysound(soundmanager.instance.levelcomplete);
        PlayerPrefs.SetInt("Best_" + LevelProgression.Current, 1);
        if (LevelProgression.Current < LevelProgression.TotalCount)
            PlayerPrefs.SetInt("level", Mathf.Max(LevelProgression.Saved, LevelProgression.Current + 1));
        PlayerPrefs.Save();
        StartCoroutine(CompleteAfterCelebration());
    }
    IEnumerator StuckAfterDelay()
    {
        yield return new WaitForSeconds(1.2f);
        while (CalmGameUI.InputBlocked && !once) yield return null;
        if (once || ShotInFlight || !CalmGameUI.Instance) yield break;
        foreach (var cannon in FindObjectsByType<Shootcolor>(FindObjectsSortMode.None)) if (cannon.canShoot) yield break;
        CalmGameUI.Instance.ShowStuck();
        if (AdsManager.Ins) AdsManager.Ins.OnLevelEnded();
    }
    IEnumerator CompleteAfterCelebration()
    {
        yield return new WaitForSeconds(1.2f);
        if (InGameUi.instance) InGameUi.instance.LevelCompleted();
        if (AdsManager.Ins) AdsManager.Ins.OnLevelEnded();
    }
    IEnumerator NextRound()
    {
        foreach (var cube in _cubes) GameFeel.Burst(cube.transform.position, cube.GetComponent<Renderer>().material.color);
        yield return new WaitForSeconds(.8f);
        var cannons = FindObjectsByType<Shootcolor>(FindObjectsSortMode.None);
        Array.Sort(cannons, (a, b) => string.CompareOrdinal(a.name, b.name));
        var names = new List<string>(); var colors = new List<Color>();
        foreach (var cannon in cannons)
            if (!names.Contains(cannon.ColorName)) { names.Add(cannon.ColorName); colors.Add(cannon.shootcolor); }
        // A bijection preserves the existing solution, collision paths and color identity.
        foreach (var target in _pattern)
        {
            int index = names.IndexOf(target.name);
            if (index < 0) continue;
            int next = (index + 1) % names.Count;
            target.name = names[next]; target.GetComponent<Renderer>().material.color = colors[next];
        }
        foreach (var cannon in cannons)
        {
            int next = (names.IndexOf(cannon.ColorName) + 1) % names.Count;
            cannon.SetColor(names[next], colors[next]); cannon.canShoot = true;
        }
        for (int i = 0; i < _cubes.Count; i++)
        {
            var state = initialBoard[i];
            int index = names.IndexOf(state.name);
            if (index >= 0)
            {
                int next = (index + 1) % names.Count;
                state.name = names[next]; state.color = colors[next];
            }
            Restore(_cubes[i], state);
        }
        undoStack.Clear(); combo = 0; Round++; once = false;
        roundStarted = false; TimeRemaining = RoundSeconds;
        if (CalmGameUI.Instance) CalmGameUI.Instance.ShowRound(Round);
    }
    public void ResetLevel() { SaveUndoData(); CalmGameUI.Load(SceneManager.GetActiveScene().name); }
    public void SaveUndoData() { PlayerPrefs.SetInt("MaxUndoCount", maxUndoCount); PlayerPrefs.Save(); }
    public void LoadUndoData()
    {
        // Older builds left a huge test value (9999) in the saved data: restart from the real starting amount once.
        if (PlayerPrefs.GetInt("UndoRules", 0) < 1)
        {
            PlayerPrefs.SetInt("MaxUndoCount", Config.InitialUndoCount);
            PlayerPrefs.SetInt("UndoRules", 1);
            PlayerPrefs.Save();
        }
        maxUndoCount = Mathf.Max(0, PlayerPrefs.GetInt("MaxUndoCount", Config.InitialUndoCount));
        // Snapshots belong to the current board and must never be loaded into another scene.
        undoStack.Clear();
    }
    void UpdateUndoUI() { if (PanelTopBtns.instance) PanelTopBtns.instance.SetTextValue(maxUndoCount); }
    void OnDestroy() { if (instance == this) instance = null; }
}

[Serializable]
public class Snapshot
{
    [NonSerialized] public List<Shootcolor> cannons = new List<Shootcolor>();
    public List<CubeState> cubeStates = new List<CubeState>();
}
[Serializable]
public class CubeState
{
    public Vector3 position;
    public Color color;
    public string name;
    public Quaternion rotation;
    public string cannonName;
    public CubeState(Vector3 position, Color color, string name, Quaternion rotation, string cannonName)
    {
        this.position = position; this.color = color; this.name = name;
        this.rotation = rotation; this.cannonName = cannonName;
    }
}
