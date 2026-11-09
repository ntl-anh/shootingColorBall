using System.Collections.Generic;
using UnityEngine;

// Soft drop shadows drawn under every block and cannon. They do not depend on scene lighting, so blocks and cannons stay
// grounded ("raised") on top of the illustrated backdrop, which replaces the lit floor.
public class BlobShadows : MonoBehaviour
{
    class Blob { public Transform target; public Renderer[] renderers; public Transform quad; }
    readonly List<Blob> blobs = new List<Blob>();
    Material material;
    Texture2D texture;
    float groundY;
    Vector3 offsetDirection = new Vector3(.35f, 0, -.5f);

    public static void Create()
    {
        new GameObject("Blob shadows").AddComponent<BlobShadows>().Build();
    }
    void Build()
    {
        var ground = GameObject.Find("Ground");
        groundY = ground ? ground.transform.position.y : 0;
        var sun = FindFirstObjectByType<Light>();
        if (sun && sun.type == LightType.Directional)
        {
            var f = sun.transform.forward;
            var flat = new Vector3(f.x, 0, f.z);
            if (flat.sqrMagnitude > .001f) offsetDirection = flat.normalized * .45f;
        }
        texture = SoftSquare();
        material = new Material(Shader.Find("Sprites/Default")) { mainTexture = texture, color = new Color(.02f, 0, .12f, .5f) };
        var game = GameManager.instance;
        if (game) foreach (var cube in game._cubes) Add(cube);
        foreach (var cannon in FindObjectsByType<Shootcolor>(FindObjectsSortMode.None)) Add(cannon.gameObject);
    }
    void Add(GameObject target)
    {
        if (!target) return;
        var renderers = target.GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length == 0) return;
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(quad.GetComponent<Collider>());
        quad.name = "Shadow";
        quad.transform.SetParent(transform, false);
        quad.transform.rotation = Quaternion.Euler(90, 0, 0);
        var r = quad.GetComponent<MeshRenderer>();
        r.sharedMaterial = material;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        blobs.Add(new Blob { target = target.transform, renderers = renderers, quad = quad.transform });
    }
    void LateUpdate()
    {
        foreach (var b in blobs)
        {
            if (!b.target) { if (b.quad) b.quad.gameObject.SetActive(false); continue; }
            Bounds bounds = default; bool any = false;
            foreach (var r in b.renderers)
            {
                if (!r || !r.enabled) continue;
                if (!any) { bounds = r.bounds; any = true; } else bounds.Encapsulate(r.bounds);
            }
            bool visible = any && b.target.gameObject.activeInHierarchy;
            b.quad.gameObject.SetActive(visible);
            if (!visible) continue;
            float size = Mathf.Max(bounds.size.x, bounds.size.z) * 1.35f;
            b.quad.position = new Vector3(bounds.center.x, groundY + .02f, bounds.center.z) + offsetDirection * Mathf.Max(bounds.size.x, bounds.size.z) * .3f;
            b.quad.localScale = new Vector3(size, size, 1);
        }
    }
    void OnDestroy()
    {
        if (material) Destroy(material);
        if (texture) Destroy(texture);
    }
    static Texture2D SoftSquare()
    {
        const int n = 64;
        var t = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float dx = Mathf.Abs((x + .5f) / n - .5f) * 2, dy = Mathf.Abs((y + .5f) / n - .5f) * 2;
            float d = Mathf.Max(dx, dy);                    // square falloff, softened at the rim
            float a = Mathf.Clamp01((1 - d) / .45f);
            t.SetPixel(x, y, new Color(1, 1, 1, a * a * (3 - 2 * a)));
        }
        t.Apply();
        return t;
    }
}
