using System.Collections.Generic;
using UnityEngine;

// Separate UI art cut from the "Color Lab Puzzle" sheet, loaded from Resources/LabUI. Every call returns null when the
// file is missing so the code-drawn UI keeps working without the images.
public static class LabArt
{
    static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

    public static Texture2D Texture(string name)
    {
        if (textures.TryGetValue(name, out var texture) && texture) return texture;
        texture = Resources.Load<Texture2D>("LabUI/" + name);
        if (texture) { texture.wrapMode = TextureWrapMode.Clamp; texture.filterMode = FilterMode.Bilinear; }
        textures[name] = texture;
        return texture;
    }

    // Art is drawn at 2x (200 pixels per unit); borders below are in 1x pixels: left, bottom, right, top.
    static Vector4 Border(string name)
    {
        switch (name)
        {
            case "pill_yellow": return new Vector4(64, 60, 64, 60);
            case "pill_purple": return new Vector4(34, 34, 34, 34);
            case "pill_dark": return new Vector4(34, 30, 34, 30);
            case "pill_green": return new Vector4(40, 38, 40, 38);
            case "tile_blank": return new Vector4(40, 40, 40, 40);
            case "tile_level": case "tile_yellow": return new Vector4(14, 20, 14, 20);
            case "bar_light": return new Vector4(26, 24, 26, 24);
            case "cell_green": case "cell_purple": return new Vector4(14, 30, 14, 30);
            case "panel_light": return new Vector4(44, 44, 44, 44);
        }
        return Vector4.zero;
    }

    public static Sprite Get(string name)
    {
        if (sprites.TryGetValue(name, out var sprite) && sprite) return sprite;
        var texture = Texture(name);
        sprite = texture ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 200, 0, SpriteMeshType.FullRect, Border(name) * 2) : null;
        sprites[name] = sprite;
        return sprite;
    }
}
