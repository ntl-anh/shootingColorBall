using System.Collections.Generic;
using UnityEngine;

// All generated art is project-local and included in players through Resources.
public static class JellyPopAssets
{
    // Keep the first four indices stable for existing saved preferences.
    public static readonly string[] ThemeNames = { "Đảo mây", "Thung lũng kẹo", "Hoàng hôn đào", "Mây đêm",
        "Biển kem", "Vườn dâu", "Rừng oải hương", "Khinh khí cầu chanh", "Đêm trăng kẹo", "San hô ngọc trai" };
    static readonly string[] BackgroundNames = { "clouds", "candy", "sunset", "clouds",
        "icecream-coast", "strawberry-garden", "lavender-forest", "lemon-balloons", "moonlit-candy", "pearl-reef" };
    public static int ThemeCount => ThemeNames.Length;
    static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    public static Sprite Sprite(string name)
    {
        if (sprites.TryGetValue(name, out var sprite)) return sprite;
        var texture = Resources.Load<Texture2D>("JellyPop/UI/" + name);
        if (!texture) return null;
        Vector4 border = Vector4.zero;
        if (name.StartsWith("button-")) border = new Vector4(82, 72, 82, 72);
        if (name == "panel") border = new Vector4(72, 72, 72, 72);
        sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, border);
        sprites[name] = sprite;
        return sprite;
    }
    public static Texture2D Background(int theme)
    {
        string name = BackgroundNames[Mathf.Clamp(theme, 0, BackgroundNames.Length - 1)];
        return Resources.Load<Texture2D>("JellyPop/Backgrounds/" + name);
    }
}
