using System.Collections;
using UnityEngine;

public class GameFeel : MonoBehaviour
{
    static Material particleMaterial;
    static Material trailMaterial;
    static Material starMaterial;
    static Material ringMaterial;
    static Material plainMaterial;
    static bool Tech => GamePreferences.SkinActive;
    public static void Trail(GameObject ball, Color color)
    {
        var renderer = ball.GetComponent<MeshRenderer>();
        var shader = Resources.Load<Shader>("JellyPop/JellySurface");
        if (renderer && shader && !Tech) renderer.material.shader = shader;
        if (!GamePreferences.Effects) return;
        if (!trailMaterial) trailMaterial = new Material(Shader.Find("Sprites/Default"));
        var trail = ball.AddComponent<TrailRenderer>();
        trail.sharedMaterial = trailMaterial;
        // Balls travel ~18 units/s, so the trail time is the streak length: keep it short and in the ball's own
        // colour, otherwise a long pale streak is left across the screen after every shot.
        trail.time = .07f;
        trail.startWidth = .14f;
        trail.endWidth = 0;
        trail.startColor = color;
        trail.endColor = new Color(color.r, color.g, color.b, 0);
        trail.minVertexDistance = .05f;
        trail.numCapVertices = 6;
        trail.numCornerVertices = 3;
        var tail = new Gradient();
        tail.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
            new[] { new GradientAlphaKey(.6f, 0), new GradientAlphaKey(0, 1) });
        trail.colorGradient = tail;
        if (Tech)
        {
            // Comet: white-hot head fading into the ball colour.
            trail.time = .3f; trail.startWidth = .3f;
            var comet = new Gradient();
            comet.SetKeys(new[] { new GradientColorKey(Color.Lerp(color, Color.white, .6f), 0), new GradientColorKey(color, .45f), new GradientColorKey(color, 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(.8f, .45f), new GradientAlphaKey(0, 1) });
            trail.colorGradient = comet;
        }
    }
    static Gradient Fade(Color color)
    {
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.Lerp(color, Color.white, .35f), 0), new GradientColorKey(color, 1) },
            new[] { new GradientAlphaKey(.9f, 0), new GradientAlphaKey(.65f, .35f), new GradientAlphaKey(0, 1) });
        return gradient;
    }
    static Material SpriteMaterial(string asset)
    {
        var material = new Material(Shader.Find("Sprites/Default"));
        var sprite = JellyPopAssets.Sprite(asset);
        if (sprite) material.mainTexture = sprite.texture;
        return material;
    }
    public static void Muzzle(Vector3 position, Color color)
    {
        if (!GamePreferences.Effects) return;
        Burst(position, Color.Lerp(color, Color.white, .4f));
    }
    public static void Burst(Vector3 position, Color color, bool celebration = false)
    {
        if (!GamePreferences.Effects) return;
        var obj = new GameObject("Color bloom");
        obj.transform.position = position;
        var ps = obj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.loop = false;
        main.duration = .2f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(.25f, celebration ? 1.5f : .65f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.2f, celebration ? 6f : 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(.06f, .16f);
        main.startColor = color;
        if (Tech && celebration)
        {
            var confetti = new Gradient();
            confetti.SetKeys(new[] { new GradientColorKey(new Color(1f, .35f, .45f), 0), new GradientColorKey(new Color(1f, .8f, .2f), .25f),
                new GradientColorKey(new Color(.3f, .9f, .4f), .5f), new GradientColorKey(new Color(.25f, .75f, 1f), .75f), new GradientColorKey(new Color(.75f, .45f, 1f), 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            main.startColor = new ParticleSystem.MinMaxGradient(confetti) { mode = ParticleSystemGradientMode.RandomColor };
        }
        main.gravityModifier = celebration ? .5f : .15f;
        main.maxParticles = celebration ? 100 : 18;
        main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)(celebration ? 80 : 12)) });
        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = .12f;
        var size = ps.sizeOverLifetime; size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 1, 1, 0));
        var fade = ps.colorOverLifetime; fade.enabled = true;
        fade.color = Tech && celebration ? Fade(Color.white) : Fade(color);
        if (!particleMaterial) particleMaterial = SpriteMaterial("sparkle");
        if (!starMaterial) starMaterial = SpriteMaterial("star");
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (Tech)
        {
            if (!plainMaterial) plainMaterial = new Material(Shader.Find("Sprites/Default"));
            renderer.sharedMaterial = plainMaterial;
        }
        else renderer.sharedMaterial = celebration ? starMaterial : particleMaterial;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        var spin = ps.rotationOverLifetime; spin.enabled = true;
        spin.z = new ParticleSystem.MinMaxCurve(-2, 2);
        if (celebration) main.startSize = new ParticleSystem.MinMaxCurve(.1f, .24f);
        else if (!Tech) ImpactRing(obj.transform, color);
        ps.Play();
        Destroy(obj, 2.5f);
    }
    static void ImpactRing(Transform parent, Color color)
    {
        var ring = new GameObject("Jelly impact ring"); ring.transform.SetParent(parent, false);
        var ps = ring.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.loop = false; main.duration = .3f;
        main.startLifetime = .3f; main.startSpeed = 0; main.startSize = .8f; main.startColor = color;
        main.maxParticles = 1;
        var emission = ps.emission; emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)1) });
        var shape = ps.shape; shape.enabled = false;
        var size = ps.sizeOverLifetime; size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.EaseInOut(0, .25f, 1, 1.35f));
        var fade = ps.colorOverLifetime; fade.enabled = true; fade.color = Fade(color);
        if (!ringMaterial) ringMaterial = SpriteMaterial("ring");
        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = ringMaterial;
        ps.Play();
    }
    public static void Bounce(Transform target)
    {
        if (!GamePreferences.Effects) return;
        var feel = target.GetComponent<GameFeel>();
        if (!feel) feel = target.gameObject.AddComponent<GameFeel>();
        feel.PlayBounce();
    }
    Vector3 originalScale;
    bool bouncing;
    void PlayBounce()
    {
        if (bouncing) return;
        StartCoroutine(AnimateBounce());
    }
    IEnumerator AnimateBounce()
    {
        originalScale = transform.localScale;
        bouncing = true;
        for (float t = 0; t < .32f; t += Time.deltaTime)
        {
            float pulse = Mathf.Sin(t / .32f * Mathf.PI);
            transform.localScale = Vector3.Scale(originalScale, new Vector3(1 + pulse * .09f, 1 - pulse * .1f, 1 + pulse * .09f));
            yield return null;
        }
        transform.localScale = originalScale;
        bouncing = false;
    }
    void OnDisable() { if (bouncing) { transform.localScale = originalScale; bouncing = false; } }
}
