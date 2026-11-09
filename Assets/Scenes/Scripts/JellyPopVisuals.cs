using System.Collections.Generic;
using UnityEngine;

// Scene-owned presentation resources. Never replaces colliders, puzzle names or colors.
public class JellyPopVisuals : MonoBehaviour
{
    readonly List<Material> materials = new List<Material>();
    readonly Dictionary<Mesh, Mesh> roundedMeshes = new Dictionary<Mesh, Mesh>();
    readonly Dictionary<Shootcolor, MeshRenderer[]> cannons = new Dictionary<Shootcolor, MeshRenderer[]>();
    readonly MaterialPropertyBlock cannonState = new MaterialPropertyBlock();
    UnityEngine.Camera view;
    Transform backdrop;
    MeshRenderer backdropRenderer;
    Mesh backdropMesh;
    Material backdropMaterial;
    float lastAspect = -1;

    // Hides the opaque floor but keeps it as a shadow receiver, so blocks and cannons still cast shadows on the backdrop.
    public static void MakeShadowCatcher(Renderer floor)
    {
        var shader = Resources.Load<Shader>("JellyPop/ShadowCatcher");
        if (!shader) { floor.enabled = false; return; }
        floor.sharedMaterial = new Material(shader);
        floor.receiveShadows = true;
        floor.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    public static void ApplyScene()
    {
        var host = new GameObject("Jelly Pop scenery");
        host.AddComponent<JellyPopVisuals>().Build();
    }
    void Build()
    {
        var shader = Resources.Load<Shader>("JellyPop/JellySurface");
        var game = GameManager.instance;
        if (game)
        {
            foreach (var block in game._cubes) StyleBlock(block, shader);
            foreach (var block in game._pattern) StyleBlock(block, shader);
        }
        foreach (var cannon in FindObjectsByType<Shootcolor>(FindObjectsSortMode.None))
        {
            var renderers = cannon.GetComponentsInChildren<MeshRenderer>();
            cannons[cannon] = renderers;
            foreach (var renderer in renderers)
            {
                StyleMaterial(renderer, shader);
                // The cannon tube alone carries the puzzle color; its stand is decorative.
                if (shader && renderer.enabled && renderer.gameObject.name != "Trunk_Cannon" && renderer.sharedMaterial)
                    renderer.sharedMaterial.color = new Color(.79f, .71f, .93f);
            }
        }
        foreach (var camera in FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None))
        {
            if (!camera.targetTexture) view = camera;
            else { camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(0, 0, 0, 0); }
        }
        var ground = GameObject.Find("Ground");
        if (ground && ground.TryGetComponent<Renderer>(out var floor)) MakeShadowCatcher(floor);
        if (view && LevelProgression.Current > 0) BuildBackdrop();
        GamePreferences.Changed += Refresh;
        UnityEngine.Camera.onPreCull += BeforeCamera;
        Refresh();
    }
    void BeforeCamera(UnityEngine.Camera camera)
    {
        if (backdropRenderer) backdropRenderer.enabled = camera == view;
    }
    void StyleMaterial(MeshRenderer renderer, Shader shader)
    {
        if (!shader || !renderer.sharedMaterial) return;
        // Keep _Color intact: shooting, undo and multi-round palettes read this value.
        var material = renderer.material;
        var color = material.color;
        material.shader = shader;
        material.color = color;
        materials.Add(material);
    }
    void StyleBlock(GameObject block, Shader shader)
    {
        if (!block) return;
        if (block.TryGetComponent<MeshRenderer>(out var renderer)) StyleMaterial(renderer, shader);
        if (!block.TryGetComponent<MeshFilter>(out var filter) || !filter.sharedMesh) return;
        var original = filter.sharedMesh;
        if (!roundedMeshes.TryGetValue(original, out var mesh))
        {
            mesh = RoundedBox(original.bounds);
            roundedMeshes[original] = mesh;
        }
        filter.sharedMesh = mesh;
    }
    // Six subdivided faces projected onto a rounded box, retaining the source mesh bounds.
    static Mesh RoundedBox(Bounds bounds)
    {
        const int steps = 8;
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var triangles = new List<int>();
        var e = bounds.extents;
        float radius = Mathf.Min(e.x, e.y, e.z) * .55f;
        var inner = e - Vector3.one * radius;
        var axes = new[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        foreach (var normal in axes)
        {
            var u = Mathf.Abs(normal.y) > .5f ? Vector3.right : Vector3.up;
            var v = Vector3.Cross(normal, u);
            int start = vertices.Count;
            for (int y = 0; y <= steps; y++) for (int x = 0; x <= steps; x++)
            {
                var p = Vector3.Scale(normal + u * (2f * x / steps - 1) + v * (2f * y / steps - 1), e);
                var core = new Vector3(Mathf.Clamp(p.x, -inner.x, inner.x), Mathf.Clamp(p.y, -inner.y, inner.y), Mathf.Clamp(p.z, -inner.z, inner.z));
                var n = (p - core).normalized;
                vertices.Add(bounds.center + core + n * radius); normals.Add(n);
            }
            for (int y = 0; y < steps; y++) for (int x = 0; x < steps; x++)
            {
                int a = start + y * (steps + 1) + x, b = a + 1, c = a + steps + 1, d = c + 1;
                triangles.Add(a); triangles.Add(b); triangles.Add(c);
                triangles.Add(b); triangles.Add(d); triangles.Add(c);
            }
        }
        var mesh = new Mesh { name = "Jelly rounded block" };
        mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        return mesh;
    }
    void BuildBackdrop()
    {
        var shader = Resources.Load<Shader>("JellyPop/JellyBackdrop");
        if (!shader) return;
        backdrop = new GameObject("Pastel backdrop", typeof(MeshFilter), typeof(MeshRenderer)).transform;
        backdrop.SetParent(view.transform, false);
        backdropMesh = new Mesh { name = "Backdrop quad" };
        backdropMesh.vertices = new[] { new Vector3(-.5f,-.5f,0), new Vector3(.5f,-.5f,0), new Vector3(-.5f,.5f,0), new Vector3(.5f,.5f,0) };
        backdropMesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        backdropMesh.uv = new[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };
        backdropMesh.RecalculateBounds();
        backdrop.GetComponent<MeshFilter>().sharedMesh = backdropMesh;
        backdropMaterial = new Material(shader);
        backdropRenderer = backdrop.GetComponent<MeshRenderer>();
        backdropRenderer.sharedMaterial = backdropMaterial;
        backdropRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        backdropRenderer.receiveShadows = false;
        LateUpdate();
    }
    void Refresh()
    {
        if (!backdropMaterial) return;
        backdropMaterial.mainTexture = JellyPopAssets.Background(GamePreferences.Theme);
        backdropMaterial.color = GamePreferences.Theme == 3 ? new Color(.48f, .42f, .66f) : Color.white;
        lastAspect = -1;
    }
    void LateUpdate()
    {
        foreach (var pair in cannons)
        {
            if (!pair.Key) continue;
            foreach (var renderer in pair.Value)
            {
                if (!renderer) continue;
                renderer.GetPropertyBlock(cannonState);
                cannonState.SetFloat("_Spent", pair.Key.canShoot ? 0 : 1);
                renderer.SetPropertyBlock(cannonState);
            }
        }
        if (!view || !backdrop) return;
        float distance = Mathf.Lerp(view.nearClipPlane, view.farClipPlane, .9f);
        float height = view.orthographic ? 2 * view.orthographicSize : 2 * distance * Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f);
        backdrop.localPosition = Vector3.forward * distance;
        backdrop.localScale = new Vector3(height * view.aspect, height, 1);
        if (Mathf.Approximately(lastAspect, view.aspect)) return;
        lastAspect = view.aspect;
        // Cover the viewport without stretching the portrait illustration.
        var texture = backdropMaterial.mainTexture;
        float imageAspect = texture ? (float)texture.width / texture.height : .5f;
        float x = Mathf.Min(1, view.aspect / imageAspect), y = Mathf.Min(1, imageAspect / view.aspect);
        backdropMesh.uv = new[] { new Vector2((1-x)/2,(1-y)/2), new Vector2((1+x)/2,(1-y)/2), new Vector2((1-x)/2,(1+y)/2), new Vector2((1+x)/2,(1+y)/2) };
    }
    void OnDestroy()
    {
        GamePreferences.Changed -= Refresh;
        UnityEngine.Camera.onPreCull -= BeforeCamera;
        foreach (var material in materials) Release(material);
        foreach (var mesh in roundedMeshes.Values) Release(mesh);
        Release(backdropMesh); Release(backdropMaterial);
    }
    static void Release(Object resource)
    {
        if (!resource) return;
        if (Application.isPlaying) Destroy(resource); else DestroyImmediate(resource);
    }
}
