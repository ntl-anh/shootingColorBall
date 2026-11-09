using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One presentation layer for every scene, including the legacy serialized levels.
public class CalmGameUI : MonoBehaviour
{
    public static CalmGameUI Instance { get; private set; }
    public static bool InputBlocked => Instance && (Instance.modal || Instance.finished);
    public static readonly string[] ThemeNames = JellyPopAssets.ThemeNames;
    static readonly Color[] Backgrounds = { Hex("DCDFFF"), Hex("E0F5EC"), Hex("FFE4DF"), Hex("423B69"),
        Hex("D7F4F5"), Hex("FFE8EB"), Hex("E3DDFF"), Hex("FFF4CB"), Hex("C7CFF6"), Hex("D5F1EC") };
    static readonly Color[] Accents = { Hex("9250ED"), Hex("8652DD"), Hex("A85BDE"), Hex("CDA7FF"),
        Hex("7354CE"), Hex("A64E93"), Hex("8051C4"), Hex("8160B6"), Hex("7453BC"), Hex("5969B1") };
    static readonly Color[] Inks = { Hex("50367D"), Hex("4F3676"), Hex("634078"), Hex("F7EFFF"),
        Hex("364F77"), Hex("683D62"), Hex("4D387B"), Hex("5C4873"), Hex("3E386F"), Hex("365D6B") };
    readonly List<Action> recolor = new List<Action>();
    readonly List<RectTransform> decorations = new List<RectTransform>();
    readonly List<Vector2> decorationPositions = new List<Vector2>();
    Canvas canvas;
    RectTransform safe;
    GameObject modal;
    Text progressText, comboText, undoText, timerText;
    Image progressFill;
    Font font;
    Sprite rounded;
    float comboUntil;
    bool finished, stuckOpen;
    int modalBindingStart;
    // Procedural skins (everything except Jelly Pop): flat colour palettes drawn with generated sprites.
    // The skin is chosen per scene load (see BuildAfterStart) so a level never mixes two looks.
    public static readonly string[] StyleNames = { "Neon Pop", "Tech tối", "Candy Pop", "Crystal Light", "Cartoon Lovely", "Modern 3D", "Nature Fresh" };
    static readonly string[] StyleTaglines = { "Neon sắc màu", "Công nghệ neon", "Ngọt ngào như kẹo", "Tinh thể lấp lánh", "Đáng yêu như hoạt hình", "Khối 3D hiện đại", "Xanh mát thiên nhiên" };
    class Skin
    {
        public Color bg, surface, ink, onBg, primary, onPrimary, secondary, onSecondary, border, gold;
        public Color confirm = new Color(.22f, .84f, .42f), onConfirm = new Color(.04f, .23f, .11f);
        public float borderThickness = 3.5f;
        public bool bevel;
        public bool Dark => bg.grayscale < .4f;
    }
    static readonly Skin[] Skins =
    {
        new Skin { bg = Hex("1B1352"), surface = Hex("C5B6F7"), ink = Hex("2B1A66"), onBg = Hex("F3EDFF"), primary = Hex("FFC62E"), onPrimary = Hex("3A2100"),
            secondary = Hex("8A5BF0"), onSecondary = Color.white, border = Hex("5A3EC8"), gold = Hex("FFC83D"), borderThickness = 6 },
        new Skin { bg = Hex("081224"), surface = Hex("11284A"), ink = Hex("EAF6FF"), onBg = Hex("EAF6FF"), primary = Hex("2BD9F2"), onPrimary = Hex("06101F"),
            secondary = Hex("0E1F3B"), onSecondary = Hex("EAF6FF"), border = Hex("2BD9F2"), gold = Hex("FFC93C") },
        new Skin { bg = Hex("FFD7EB"), surface = Hex("FFF4E6"), ink = Hex("5E2D5C"), onBg = Hex("6B2F63"), primary = Hex("FF5FA8"), onPrimary = Color.white,
            secondary = Hex("3FA9F5"), onSecondary = Color.white, border = Hex("FF8CC6"), gold = Hex("FFC400"), borderThickness = 6 },
        new Skin { bg = Hex("0E1546"), surface = Hex("17236B"), ink = Hex("E8F3FF"), onBg = Hex("E8F3FF"), primary = Hex("3EC8FF"), onPrimary = Hex("04123A"),
            secondary = Hex("1B2F8A"), onSecondary = Hex("E8F3FF"), border = Hex("63D2FF"), gold = Hex("9BE7FF"), bevel = true },
        new Skin { bg = Hex("BFE6FF"), surface = Hex("FFF2DD"), ink = Hex("6B4423"), onBg = Hex("4B3220"), primary = Hex("FF8A2B"), onPrimary = Color.white,
            secondary = Hex("6CCB5F"), onSecondary = Hex("20501C"), border = Hex("F0A248"), gold = Hex("FFB300"), borderThickness = 6 },
        new Skin { bg = Hex("0B0F1E"), surface = Hex("1B2440"), ink = Hex("EDF1FF"), onBg = Hex("EDF1FF"), primary = Hex("6C7BFF"), onPrimary = Color.white,
            secondary = Hex("2A3558"), onSecondary = Hex("DDE5FF"), border = Hex("9AA6E8"), gold = Hex("FFD24A"), bevel = true, borderThickness = 4 },
        new Skin { bg = Hex("20502D"), surface = Hex("F4E6C6"), ink = Hex("4A2E14"), onBg = Hex("F4E6C6"), primary = Hex("43A047"), onPrimary = Color.white,
            secondary = Hex("9A6531"), onSecondary = Hex("FFF3DC"), border = Hex("7B4A22"), gold = Hex("FFC93C"), borderThickness = 7 },
    };
    Skin skin;
    int activeStyle;
    // Style 0 uses the separate "Color Lab" art from Resources/LabUI whenever it is present.
    bool Lab => skin != null && activeStyle == 0 && LabArt.Get("pill_yellow") != null;
    readonly Dictionary<string, Sprite> shapes = new Dictionary<string, Sprite>();
    Sprite Shape(bool outline, float thickness)
    {
        string key = skin.bevel + "|" + outline + "|" + thickness;
        if (!shapes.TryGetValue(key, out var sprite)) shapes[key] = sprite = MakeShape(skin.bevel, outline, thickness);
        return sprite;
    }
    Color Bg => skin != null ? skin.bg : Backgrounds[GamePreferences.Theme];
    Color Accent => skin != null ? skin.primary : Accents[GamePreferences.Theme];
    Color Ink => skin != null ? skin.ink : Inks[GamePreferences.Theme];
    Color OnBg => skin != null ? skin.onBg : Ink;
    Color Surface => skin != null ? skin.surface : GamePreferences.Theme == 3 ? Hex("635987") : Hex("FCF8FF");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        var host = new GameObject("Jelly Pop presentation");
        DontDestroyOnLoad(host);
        Instance = host.AddComponent<CalmGameUI>();
    }
    void Awake()
    {
        font = Font.CreateDynamicFontFromOSFont(new[] { "Segoe UI", "Roboto", "Noto Sans", "Arial" }, 36);
        if (!font) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rounded = JellyPopAssets.Sprite("panel");
        if (!rounded) rounded = MakeRounded(false);
        SceneManager.sceneLoaded += Loaded;
        GamePreferences.Changed += ApplyTheme;
        GameEconomy.Changed += RefreshCoins;
        BuildCover();
    }
    void OnDestroy()
    {
        GameEconomy.Changed -= RefreshCoins;
        SceneManager.sceneLoaded -= Loaded;
        GamePreferences.Changed -= ApplyTheme;
        if (Instance == this) Instance = null;
    }
    // One persistent full-screen cover: it fades in before a scene changes and fades out after the new UI is built,
    // so neither the old scene's UI nor a half-built new scene is ever visible.
    CanvasGroup coverGroup;
    Image coverImage;
    Coroutine coverFade;
    void BuildCover()
    {
        var obj = new GameObject("Scene cover", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(Image));
        obj.transform.SetParent(transform, false);
        var coverCanvas = obj.GetComponent<Canvas>();
        coverCanvas.renderMode = RenderMode.ScreenSpaceOverlay; coverCanvas.sortingOrder = 30000;
        coverGroup = obj.GetComponent<CanvasGroup>(); coverGroup.alpha = 1; coverGroup.blocksRaycasts = true;
        coverImage = obj.GetComponent<Image>(); coverImage.color = Skins[0].bg;
    }
    IEnumerator FadeCover(float target, float seconds)
    {
        if (!coverGroup) yield break;
        coverGroup.blocksRaycasts = true;
        float start = coverGroup.alpha;
        for (float t = 0; t < seconds; t += Time.unscaledDeltaTime)
        {
            coverGroup.alpha = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        coverGroup.alpha = target;
        coverGroup.blocksRaycasts = target > 0;
        coverFade = null;
    }
    void SetCover(float target, float seconds)
    {
        if (coverFade != null) StopCoroutine(coverFade);
        coverFade = StartCoroutine(FadeCover(target, seconds));
    }
    // Every scene change goes through here: fade to the cover, load in the background, then the new UI fades in.
    public static void Load(string scene)
    {
        if (Instance) Instance.StartCoroutine(Instance.LoadRoutine(scene));
        else SceneManager.LoadScene(scene);
    }
    bool loading;
    IEnumerator LoadRoutine(string scene)
    {
        if (loading) yield break;
        loading = true;
        if (coverFade != null) StopCoroutine(coverFade);
        yield return coverFade = StartCoroutine(FadeCover(1, .15f));
        yield return SceneManager.LoadSceneAsync(scene);
        loading = false;
    }
    void Loaded(Scene scene, LoadSceneMode mode)
    {
        // Anything that loads a scene directly (not through Load) still gets covered immediately.
        foreach (var legacy in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (legacy.transform.root != transform) legacy.enabled = false;
        if (!coverGroup) BuildCover();
        if (coverFade != null) { StopCoroutine(coverFade); coverFade = null; }
        coverGroup.alpha = 1; coverGroup.blocksRaycasts = true;
        coverImage.color = Skins[Mathf.Clamp(GamePreferences.Style, 0, Skins.Length - 1)].bg;
        StartCoroutine(BuildAfterStart());
    }
    IEnumerator BuildAfterStart()
    {
        yield return null;
        if (canvas) Destroy(canvas.gameObject);
        recolor.Clear(); decorations.Clear(); decorationPositions.Clear();
        modal = null; finished = false; stuckOpen = false;
        activeStyle = GamePreferences.Style;
        skin = Skins[activeStyle];
        GamePreferences.SkinActive = skin != null;
        progressText = comboText = undoText = null;
        coinTexts.Clear();
        timerText = null;
        progressFill = null;
        foreach (var legacy in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None)) if (legacy.transform.root != transform) legacy.enabled = false;
        if (!FindFirstObjectByType<EventSystem>())
            new GameObject("UI input", typeof(EventSystem), typeof(StandaloneInputModule));
        var root = new GameObject("Jelly Pop UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720, 1280);
        scaler.matchWidthOrHeight = .5f;
        safe = Rect("Safe area", root.transform, Vector2.zero, Vector2.zero);
        UpdateSafeArea();
        if (LevelProgression.Current == 0) BuildHome();
        else { BuildGame(); if (skin != null) BuildGameBackdrop(); }
        foreach (var decoration in decorations) decorationPositions.Add(decoration.anchoredPosition);
        ApplyTheme();
        if (skin == null) JellyPopVisuals.ApplyScene();
        yield return null;
        SetCover(0, .25f);
    }
    void UpdateSafeArea()
    {
        var area = Screen.safeArea;
        safe.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
        safe.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
        safe.offsetMin = safe.offsetMax = Vector2.zero;
    }
    void Update()
    {
        if (!safe) return;
        UpdateSafeArea();
        for (int i = 0; i < decorations.Count; i++)
        {
            var d = decorations[i];
            if (d)
            {
                d.localRotation = Quaternion.Euler(0, 0, GamePreferences.Effects ? Mathf.Sin(Time.unscaledTime * .6f + i) * 8 : 0);
                d.anchoredPosition = decorationPositions[i] + (GamePreferences.Effects ? Vector2.up * Mathf.Sin(Time.unscaledTime * .8f + i) * 8 : Vector2.zero);
            }
        }
        if (comboText && Time.unscaledTime > comboUntil) comboText.text = "";
        var game = GameManager.instance;
        if (LevelProgression.Current > 0 && game && progressText)
        {
            int count = game._cubes.Count;
            int matches = 0;
            for (int i = 0; i < Mathf.Min(count, game._pattern.Count); i++)
                if (game._cubes[i].name == game._pattern[i].name) matches++;
            progressText.text = matches + " / " + count + " ô đúng màu   ·   Lượt " + game.Round + "/" + LevelProgression.Rounds;
            progressFill.fillAmount = Mathf.Lerp(progressFill.fillAmount, count > 0 ? (float)matches / count : 0, Time.unscaledDeltaTime * 8);
            undoText.text = "Hoàn tác · " + game.MaxUndoCount;
            if (timerText) timerText.text = game.Timed ? Mathf.CeilToInt(game.TimeRemaining) + " giây" : "Thư giãn";
        }
        if (!finished && !stuckOpen && Input.GetKeyDown(KeyCode.Escape)) { if (modal) CloseModal(); else OpenSettings(); }
    }
    // Home backdrop drawn in code: vertical gradient plus softly floating colour blocks and bubbles.
    void BuildBackdrop(Transform parent, string labBackground = "bg_sky")
    {
        var back = Rect("Backdrop", parent, Vector2.zero, Vector2.zero);
        Stretch(back); back.SetAsFirstSibling();
        var gradient = back.gameObject.AddComponent<RawImage>(); gradient.raycastTarget = false;
        gradient.texture = skin.Dark ? UiIcons.Gradient(Color.Lerp(skin.bg, skin.secondary, .38f), Color.Lerp(skin.bg, Color.black, .35f))
            : UiIcons.Gradient(Color.Lerp(skin.bg, Color.white, .5f), Color.Lerp(skin.bg, skin.primary, .22f));
        var scene = Lab ? LabArt.Texture(labBackground) : null;
        if (scene)
        {
            // Illustrated scene from the art sheet, scaled to cover the whole screen.
            var picture = Rect("Scene", back, Vector2.zero, Vector2.zero).gameObject.AddComponent<RawImage>();
            picture.texture = scene; picture.raycastTarget = false;
            var fit = picture.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; fit.aspectRatio = (float)scene.width / scene.height;
            return;
        }
        var rng = new System.Random(11);
        Color[] palette = { Hex("FF4F7B"), Hex("FFC93C"), Hex("3BE08A"), Hex("2BD9F2"), Hex("B07CFF") };
        for (int i = 0; i < 14; i++)
        {
            float size = 40 + rng.Next(0, 70);
            var color = palette[i % palette.Length];
            bool circle = i % 3 == 0;
            var deco = Rect("Decoration", back, new Vector2(rng.Next(-330, 330), rng.Next(-600, 600)), Vector2.one * size).gameObject.AddComponent<Image>();
            deco.sprite = circle ? UiIcons.Circle() : Shape(false, 0);
            deco.type = circle ? Image.Type.Simple : Image.Type.Sliced;
            deco.color = new Color(color.r, color.g, color.b, skin.Dark ? .3f : .4f);
            deco.raycastTarget = false;
            decorations.Add(deco.rectTransform);
        }
        // Big soft planets in the corners give the "space lab" depth.
        var planets = new[] { new Vector3(-300, 520, 360), new Vector3(330, -430, 440), new Vector3(-340, -560, 300) };
        for (int i = 0; i < planets.Length; i++)
        {
            var planet = Rect("Planet", back, new Vector2(planets[i].x, planets[i].y), Vector2.one * planets[i].z).gameObject.AddComponent<Image>();
            planet.sprite = UiIcons.Circle(); planet.raycastTarget = false;
            planet.color = skin.Dark ? new Color(skin.secondary.r, skin.secondary.g, skin.secondary.b, .22f) : new Color(1, 1, 1, .3f);
            planet.transform.SetAsFirstSibling();
        }
    }
    // Level scenes: the same coded backdrop on a canvas that sits far behind the board (depth-tested), replacing the scene floor.
    void BuildGameBackdrop()
    {
        UnityEngine.Camera view = null;
        foreach (var candidate in UnityEngine.Camera.allCameras) if (!candidate.targetTexture) { view = candidate; break; }
        if (!view) return;
        var root = new GameObject("Backdrop canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        var backdropCanvas = root.GetComponent<Canvas>();
        backdropCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        backdropCanvas.worldCamera = view;
        backdropCanvas.planeDistance = Mathf.Min(view.farClipPlane - 10, 900);
        backdropCanvas.sortingOrder = -50;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720, 1280);
        scaler.matchWidthOrHeight = .5f;
        BuildBackdrop(root.transform, "bg_lab");
        var ground = GameObject.Find("Ground");
        if (ground && ground.TryGetComponent<Renderer>(out var floor)) floor.enabled = false;
    }
    void BuildHomeSkin()
    {
        BuildBackdrop(canvas.transform);
        var logoArt = Lab ? LabArt.Get("logo") : null;
        if (logoArt)
        {
            var logo = Rect("Logo", safe, new Vector2(-55, 335), new Vector2(540, 540f * logoArt.rect.height / logoArt.rect.width)).gameObject.AddComponent<Image>();
            logo.sprite = logoArt; logo.preserveAspect = true; logo.raycastTarget = false;
            var mascot = LabArt.Get("icon_mascot");
            if (mascot)
            {
                var cat = Rect("Mascot", safe, new Vector2(250, 360), new Vector2(210, 210)).gameObject.AddComponent<Image>();
                cat.sprite = mascot; cat.preserveAspect = true; cat.raycastTarget = false;
                decorations.Add(cat.rectTransform);
            }
            var flask = LabArt.Get("icon_flask");
            if (flask)
            {
                var bottle = Rect("Flask", safe, new Vector2(-290, 262), new Vector2(96, 96)).gameObject.AddComponent<Image>();
                bottle.sprite = flask; bottle.preserveAspect = true; bottle.raycastTarget = false;
            }
        }
        else
        {
            var logoA = Label(safe, "Color Ball", new Vector2(0, 360), new Vector2(660, 120), 100, FontStyle.Bold, true);
            logoA.color = Color.white;
            var strokeA = logoA.gameObject.AddComponent<Outline>();
            strokeA.effectColor = new Color(.25f, .13f, .62f, 1); strokeA.effectDistance = new Vector2(4, -4);
            var logoB = Label(safe, "Shooting", new Vector2(0, 258), new Vector2(660, 120), 100, FontStyle.Bold, true);
            logoB.color = skin.primary;
            var strokeB = logoB.gameObject.AddComponent<Outline>();
            strokeB.effectColor = new Color(.45f, .2f, .05f, 1); strokeB.effectDistance = new Vector2(4, -4);
        }
        Label(safe, "Ghép màu thông minh", new Vector2(0, 180), new Vector2(650, 50), 26, FontStyle.Bold, true);
        var dots = new[] { Hex("FF4F9B"), Hex("FFC93C"), Hex("3BE08A"), Hex("2BD9F2") };
        for (int i = 0; i < dots.Length; i++)
        {
            var color = dots[i];
            var dot = Rect("Dot", safe, new Vector2(-105 + i * 70, 110), new Vector2(40, 40)).gameObject.AddComponent<Image>();
            dot.sprite = UiIcons.Circle(); dot.color = color; dot.raycastTarget = false;
            decorations.Add(dot.rectTransform);
        }
        BuildHomeMenu();
        Label(safe, "100 màn chơi · " + StyleNames[activeStyle] + " · " + StyleTaglines[activeStyle], new Vector2(0, -555), new Vector2(640, 40), 19, FontStyle.Normal, true);
    }
    // Shared home layout: coin bar on top, one big play button, four feature tiles, sound / how-to row.
    void BuildHomeMenu()
    {
        CoinBar(safe, new Vector2(-195, -50));
        IconButton(safe, "settings", "Cài đặt", new Vector2(290, -50), OpenSettings, 64, true);
        var play = Button(safe, "Chơi Ngay\nMàn " + LevelProgression.Saved, new Vector2(0, -40), new Vector2(580, 120), LevelProgression.Continue, true);
        var playText = play.GetComponentInChildren<Text>();
        playText.fontSize = 36; playText.resizeTextMaxSize = 36;
        playText.rectTransform.anchoredPosition = new Vector2(40, Lab ? 6 : 0);
        var playGlyph = Rect("Play", play.transform, new Vector2(-170, Lab ? 6 : 0), new Vector2(66, 66)).gameObject.AddComponent<Image>();
        playGlyph.sprite = UiIcons.Get("play"); playGlyph.raycastTarget = false; playGlyph.color = Lab ? Color.white : skin.onPrimary;
        recolor.Add(() => { if (playGlyph) playGlyph.color = Lab ? Color.white : skin.onPrimary; });
        if (Lab) { var playEdge = playGlyph.gameObject.AddComponent<Shadow>(); playEdge.effectColor = new Color(.55f, .25f, .02f, .8f); playEdge.effectDistance = new Vector2(2, -3); }
        string[] names = { "Nhiệm vụ", "Quà ngày", "Cửa hàng", "Chọn màn" };
        string[] icons = { "quest", "gift", "shop", "levels" };
        string[] labIcons = { "icon_calendar", "icon_gift", "icon_shop", "icon_trophy" };
        Action[] actions = { ShowQuests, ShowDaily, OpenStylePicker, ShowLevelSelect };
        bool[] badge = { GameEconomy.AnyQuestReady(), GameEconomy.DailyAvailable, false, false };
        for (int i = 0; i < 4; i++)
        {
            // The first three tiles are finished art (icon and caption included); the fourth uses the blank tile.
            bool baked = false;
            var tile = Button(safe, names[i], new Vector2(-234 + i * 156, -195), new Vector2(148, 112), actions[i], false, true, false, Lab ? "tile_blank" : null);
            var tileText = tile.GetComponentInChildren<Text>();
            tileText.rectTransform.anchoredPosition = new Vector2(0, Lab ? -31 : -35); tileText.rectTransform.sizeDelta = new Vector2(140, 30);
            tileText.fontSize = 19; tileText.resizeTextMaxSize = 19;
            var colorIcon = Lab ? LabArt.Get(labIcons[i]) : null;
            if (colorIcon)
            {
                var picture = Rect("Icon", tile.transform, new Vector2(0, 23), new Vector2(62, 62)).gameObject.AddComponent<Image>();
                picture.sprite = colorIcon; picture.preserveAspect = true; picture.raycastTarget = false;
            }
            else if (!baked)
            {
                var glyph = Rect("Glyph", tile.transform, new Vector2(0, 17), new Vector2(50, 50)).gameObject.AddComponent<Image>();
                glyph.sprite = UiIcons.Get(icons[i]); glyph.raycastTarget = false; glyph.preserveAspect = true; glyph.color = skin.onSecondary;
                recolor.Add(() => { if (glyph) glyph.color = skin.onSecondary; });
            }
            if (badge[i])
            {
                var dot = Panel(tile.transform, new Vector2(60, 42), new Vector2(26, 26), () => Hex("FF3B5C"));
                dot.raycastTarget = false;
            }
        }
        Label(safe, "Sao đã đạt: " + GameEconomy.TotalStars() + " / " + LevelProgression.TotalCount * 3, new Vector2(0, -275), new Vector2(560, 40), 20, FontStyle.Bold, true);
        Button(safe, "Âm thanh: " + (GamePreferences.Sound ? "Bật" : "Tắt"), new Vector2(-157, -350), new Vector2(290, 70), () => { GamePreferences.SetSound(!GamePreferences.Sound); Rebuild(); });
        Button(safe, "Cách chơi", new Vector2(157, -350), new Vector2(290, 70), ShowHelp);
    }
    void Rebuild() { Load(SceneManager.GetActiveScene().name); }
    readonly List<Text> coinTexts = new List<Text>();
    // Gold coin + balance pill with a green "+" that opens the shop; every pill follows the balance live.
    void CoinBar(Transform parent, Vector2 position, bool top = true)
    {
        bool art = Lab && LabArt.Get("coin") != null && LabArt.Get("plus_green") != null;
        var pill = Panel(parent, position, new Vector2(190, art ? 60 : 56), () => skin.Dark ? Color.Lerp(skin.bg, skin.secondary, .55f) : Surface, true, true, "pill_dark");
        if (top) AnchorTop(pill.rectTransform);
        var coin = Rect("Coin", pill.transform, new Vector2(-68, art ? 2 : 0), new Vector2(art ? 40 : 32, art ? 40 : 32)).gameObject.AddComponent<Image>();
        if (art) { coin.sprite = LabArt.Get("coin"); coin.color = Color.white; }
        else { coin.sprite = UiIcons.Circle(); coin.color = skin.gold; }
        coin.raycastTarget = false;
        var label = Label(pill.transform, GameEconomy.Coins.ToString(), new Vector2(-8, art ? 2 : 0), new Vector2(80, 44), 24, FontStyle.Bold, skin.Dark);
        label.resizeTextForBestFit = true; label.resizeTextMinSize = 12; label.resizeTextMaxSize = 24;
        coinTexts.Add(label);
        var plus = Panel(pill.transform, new Vector2(66, art ? 2 : 0), new Vector2(art ? 44 : 40, art ? 44 : 40), () => art ? Color.white : skin.confirm);
        if (art) plus.sprite = LabArt.Get("plus_green");
        var glyph = Rect("Plus", plus.transform, Vector2.zero, new Vector2(26, 26)).gameObject.AddComponent<Image>();
        glyph.sprite = UiIcons.Get("plus"); glyph.color = skin.onConfirm; glyph.raycastTarget = false;
        glyph.enabled = !art;
        var plusButton = plus.gameObject.AddComponent<Button>(); plusButton.targetGraphic = plus;
        plus.gameObject.AddComponent<JellyPopPress>();
        plusButton.onClick.AddListener(() => { GameHaptics.Pulse(.3f); if (!modal) OpenStylePicker(); });
    }
    void RefreshCoins()
    {
        coinTexts.RemoveAll(t => !t);
        foreach (var t in coinTexts) t.text = GameEconomy.Coins.ToString();
    }
    void BuildHome()
    {
        if (skin != null) { BuildHomeSkin(); return; }
        var background = Rect("Pastel scenery", canvas.transform, Vector2.zero, Vector2.zero);
        Stretch(background); background.SetAsFirstSibling();
        var bg = Rect("Jelly Pop background", background, Vector2.zero, Vector2.zero).gameObject.AddComponent<RawImage>();
        bg.raycastTarget = false;
        var fit = bg.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; fit.aspectRatio = .5f;
        recolor.Add(() => { if (bg) { bg.texture = JellyPopAssets.Background(GamePreferences.Theme); if (bg.texture) fit.aspectRatio = (float)bg.texture.width / bg.texture.height; bg.color = GamePreferences.Theme == 3 ? new Color(.48f, .42f, .66f) : Color.white; } });
        Label(safe, "J E L L Y  P O P", new Vector2(0, 445), new Vector2(650, 50), 22, FontStyle.Bold);
        var title = Label(safe, "Shooting\nColor", new Vector2(0, 295), new Vector2(640, 250), 84, FontStyle.Bold);
        var outline = title.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(1, 1, 1, .85f); outline.effectDistance = new Vector2(2, -2);
        Label(safe, "Chạm nhẹ. Bật sắc màu!", new Vector2(0, 130), new Vector2(650, 60), 25);
        var candyColors = new[] { Hex("FF706F"), Hex("FFC34F"), Hex("65DC9B"), Hex("B365EE"), Hex("60BEFF") };
        for (int i = 0; i < candyColors.Length; i++)
        {
            int index = i;
            var dot = Art(safe, "block", new Vector2(-160 + i * 80, 25), new Vector2(68, 68));
            dot.color = candyColors[index];
            decorations.Add(dot.rectTransform);
        }
        BuildHomeMenu();
        Label(safe, "100 màn chơi · Thế giới kẹo sắc màu", new Vector2(0, -555), new Vector2(640, 40), 19);
    }
    void BuildGame()
    {
        if (!FindFirstObjectByType<CannonInput>()) new GameObject("Cannon input").AddComponent<CannonInput>();
        // Floating HUD like the reference: no header / footer slab, only pills and buttons over the backdrop.
        var header = Panel(safe, new Vector2(0, -90), new Vector2(660, 152), () => Color.clear, true, false);
        header.rectTransform.anchorMin = header.rectTransform.anchorMax = new Vector2(.5f, 1);
        header.raycastTarget = false;
        IconButton(header.transform, "home", "Trang chủ", new Vector2(-299, 27), () => Load("MainMenu"), 60);
        Pill(header.transform, "Màn " + LevelProgression.Current.ToString("00"), new Vector2(-213, 27), new Vector2(100, 50), 22);
        timerText = Pill(header.transform, "", new Vector2(-60, 27), new Vector2(150, 50), 19);
        CoinBar(header.transform, new Vector2(165, 27), false);
        IconButton(header.transform, "settings", "Cài đặt", new Vector2(299, 27), OpenSettings, 60);
        progressText = Label(header.transform, "", new Vector2(0, -29), new Vector2(610, 40), 18, FontStyle.Normal, true);
        var track = Panel(header.transform, new Vector2(0, -59), new Vector2(600, 6), () => skin.Dark ? new Color(1, 1, 1, .18f) : Color.Lerp(Surface, Ink, .18f));
        progressFill = Panel(track.transform, Vector2.zero, new Vector2(600, 6), () => skin.confirm);
        progressFill.type = Image.Type.Filled; progressFill.fillMethod = Image.FillMethod.Horizontal; progressFill.fillAmount = 0;
        // Keep the original preview camera and render texture: the reference stays exact.
        var previewObject = GameObject.Find("Main CameraPrivew");
        var preview = previewObject ? previewObject.GetComponent<UnityEngine.Camera>() : null;
        if (preview && preview.targetTexture)
        {
            HidePreviewRig(previewObject);
            var card = Panel(safe, new Vector2(0, -295), new Vector2(250, 230), () => Surface, true, true, "panel_light");
            card.rectTransform.anchorMin = card.rectTransform.anchorMax = new Vector2(.5f, 1);
            previewCard = card.rectTransform;
            Pill(card.transform, "Mẫu cần phối", new Vector2(0, 108), new Vector2(180, 36), 16);
            var image = Rect("Target preview", card.transform, new Vector2(0, -13), new Vector2(190, 156)).gameObject.AddComponent<RawImage>();
            image.texture = preview.targetTexture; image.raycastTarget = false;
            float aspect = (float)preview.targetTexture.width / preview.targetTexture.height;
            image.rectTransform.sizeDelta = aspect >= 190f / 156 ? new Vector2(190, 190 / aspect) : new Vector2(156 * aspect, 156);
        }
        var footer = Panel(safe, new Vector2(0, 125), new Vector2(660, 114), () => Color.clear, true, false);
        footer.rectTransform.anchorMin = footer.rectTransform.anchorMax = new Vector2(.5f, 0);
        footer.raycastTarget = false;
        IconButton(footer.transform, "restart", "Chơi lại", new Vector2(-280, 0), () => { if (GameManager.instance) GameManager.instance.ResetLevel(); }, 76);
        var undo = Button(footer.transform, "Hoàn tác", Vector2.zero, new Vector2(380, 80), () => { if (GameManager.instance) GameManager.instance.UndoLastAction(); }, false, true, true);
        undoText = undo.GetComponentInChildren<Text>();
        IconButton(footer.transform, "info", "Cách chơi", new Vector2(280, 0), ShowHelp, 76);
        comboText = Label(safe, "", new Vector2(0, -150), new Vector2(640, 70), 36, FontStyle.Bold, true);
    }
    // Rounded purple pill with a light label, used for the level, timer and card captions.
    Text Pill(Transform parent, string content, Vector2 position, Vector2 size, int fontSize)
    {
        var pill = Panel(parent, position, size, () => skin.secondary, true, true, "pill_purple");
        pill.raycastTarget = false;
        var label = Label(pill.transform, content, new Vector2(0, Lab ? size.y * .04f : 0), size - new Vector2(8, 4), fontSize, FontStyle.Bold);
        label.resizeTextForBestFit = true; label.resizeTextMinSize = 10; label.resizeTextMaxSize = fontSize;
        recolor.RemoveAt(recolor.Count - 1);
        label.color = skin.onSecondary;
        recolor.Add(() => { if (label) label.color = skin.onSecondary; });
        return label;
    }
    // The target pattern, its black backdrop plane and the preview frame live inside the level and are rendered
    // to a texture by the preview camera. Move them to their own layer so the gameplay camera never draws them
    // behind our preview card (the preview camera still sees every layer).
    RectTransform previewCard;
    // Viewport height (0-1) of the preview card's lower edge; shots above it are drawn behind UI anyway.
    public float ShotHiddenAboveViewportY
    {
        get
        {
            if (!previewCard) return float.MaxValue;
            var corners = new Vector3[4];
            previewCard.GetWorldCorners(corners);
            return corners[0].y / Screen.height;
        }
    }
    const int PreviewLayer = 30;
    static void HidePreviewRig(GameObject previewCamera)
    {
        var pattern = GameObject.Find("Pattern");
        foreach (var root in new[] { pattern, previewCamera })
            if (root) foreach (var t in root.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = PreviewLayer;
        foreach (var camera in FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None))
            if (!camera.targetTexture) camera.cullingMask &= ~(1 << PreviewLayer);
    }
    public void ShowMessage(string text)
    {
        if (!comboText) return;
        comboText.text = text;
        comboText.fontSize = 28;
        comboUntil = Time.unscaledTime + 2.5f;
    }
    public void ShowRound(int round)
    {
        if (!comboText) return;
        comboText.text = "Lượt " + round + " · Đổi sắc màu!";
        comboText.fontSize = 30;
        comboUntil = Time.unscaledTime + 2.5f;
    }
    public void ShowStuck()
    {
        if (modal || finished) return;
        var card = Modal("Chưa đúng màu", "Đã bắn hết súng nhưng chưa khớp mẫu.\nHãy hoàn tác hoặc chơi lại để đổi thứ tự bắn.", 480);
        stuckOpen = true;
        Button(card, "Hoàn tác", new Vector2(0, -20), new Vector2(530, 80), () => { CloseModal(); if (GameManager.instance) GameManager.instance.UndoLastAction(); }, true);
        Button(card, "Chơi lại", new Vector2(0, -130), new Vector2(530, 80), () => GameManager.instance.ResetLevel());
    }
    public void ShowTimedOut()
    {
        finished = true;
        if (AdsManager.Ins) AdsManager.Ins.OnLevelEnded();
        var card = Modal("Thử một nhịp mới", "Hết thời gian lượt này.\nMột thứ tự bắn khác đang chờ bạn khám phá.");
        Button(card, "Chơi lại màn này", new Vector2(0, -100), new Vector2(530, 80), () => GameManager.instance.ResetLevel(), true);
        Button(card, "Về trang chủ", new Vector2(0, -210), new Vector2(530, 80), () => Load("MainMenu"));
    }
    public void ShowCompleted()
    {
        if (finished) return;
        finished = true;
        var card = Modal(skin != null ? "THẮNG!" : "Hoàn thành!", "Bạn vừa làm thế giới thêm một chút màu.");
        int earned = GameManager.instance ? GameManager.instance.StarsEarned : 3;
        int coins = GameEconomy.CompleteLevel(LevelProgression.Current, earned);
        for (int i = 0; i < 3; i++)
        {
            bool lit = i < earned;
            var position = new Vector2((i - 1) * 110, i == 1 ? 115 : 100);
            if (skin != null)
            {
                var star = Label(card, "★", position, new Vector2(100, 100), i == 1 ? 80 : 64, FontStyle.Bold);
                Func<Color> starColor = () => lit ? skin.gold : Color.Lerp(skin.surface, skin.ink, .25f);
                star.color = starColor();
                recolor.Add(() => { if (star) star.color = starColor(); });
                if (lit) GameFeel.Bounce(star.transform);
            }
            else
            {
                var star = Art(card, "star", position, Vector2.one * (i == 1 ? 108 : 88));
                if (!lit) star.color = new Color(.55f, .5f, .62f, .55f);
            }
        }
        Label(card, "MÀN " + LevelProgression.Current + "   ·   +" + coins + " xu", new Vector2(0, -15), new Vector2(560, 60), 28, FontStyle.Bold);
        Button(card, LevelProgression.Current == 100 ? "Trọn vẹn 100 màn · Về nhà" : "Màn tiếp theo  →", new Vector2(0, -130), new Vector2(530, 88), LevelProgression.Next, true);
        Button(card, "Về trang chủ", new Vector2(0, -240), new Vector2(530, 74), () => Load("MainMenu"));
    }
    public void OpenSettings()
    {
        OpenSettingsPage(GamePreferences.Theme / 4);
    }
    void OpenSettingsPage(int page)
    {
        if (modal) return;
        const int perPage = 4;
        int pageCount = Mathf.CeilToInt((float)ThemeNames.Length / perPage);
        page = Mathf.Clamp(page, 0, pageCount - 1);
        var card = Modal("Không gian của bạn", "Chỉnh một chút, chơi thật thoải mái.", 1130);
        CloseButton(card, "Đóng cài đặt", new Vector2(270, 520));
        Action rebuild = () => { CloseModal(); OpenSettingsPage(page); };
        ToggleRow(card, "Âm thanh", new Vector2(0, 355), 68, GamePreferences.Sound, on => GamePreferences.SetSound(on), rebuild);
        var vibrationRow = Panel(card, new Vector2(0, 265), new Vector2(550, 68), () => Color.Lerp(Surface, Accent, .09f), true, false, "bar_light");
        vibrationRow.gameObject.name = "Rung toàn bộ game";
        var vibrationToggle = vibrationRow.gameObject.AddComponent<Toggle>();
        vibrationToggle.targetGraphic = vibrationRow;
        vibrationToggle.isOn = GamePreferences.Haptics;
        var vibrationLabel = Label(vibrationRow.transform, "", new Vector2(-65, 0), new Vector2(380, 60), 23, FontStyle.Bold);
        var switchTrack = Panel(vibrationRow.transform, new Vector2(210, 0), new Vector2(90, 42),
            () => GamePreferences.Haptics ? Accent : Color.Lerp(Surface, Ink, .25f));
        switchTrack.raycastTarget = false;
        var switchKnob = Panel(switchTrack.transform, Vector2.zero, new Vector2(32, 32), () => Color.white);
        switchKnob.raycastTarget = false;
        var value = Label(card, "Mức rung · " + Mathf.RoundToInt(GamePreferences.Strength * 100) + "%", new Vector2(-60, 195), new Vector2(420, 40), 21);
        var sliderRect = Rect("Vibration strength", card, new Vector2(-65, 140), new Vector2(400, 44));
        var slider = sliderRect.gameObject.AddComponent<Slider>();
        var track = Panel(sliderRect, Vector2.zero, new Vector2(400, 8), () => Color.Lerp(Surface, Accent, .25f));
        var handleArea = Rect("Handle area", sliderRect, Vector2.zero, new Vector2(370, 44));
        var handle = Panel(handleArea, Vector2.zero, new Vector2(30, 30), () => Accent);
        slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.value = GamePreferences.Strength;
        var tryVibration = Button(card, "Thử", new Vector2(220, 140), new Vector2(100, 58), () => GameHaptics.Pulse(), false, false);
        var vibrationHint = Label(card, "", new Vector2(0, 82), new Vector2(560, 40), 16);
        Action refreshVibration = () =>
        {
            bool enabled = GamePreferences.Haptics;
            vibrationLabel.text = "Rung toàn bộ game · " + (enabled ? "Bật" : "Tắt");
            switchTrack.color = enabled ? Accent : Color.Lerp(Surface, Ink, .25f);
            switchKnob.rectTransform.anchoredPosition = new Vector2(enabled ? 23 : -23, 0);
            slider.interactable = enabled;
            tryVibration.interactable = GameHaptics.Enabled;
            value.text = "Mức rung · " + Mathf.RoundToInt(GamePreferences.Strength * 100) + "%";
            vibrationHint.text = !enabled ? "Đã tắt rung cho mọi thao tác và màn chơi."
                : GamePreferences.Strength <= 0 ? "Mức rung đang ở 0%. Tăng mức để thử rung."
                : Application.isMobilePlatform ? "Tự lưu cho toàn bộ game · Mức rung tùy thiết bị."
                : "Tự lưu cho toàn bộ game · Thử rung trên điện thoại.";
        };
        vibrationToggle.onValueChanged.AddListener(enabled =>
        {
            GamePreferences.SetHaptics(enabled);
            refreshVibration();
            if (enabled) GameHaptics.Pulse(.35f);
        });
        slider.onValueChanged.AddListener(v => { GamePreferences.SetStrength(v); refreshVibration(); });
        refreshVibration();
        if (skin != null)
            Label(card, "Phong cách " + StyleNames[activeStyle] + " dùng bộ màu riêng,\nkhông có bộ sưu tập hình nền.", new Vector2(0, -50), new Vector2(560, 70), 18);
        else
        {
            Label(card, "BỘ SƯU TẬP HÌNH NỀN", new Vector2(0, 20), new Vector2(560, 40), 18, FontStyle.Bold);
            for (int i = page * perPage; i < Mathf.Min(ThemeNames.Length, (page + 1) * perPage); i++)
            {
                int theme = i;
                int slot = i % perPage;
                Button(card, (GamePreferences.Theme == i ? "● " : "○ ") + ThemeNames[i], new Vector2(slot % 2 == 0 ? -143 : 143, -35 - slot / 2 * 68), new Vector2(270, 56), () => { GamePreferences.SetTheme(theme); rebuild(); }, GamePreferences.Theme == i);
            }
            var previous = Button(card, "← Trước", new Vector2(-205, -158), new Vector2(130, 40), () => { CloseModal(); OpenSettingsPage(page - 1); });
            previous.interactable = page > 0;
            Label(card, (page + 1) + " / " + pageCount + " · " + ThemeNames.Length + " lựa chọn", new Vector2(0, -158), new Vector2(260, 36), 17);
            var next = Button(card, "Sau →", new Vector2(205, -158), new Vector2(130, 40), () => { CloseModal(); OpenSettingsPage(page + 1); });
            next.interactable = page < pageCount - 1;
        }
        Button(card, "Phong cách giao diện  ·  " + StyleNames[GamePreferences.Style] + "  ›", new Vector2(0, -235), new Vector2(550, 64), OpenStylePicker);
        if (LevelProgression.Current > 0 && GamePreferences.Style != activeStyle)
            Label(card, "Sẽ áp dụng khi chơi lại hoặc sang màn mới.", new Vector2(0, -283), new Vector2(560, 30), 16);
        ToggleRow(card, "Hiệu ứng chuyển động", new Vector2(0, -325), 64, GamePreferences.Effects, on => GamePreferences.SetEffects(on), rebuild);
        Button(card, "Đặt lại hành trình…", new Vector2(0, -410), new Vector2(550, 62), ConfirmReset);
        Button(card, "Xong", new Vector2(0, -495), new Vector2(550, 70), CloseModal, true);
    }
    // Settings row with a pill switch (green when on).
    void ToggleRow(Transform parent, string title, Vector2 position, float height, bool value, Action<bool> set, Action rebuild)
    {
        var row = Panel(parent, position, new Vector2(550, height), () => Color.Lerp(Surface, Ink, .1f), true, false, "bar_light");
        var label = Label(row.transform, title, new Vector2(-90, 0), new Vector2(340, 50), 23, FontStyle.Bold);
        label.alignment = TextAnchor.MiddleLeft;
        var track = Panel(row.transform, new Vector2(210, 0), new Vector2(90, 42), () => value ? (skin != null ? skin.confirm : Accent) : Color.Lerp(Surface, Ink, .3f));
        track.raycastTarget = false;
        var knob = Panel(track.transform, new Vector2(value ? 23 : -23, 0), new Vector2(32, 32), () => Color.white);
        knob.raycastTarget = false;
        var toggle = row.gameObject.AddComponent<Button>(); toggle.targetGraphic = row;
        toggle.onClick.AddListener(() => { GameHaptics.Pulse(.3f); set(!value); rebuild(); });
    }
    // Style shop: styles with a price are unlocked once with coins, then selectable for free.
    void OpenStylePicker()
    {
        var card = Modal("Cửa hàng giao diện", "Mở khóa bằng xu kiếm từ màn chơi,\nnhiệm vụ và quà mỗi ngày.");
        var balance = Label(card, "Xu của bạn: " + GameEconomy.Coins, new Vector2(0, 170), new Vector2(500, 36), 22, FontStyle.Bold);
        var hint = Label(card, LevelProgression.Current > 0 ? "Trong màn chơi: áp dụng khi chơi lại hoặc sang màn mới." : "Đổi xong, trang chủ sẽ tải lại ngay.", new Vector2(0, -205), new Vector2(580, 30), 16);
        string[] styleIcons = { "icon_galaxy", "icon_layers", "icon_candy", "icon_crystal", "icon_cat", "icon_cube", "icon_leaf" };
        for (int i = 0; i < StyleNames.Length; i++)
        {
            int style = i;
            var position = new Vector2(i % 2 == 0 ? -143 : 143, 110 - i / 2 * 78);
            bool owned = GameEconomy.StyleOwned(i);
            string title = (GamePreferences.Style == i && !Lab ? "● " : "") + StyleNames[i] + (owned ? "" : "\n" + GameEconomy.StylePrice(i) + " xu");
            var styleButton = Button(card, title, position, new Vector2(270, 68), () =>
            {
                if (!GameEconomy.StyleOwned(style) && !GameEconomy.BuyStyle(style))
                {
                    hint.text = "Cần thêm " + (GameEconomy.StylePrice(style) - GameEconomy.Coins) + " xu. Thắng màn hoặc làm nhiệm vụ nhé!";
                    return;
                }
                GamePreferences.SetStyle(style);
                // Menus rebuild instantly; inside a level the board keeps its look until the next load.
                if (LevelProgression.Current == 0) { CloseModal(); Rebuild(); }
                else { CloseModal(); OpenSettings(); }
            }, GamePreferences.Style == i);
            var styleIcon = Lab ? LabArt.Get(styleIcons[i]) : null;
            if (styleIcon)
            {
                var badge = Rect("Style icon", styleButton.transform, new Vector2(-93, 2), new Vector2(38, 38)).gameObject.AddComponent<Image>();
                badge.sprite = styleIcon; badge.preserveAspect = true; badge.raycastTarget = false;
                var styleText = styleButton.GetComponentInChildren<Text>();
                styleText.rectTransform.anchoredPosition = new Vector2(22, 3); styleText.rectTransform.sizeDelta = new Vector2(150, 52);
            }
        }
        Button(card, "Xong", new Vector2(0, -280), new Vector2(530, 70), () => { CloseModal(); if (LevelProgression.Current > 0) OpenSettings(); }, true);
    }
    void ShowLevelSelect() { ShowLevelSelect(-1); }
    void ShowLevelSelect(int page)
    {
        const int perPage = 20;
        int pages = LevelProgression.TotalCount / perPage;
        if (page < 0) page = (LevelProgression.Saved - 1) / perPage;
        page = Mathf.Clamp(page, 0, pages - 1);
        var card = Modal("Chọn màn", "Màn " + (page * perPage + 1) + " – " + (page + 1) * perPage + "   ·   " + GameEconomy.TotalStars() + " sao");
        CloseButton(card, "Đóng", new Vector2(270, 315));
        for (int i = 0; i < perPage; i++)
        {
            int level = page * perPage + i + 1;
            int stars = GameEconomy.Stars(level);
            var cell = Button(card, level + (stars > 0 ? "\n" + new string('★', stars) : ""),
                new Vector2(-195 + i % 4 * 130, 120 - i / 4 * 80), new Vector2(120, 72),
                () => Load(level.ToString()), level == LevelProgression.Saved, true, false,
                level == LevelProgression.Saved ? "tile_yellow" : "tile_level");
            cell.interactable = level <= LevelProgression.Saved;
        }
        int current = page;
        var previous = Button(card, "← Trước", new Vector2(-200, -285), new Vector2(150, 56), () => ShowLevelSelect(current - 1));
        previous.interactable = page > 0;
        Label(card, (page + 1) + " / " + pages, new Vector2(0, -285), new Vector2(150, 40), 20);
        var next = Button(card, "Sau →", new Vector2(200, -285), new Vector2(150, 56), () => ShowLevelSelect(current + 1));
        next.interactable = page < pages - 1;
    }
    void ShowQuests()
    {
        var card = Modal("Nhiệm vụ hôm nay", "Làm mới mỗi ngày.\nHoàn thành để nhận xu.");
        CloseButton(card, "Đóng", new Vector2(270, 315));
        for (int i = 0; i < GameEconomy.QuestNames.Length; i++)
        {
            int quest = i;
            var row = Panel(card, new Vector2(0, 120 - i * 105), new Vector2(570, 92), () => Color.Lerp(Surface, skin != null ? skin.ink : Accent, .08f), true, false, "bar_light");
            Label(row.transform, GameEconomy.QuestNames[i], new Vector2(-115, 21), new Vector2(310, 36), 22, FontStyle.Bold);
            Label(row.transform, GameEconomy.QuestProgress(i) + " / " + GameEconomy.QuestTargets[i] + "   ·   +" + GameEconomy.QuestRewards[i] + " xu",
                new Vector2(-115, -17), new Vector2(310, 32), 18);
            bool claimed = GameEconomy.QuestClaimed(i), ready = GameEconomy.QuestComplete(i) && !claimed;
            var claim = Button(row.transform, claimed ? "Đã nhận" : ready ? "Nhận" : "Chưa xong", new Vector2(200, 0), new Vector2(150, 60),
                () => { GameEconomy.ClaimQuest(quest); ShowQuests(); }, false, true, ready);
            claim.interactable = ready;
        }
        Label(card, "Xu của bạn: " + GameEconomy.Coins, new Vector2(0, -200), new Vector2(500, 36), 20, FontStyle.Bold);
        Button(card, "Xong", new Vector2(0, -270), new Vector2(530, 70), CloseModal, true);
    }
    void ShowDaily()
    {
        var card = Modal("Quà mỗi ngày", "Điểm danh liên tiếp 7 ngày\nđể nhận quà lớn hơn.");
        CloseButton(card, "Đóng", new Vector2(270, 315));
        bool available = GameEconomy.DailyAvailable;
        int day = GameEconomy.DailyDay;
        for (int d = 0; d < 7; d++)
        {
            bool current = d == day && available, done = d < day || (d == day && !available);
            var position = d < 4 ? new Vector2(-195 + d * 130, 105) : new Vector2(-130 + (d - 4) * 130, 5);
            var tile = Panel(card, position, new Vector2(120, 92), () => current ? Color.Lerp(Surface, Accent, .4f)
                : Color.Lerp(Surface, skin != null ? skin.ink : Accent, done ? .2f : .07f), true, current, current ? "tile_yellow" : done ? "cell_green" : "cell_purple");
            var dayLabel = Label(tile.transform, "Ngày " + (d + 1) + "\n" + (done ? "Đã nhận" : "+" + GameEconomy.DailyRewards[d]), new Vector2(0, Lab ? 3 : 0), new Vector2(112, 80), 19, FontStyle.Bold);
            if (Lab)
            {
                recolor.RemoveAt(recolor.Count - 1);
                dayLabel.color = current ? skin.onPrimary : Color.white;
            }
        }
        var claim = Button(card, available ? "Nhận +" + GameEconomy.DailyRewards[day] + " xu" : "Hẹn gặp lại ngày mai", new Vector2(0, -115), new Vector2(530, 84),
            () => { GameEconomy.ClaimDaily(); ShowDaily(); }, false, true, available);
        claim.interactable = available;
        Label(card, "Xu của bạn: " + GameEconomy.Coins, new Vector2(0, -190), new Vector2(500, 36), 20, FontStyle.Bold);
        Button(card, "Xong", new Vector2(0, -270), new Vector2(530, 70), CloseModal);
    }
    void ConfirmReset()
    {
        CloseModal();
        var card = Modal("Bắt đầu lại?", "Tiến độ và lượt hoàn tác sẽ về ban đầu.\nCài đặt âm thanh, rung và giao diện vẫn được giữ.");
        Button(card, "Giữ hành trình", new Vector2(0, -100), new Vector2(530, 80), CloseModal, true);
        Button(card, "Đặt lại và về trang chủ", new Vector2(0, -210), new Vector2(530, 80), () => { LevelProgression.ResetProgress(); Load("MainMenu"); });
    }
    void ShowHelp()
    {
        var card = Modal("Chơi cùng sắc màu", "Quan sát mẫu, chạm từng khẩu pháo\nđể tô các ô thành đúng màu.");
        CloseButton(card, "Đóng hướng dẫn", new Vector2(270, 315));
        Label(card, "Thứ tự bắn tạo nên lời giải.\nHoàn tác khi cần thử một hướng khác.\nBắn thừa ít → nhiều sao → nhiều xu.\nXu mở giao diện mới trong Cửa hàng.\n\nMàn 51–75: hai lượt; 76–100: ba lượt.\nMàu đổi mỗi lượt, thời gian giảm dần.\nĐồng hồ chạy từ phát bắn đầu mỗi lượt.\nMở cài đặt để tạm dừng đồng hồ.", new Vector2(0, -20), new Vector2(550, 280), 22);
        Button(card, "Đã hiểu", new Vector2(0, -240), new Vector2(530, 80), CloseModal, true);
    }
    RectTransform Modal(string title, string subtitle, float height = 720)
    {
        CloseModal();
        modalBindingStart = recolor.Count;
        var dim = Panel(canvas.transform, Vector2.zero, Vector2.zero, () => skin != null ? (skin.Dark ? new Color(.01f, .03f, .08f, .72f) : new Color(.25f, .12f, .25f, .5f)) : new Color(.04f, .09f, .12f, .58f), false);
        Stretch(dim.rectTransform); modal = dim.gameObject;
        var card = Panel(dim.transform, Vector2.zero, new Vector2(650, height), () => Surface, true, true, "panel_light");
        // Fit both narrow phones and landscape displays without clipping controls.
        Canvas.ForceUpdateCanvases();
        var bounds = ((RectTransform)canvas.transform).rect.size;
        card.transform.localScale = Vector3.one * Mathf.Min(1, (bounds.x - 32) / 650, (bounds.y - 48) / height);
        if (GamePreferences.Effects) StartCoroutine(RevealCard(card.rectTransform, card.transform.localScale));
        if (skin != null)
        {
            // Title ribbon straddling the top edge of the card.
            var ribbon = Panel(card.transform, new Vector2(0, height / 2 - 8), new Vector2(400, 80), () => skin.secondary, true, true, "pill_purple");
            if (!Lab)
            {
                var lip = ribbon.gameObject.AddComponent<Shadow>();
                lip.effectColor = new Color(0, 0, 0, .35f); lip.effectDistance = new Vector2(0, -5);
            }
            var heading = Label(ribbon.transform, title, new Vector2(0, Lab ? 4 : 0), new Vector2(370, 70), 34, FontStyle.Bold);
            heading.resizeTextForBestFit = true; heading.resizeTextMinSize = 18; heading.resizeTextMaxSize = 34;
            recolor.RemoveAt(recolor.Count - 1);
            heading.color = skin.onSecondary;
            recolor.Add(() => { if (heading) heading.color = skin.onSecondary; });
            var edge = heading.gameObject.AddComponent<Outline>();
            edge.effectColor = new Color(.17f, .1f, .4f, .9f); edge.effectDistance = new Vector2(1.5f, -1.5f);
        }
        else
        Label(card.transform, title, new Vector2(0, height / 2 - 72), new Vector2(580, 70), 36, FontStyle.Bold);
        Label(card.transform, subtitle, new Vector2(0, height / 2 - 142), new Vector2(580, 80), 21);
        return card.rectTransform;
    }
    IEnumerator RevealCard(RectTransform card, Vector3 scale)
    {
        for (float t = 0; t < .2f; t += Time.unscaledDeltaTime)
        {
            if (!card) yield break;
            if (!GamePreferences.Effects) break;
            float p = t / .2f;
            card.localScale = scale * Mathf.LerpUnclamped(.93f, 1, 1 - Mathf.Pow(1 - p, 3));
            yield return null;
        }
        if (card) card.localScale = scale;
    }
    void CloseModal()
    {
        stuckOpen = false;
        if (!modal) return;
        // Prune bindings before destroying their graphics so repeated opening stays bounded.
        modal.SetActive(false); Destroy(modal); modal = null;
        recolor.RemoveRange(modalBindingStart, recolor.Count - modalBindingStart);
    }
    void ApplyTheme()
    {
        for (int i = recolor.Count - 1; i >= 0; i--)
        {
            try { recolor[i](); } catch (MissingReferenceException) { recolor.RemoveAt(i); }
        }
        foreach (var camera in FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None))
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = camera.targetTexture ? new Color(0, 0, 0, 0) : Bg;
        }
        foreach (var trail in FindObjectsByType<TrailRenderer>(FindObjectsSortMode.None)) trail.enabled = GamePreferences.Effects;
        if (!GamePreferences.Effects)
            foreach (var particle in FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
                if (particle.gameObject.name == "Color bloom") particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var ground = GameObject.Find("Ground");
        if (ground && ground.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.mainTexture = null;
            renderer.material.color = Color.Lerp(Bg, Surface, .3f);
        }
    }
    RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        var rect = obj.GetComponent<RectTransform>(); rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position; rect.sizeDelta = size;
        return rect;
    }
    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
    Image Panel(Transform parent, Vector2 position, Vector2 size, Func<Color> color, bool round = true, bool outlined = false, string labSprite = null)
    {
        var image = Rect("Surface", parent, position, size).gameObject.AddComponent<Image>();
        var art = round && labSprite != null && Lab ? LabArt.Get(labSprite) : null;
        if (art) { color = () => Color.white; outlined = false; }
        if (art) { image.sprite = art; image.type = art.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple; image.preserveAspect = art.border == Vector4.zero; }
        else if (round)
        {
            image.sprite = skin != null ? Shape(false, 0) : rounded; image.type = Image.Type.Sliced;
            if (skin == null) image.pixelsPerUnitMultiplier = 3;
        }
        if (round && size.x > 230 && size.y > 110)
        {
            var shadow = image.gameObject.AddComponent<Shadow>();
            shadow.effectColor = skin != null ? new Color(0, 0, 0, skin.Dark ? .35f : .2f) : new Color(.28f, .12f, .48f, .14f);
            shadow.effectDistance = new Vector2(0, skin != null ? -6 : -7);
        }
        image.color = color();
        recolor.Add(() => { if (image) image.color = color(); });
        if (skin != null && round && outlined)
        {
            // Big panels get the skin's full border; small buttons a thinner one so labels keep their room.
            var frame = Rect("Outline", image.transform, Vector2.zero, Vector2.zero).gameObject.AddComponent<Image>();
            Stretch(frame.rectTransform);
            frame.sprite = Shape(true, size.y < 100 ? Mathf.Min(3.5f, skin.borderThickness) : skin.borderThickness);
            frame.type = Image.Type.Sliced; frame.raycastTarget = false;
            frame.color = skin.border;
        }
        return image;
    }
    // onBackground: text sits directly on the scene background (not on a panel), so it needs the background-contrast ink.
    Text Label(Transform parent, string content, Vector2 position, Vector2 size, int fontSize, FontStyle style = FontStyle.Normal, bool onBackground = false)
    {
        var label = Rect("Label", parent, position, size).gameObject.AddComponent<Text>();
        label.font = font; label.text = content; label.fontSize = fontSize;
        label.fontStyle = style; label.alignment = TextAnchor.MiddleCenter;
        label.supportRichText = false; label.raycastTarget = false;
        Func<Color> ink = () => onBackground ? OnBg : Ink;
        label.color = ink();
        recolor.Add(() => { if (label) label.color = ink(); });
        return label;
    }
    Button Button(Transform parent, string title, Vector2 position, Vector2 size, Action action, bool primary = false, bool feedback = true, bool confirm = false, string labSprite = null)
    {
        Image surface;
        if (skin != null)
        {
            string art = labSprite ?? (confirm ? "pill_green" : primary ? "pill_yellow" : "pill_purple");
            surface = Panel(parent, position, size, () => confirm ? skin.confirm : primary ? skin.primary : skin.secondary, true, !primary && !confirm, art);
            // The cut-out art already carries its own bottom lip.
            if (!Lab)
            {
                var lip = surface.gameObject.AddComponent<Shadow>();
                lip.effectColor = new Color(0, 0, 0, skin.Dark ? .45f : .25f); lip.effectDistance = new Vector2(0, -5);
            }
        }
        else
        {
            surface = Panel(parent, position, size, () => Color.white);
            var sprite = JellyPopAssets.Sprite(primary ? "button-primary" : "button-secondary");
            if (sprite) surface.sprite = sprite;
            surface.pixelsPerUnitMultiplier = 3;
        }
        var button = surface.gameObject.AddComponent<Button>(); button.targetGraphic = surface;
        var colors = button.colors;
        if (skin != null)
        {
            colors.highlightedColor = new Color(.93f, .96f, 1f); colors.pressedColor = new Color(.7f, .74f, .82f);
            colors.disabledColor = new Color(.6f, .65f, .7f, .5f);
        }
        else
        {
            colors.highlightedColor = new Color(1, .96f, 1); colors.pressedColor = new Color(.78f, .64f, .9f);
            colors.disabledColor = new Color(.67f, .63f, .73f, .65f);
        }
        colors.fadeDuration = .1f; button.colors = colors;
        string icon = skin != null ? null : ButtonIcon(title);
        bool hasIcon = icon != null && size.x >= 170;
        if (hasIcon) Art(surface.transform, icon, new Vector2(-size.x / 2 + 32, 0), Vector2.one * Mathf.Min(43, size.y - 16));
        var text = Label(surface.transform, title, hasIcon ? new Vector2(22, 0) : Vector2.zero,
            size - new Vector2(hasIcon ? 68 : 24, 12), size.x < 200 ? 19 : 23, FontStyle.Bold);
        text.resizeTextForBestFit = true; text.resizeTextMinSize = 12; text.resizeTextMaxSize = title.Contains("\n") ? Mathf.Min(text.fontSize, 19) : text.fontSize;
        if (title.Contains("\n")) text.lineSpacing = .9f;
        // The art has a lip under the body, so the visual centre sits slightly above the rect centre.
        if (Lab) text.rectTransform.anchoredPosition += new Vector2(0, size.y * .04f);
        // Label() registered an Ink recolor, and ApplyTheme runs recolors last-to-first, so it would overwrite the
        // button text colour below (dark text on a purple primary button). Drop it; the button owns its text colour.
        recolor.RemoveAt(recolor.Count - 1);
        if (skin != null ? (!confirm && primary && skin.onPrimary.grayscale > .5f) : primary)
        {
            var edge = text.gameObject.AddComponent<Outline>();
            edge.effectColor = skin != null ? Color.Lerp(skin.primary, Color.black, .55f) : new Color(.29f, .16f, .54f, .85f);
            edge.effectDistance = new Vector2(1.5f, -1.5f);
        }
        Func<Color> textColor = () => skin != null ? (confirm ? skin.onConfirm : primary ? skin.onPrimary : skin.onSecondary) : (primary ? Color.white : Hex("50367D"));
        text.color = textColor();
        recolor.Add(() => { if (text) text.color = textColor(); });
        surface.gameObject.AddComponent<JellyPopPress>();
        button.onClick.AddListener(() => { if (feedback) GameHaptics.Pulse(.3f); action(); });
        return button;
    }
    void CloseButton(Transform parent, string title, Vector2 position)
    {
        if (skin != null) IconButton(parent, "close", title, position, CloseModal, 58);
        else IconButton(parent, "close", title, position, CloseModal);
    }
    Image Art(Transform parent, string asset, Vector2 position, Vector2 size)
    {
        var image = Rect(asset, parent, position, size).gameObject.AddComponent<Image>();
        image.sprite = JellyPopAssets.Sprite(asset); image.preserveAspect = true; image.raycastTarget = false;
        return image;
    }
    static void AnchorTop(RectTransform rect) { rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1); }
    // Square icon button using the shipped glyph art; skinned styles tint it with their own accent colour.
    RectTransform IconButton(Transform parent, string asset, string title, Vector2 position, Action action, float size = 70, bool top = false)
    {
        if (skin != null)
        {
            // Home, restart and settings are complete buttons in the art sheet; every other icon sits on the blank round button.
            bool baked = Lab && LabArt.Get("btn_" + asset) != null;
            var tile = Panel(parent, position, new Vector2(size, size), () => skin.secondary, true, true, baked ? "btn_" + asset : "btn_round");
            if (top) AnchorTop(tile.rectTransform);
            tile.gameObject.name = title;
            if (!Lab)
            {
                var lip = tile.gameObject.AddComponent<Shadow>();
                lip.effectColor = new Color(0, 0, 0, skin.Dark ? .45f : .25f); lip.effectDistance = new Vector2(0, -4);
            }
            if (!baked)
            {
                var glyph = Rect("Glyph", tile.transform, Vector2.zero, Vector2.one * size * .62f).gameObject.AddComponent<Image>();
                glyph.sprite = UiIcons.Get(asset); glyph.raycastTarget = false; glyph.preserveAspect = true;
                glyph.color = skin.onSecondary;
                recolor.Add(() => { if (glyph) glyph.color = skin.onSecondary; });
            }
            var tileButton = tile.gameObject.AddComponent<Button>(); tileButton.targetGraphic = tile;
            var tileColors = tileButton.colors;
            tileColors.highlightedColor = new Color(.93f, .96f, 1f); tileColors.pressedColor = new Color(.7f, .74f, .82f); tileColors.fadeDuration = .1f;
            tileButton.colors = tileColors;
            tile.gameObject.AddComponent<JellyPopPress>();
            tileButton.onClick.AddListener(() => { GameHaptics.Pulse(.3f); action(); });
            return tile.rectTransform;
        }
        var image = Art(parent, asset, position, new Vector2(size, size));
        if (top) AnchorTop(image.rectTransform);
        if (skin != null)
        {
            Func<Color> tint = () => Color.Lerp(Color.white, skin.primary, skin.Dark ? .55f : .75f);
            image.color = tint();
            recolor.Add(() => { if (image) image.color = tint(); });
        }
        image.gameObject.name = title; image.raycastTarget = true;
        var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        var colors = button.colors; colors.pressedColor = new Color(.8f, .66f, .94f); colors.fadeDuration = .1f; button.colors = colors;
        image.gameObject.AddComponent<JellyPopPress>();
        button.onClick.AddListener(() => { GameHaptics.Pulse(.3f); action(); });
        return image.rectTransform;
    }
    static string ButtonIcon(string title)
    {
        if (title.Contains("Âm thanh")) return GamePreferences.Sound ? "sound-on" : "sound-off";
        if (title.Contains("Cài đặt")) return "settings";
        if (title.Contains("Cách chơi") || title.Contains("Đã hiểu")) return "info";
        if (title.Contains("trang chủ") || title.Contains("Trang chủ")) return "home";
        if (title.Contains("Chơi lại") || title.Contains("Đặt lại")) return "restart";
        if (title.Contains("Hoàn tác")) return "undo";
        return null;
    }
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
    static Sprite MakeRounded(bool outline) { return MakeShape(false, outline, 3.5f); }
    // Nine-slice friendly rounded square, or a chamfered ("cut corner") square when bevel is set.
    static Sprite MakeShape(bool bevel, bool outline, float thickness)
    {
        const int size = 64;
        const float radius = 20, cut = 15;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
        {
            float px = Mathf.Abs(x + .5f - size / 2f), py = Mathf.Abs(y + .5f - size / 2f), sd;
            if (bevel)
                sd = Mathf.Max(Mathf.Max(px, py) - size / 2f, (px + py - (size - cut)) * .7071f);
            else
            {
                // Signed distance to a rounded square: negative inside.
                float qx = px - (size / 2f - radius), qy = py - (size / 2f - radius);
                sd = new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - radius;
            }
            float alpha = Mathf.Clamp01(.5f - sd);
            if (outline) alpha *= Mathf.Clamp01(sd + thickness);
            texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
    }
}
