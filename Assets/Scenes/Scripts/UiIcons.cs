using System;
using System.Collections.Generic;
using UnityEngine;

// Icons and backdrop textures drawn at runtime (white on transparent, tinted by the UI), so the UI needs no image files.
public static class UiIcons
{
    const int Size = 96;
    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Get(string name)
    {
        if (cache.TryGetValue(name, out var sprite)) return sprite;
        Func<float, float, bool> shape = Shape(name);
        if (shape == null) return null;
        var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, true) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        const int samples = 3;
        for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                int hit = 0;
                for (int sy = 0; sy < samples; sy++)
                    for (int sx = 0; sx < samples; sx++)
                    {
                        float u = ((x + (sx + .5f) / samples) / Size) * 2 - 1;
                        float v = ((y + (sy + .5f) / samples) / Size) * 2 - 1;
                        if (shape(u, v)) hit++;
                    }
                texture.SetPixel(x, y, new Color(1, 1, 1, hit / (float)(samples * samples)));
            }
        texture.Apply();
        sprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(.5f, .5f), 100);
        cache[name] = sprite;
        return sprite;
    }

    // Soft vertical gradient for scene backdrops.
    public static Texture2D Gradient(Color top, Color bottom)
    {
        var texture = new Texture2D(2, 128, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        for (int y = 0; y < 128; y++)
            for (int x = 0; x < 2; x++) texture.SetPixel(x, y, Color.Lerp(bottom, top, y / 127f));
        texture.Apply();
        return texture;
    }

    // Filled circle with a soft 1px edge.
    public static Sprite Circle()
    {
        if (cache.TryGetValue("circle", out var sprite)) return sprite;
        const int size = 64;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(size / 2f, size / 2f)) - size / 2f;
                texture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(.5f - d)));
            }
        texture.Apply();
        sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(.5f, .5f), 100);
        cache["circle"] = sprite;
        return sprite;
    }

    static Func<float, float, bool> Shape(string name)
    {
        switch (name)
        {
            case "home":
                return (u, v) =>
                {
                    bool roof = Inside(new[] { new Vector2(-.92f, .02f), new Vector2(0, .85f), new Vector2(.92f, .02f) }, u, v);
                    bool body = u > -.66f && u < .66f && v > -.82f && v < .08f;
                    bool door = u > -.2f && u < .2f && v < -.2f;
                    return (roof || body) && !door;
                };
            case "restart":
            case "undo":
                return (u, v) =>
                {
                    if (name == "undo") u = -u;
                    float r = Mathf.Sqrt(u * u + v * v);
                    float angle = Mathf.Atan2(v, u) * Mathf.Rad2Deg;
                    bool ring = Mathf.Abs(r - .58f) < .14f && !(angle > 5 && angle < 80);
                    bool head = Inside(new[] { new Vector2(.30f, .92f), new Vector2(.30f, .26f), new Vector2(.92f, .59f) }, u, v);
                    return ring || head;
                };
            case "settings":
                return (u, v) =>
                {
                    float r = Mathf.Sqrt(u * u + v * v);
                    float theta = Mathf.Atan2(v, u);
                    float edge = .6f + (Mathf.Cos(theta * 8) > 0 ? .22f : 0);
                    return r < edge && r > .22f;
                };
            case "info":
                return (u, v) =>
                {
                    float r = Mathf.Sqrt(u * u + v * v);
                    bool dot = Mathf.Abs(u) < .11f && v > .32f && v < .56f;
                    bool bar = Mathf.Abs(u) < .11f && v > -.55f && v < .17f;
                    return r < .88f && !dot && !bar;
                };
            case "sound-on":
            case "sound-off":
                return (u, v) =>
                {
                    bool box = u > -.82f && u < -.5f && Mathf.Abs(v) < .24f;
                    bool cone = Inside(new[] { new Vector2(-.5f, .24f), new Vector2(-.06f, .62f), new Vector2(-.06f, -.62f), new Vector2(-.5f, -.24f) }, u, v);
                    if (name == "sound-off")
                    {
                        bool cross = Seg(u, v, .22f, .38f, .8f, -.38f) < .1f || Seg(u, v, .22f, -.38f, .8f, .38f) < .1f;
                        return box || cone || cross;
                    }
                    float r = Mathf.Sqrt((u + .06f) * (u + .06f) + v * v);
                    float angle = Mathf.Abs(Mathf.Atan2(v, u + .06f) * Mathf.Rad2Deg);
                    bool waves = angle < 42 && ((r > .42f && r < .56f) || (r > .72f && r < .86f));
                    return box || cone || waves;
                };
            case "close":
                return (u, v) => Seg(u, v, -.55f, -.55f, .55f, .55f) < .13f || Seg(u, v, -.55f, .55f, .55f, -.55f) < .13f;
            case "play":
                return (u, v) => Inside(new[] { new Vector2(-.55f, .8f), new Vector2(-.55f, -.8f), new Vector2(.8f, 0) }, u, v);
            case "plus":
                return (u, v) => (Mathf.Abs(u) < .13f && Mathf.Abs(v) < .65f) || (Mathf.Abs(v) < .13f && Mathf.Abs(u) < .65f);
            case "levels":
                return (u, v) => Mathf.Abs(Mathf.Abs(u) - .47f) < .35f && Mathf.Abs(Mathf.Abs(v) - .47f) < .35f && Mathf.Abs(u) > .12f && Mathf.Abs(v) > .12f;
            case "quest":
                return (u, v) =>
                {
                    bool body = Mathf.Abs(u) < .62f && v > -.85f && v < .7f;
                    bool clip = Mathf.Abs(u) < .3f && v > .55f && v < .92f;
                    bool lines = Mathf.Abs(u) < .38f && (Mathf.Abs(v - .22f) < .08f || Mathf.Abs(v + .08f) < .08f || Mathf.Abs(v + .38f) < .08f);
                    return (body || clip) && !lines;
                };
            case "gift":
                return (u, v) =>
                {
                    bool box = Mathf.Abs(u) < .72f && v > -.85f && v < .1f;
                    bool lid = Mathf.Abs(u) < .86f && v > .1f && v < .42f;
                    bool ribbon = Mathf.Abs(u) < .1f && v > -.85f && v < .42f;
                    float b1 = Vector2.Distance(new Vector2(u, v), new Vector2(-.28f, .66f)), b2 = Vector2.Distance(new Vector2(u, v), new Vector2(.28f, .66f));
                    bool bow = (Mathf.Abs(b1 - .22f) < .08f) || (Mathf.Abs(b2 - .22f) < .08f);
                    return ((box || lid) && !ribbon) || bow;
                };
            case "shop":
                return (u, v) =>
                {
                    bool awning = Inside(new[] { new Vector2(-.55f, .85f), new Vector2(.55f, .85f), new Vector2(.92f, .3f), new Vector2(-.92f, .3f) }, u, v);
                    bool body = Mathf.Abs(u) < .76f && v > -.85f && v < .22f;
                    bool door = Mathf.Abs(u + .25f) < .2f && v < -.2f;
                    bool window = u > .15f && u < .55f && v > -.35f && v < -.05f;
                    return (awning || body) && !door && !window;
                };
            case "star":
                return (u, v) =>
                {
                    var points = new Vector2[10];
                    for (int i = 0; i < 10; i++)
                    {
                        float a = Mathf.PI / 2 + i * Mathf.PI / 5, rr = i % 2 == 0 ? .95f : .42f;
                        points[i] = new Vector2(Mathf.Cos(a) * rr, Mathf.Sin(a) * rr);
                    }
                    return Inside(points, u, v);
                };
        }
        return null;
    }

    static float Seg(float px, float py, float ax, float ay, float bx, float by)
    {
        float dx = bx - ax, dy = by - ay;
        float t = Mathf.Clamp01(((px - ax) * dx + (py - ay) * dy) / (dx * dx + dy * dy));
        float cx = ax + t * dx - px, cy = ay + t * dy - py;
        return Mathf.Sqrt(cx * cx + cy * cy);
    }
    // Even-odd point in polygon.
    static bool Inside(Vector2[] poly, float x, float y)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            if ((poly[i].y > y) != (poly[j].y > y) && x < (poly[j].x - poly[i].x) * (y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                inside = !inside;
        return inside;
    }
}
