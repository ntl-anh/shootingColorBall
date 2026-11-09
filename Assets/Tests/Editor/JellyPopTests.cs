using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class JellyPopTests
{
    static Type Visuals => AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JellyPopVisuals")).First(t => t != null);

    [Test]
    public void RoundBlockRetainsPuzzleColorColliderTransformAndBounds()
    {
        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var host = new GameObject("Presentation test");
        var originalMaterial = new Material(Shader.Find("Sprites/Default"));
        block.GetComponent<MeshRenderer>().sharedMaterial = originalMaterial;
        var color = new Color(.96f, .29f, .15f);
        originalMaterial.color = color;
        block.name = "Red";
        block.transform.position = new Vector3(3, 1, -2);
        block.transform.rotation = Quaternion.Euler(-90, 0, 0);
        var matrix = block.transform.localToWorldMatrix;
        var collider = block.GetComponent<BoxCollider>();
        var bounds = block.GetComponent<MeshFilter>().sharedMesh.bounds;
        try
        {
            var shader = Resources.Load<Shader>("JellyPop/JellySurface");
            Assert.IsNotNull(shader);
            var style = host.AddComponent(Visuals);
            Visuals.GetMethod("StyleBlock", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(style, new object[] { block, shader });
            Assert.AreEqual("Red", block.name);
            Assert.AreEqual(matrix, block.transform.localToWorldMatrix);
            Assert.AreSame(collider, block.GetComponent<BoxCollider>());
            Assert.AreEqual(Vector3.one, collider.size);
            Assert.AreEqual(color, block.GetComponent<MeshRenderer>().sharedMaterial.color);
            var mesh = block.GetComponent<MeshFilter>().sharedMesh;
            Assert.Less(Vector3.Distance(bounds.center, mesh.bounds.center), .0001f);
            Assert.Less(Vector3.Distance(bounds.size, mesh.bounds.size), .0001f);
            var vertices = mesh.vertices; var normals = mesh.normals; var indices = mesh.triangles;
            Assert.IsTrue(normals.All(n => Mathf.Abs(n.magnitude - 1) < .0001f));
            for (int i = 0; i < indices.Length; i += 3)
            {
                int a = indices[i], b = indices[i+1], c = indices[i+2];
                var face = Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]);
                Assert.Greater(Vector3.Dot(face, normals[a] + normals[b] + normals[c]), 0, "Outward, nondegenerate triangle");
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(block);
            UnityEngine.Object.DestroyImmediate(originalMaterial);
        }
    }
    [Test]
    public void PlayerResourcesIncludeScenerySpritesAndBothShaders()
    {
        foreach (string name in new[] { "clouds", "candy", "sunset", "icecream-coast", "strawberry-garden", "lavender-forest", "lemon-balloons", "moonlit-candy", "pearl-reef" })
            Assert.IsNotNull(Resources.Load<Texture2D>("JellyPop/Backgrounds/" + name), name);
        foreach (string name in new[] { "home", "restart", "settings", "info", "sound-on", "sound-off", "close", "undo", "button-primary", "button-secondary", "panel", "star", "block", "sparkle", "glow", "ring" })
            Assert.IsNotNull(Resources.Load<Texture2D>("JellyPop/UI/" + name), name);
        Assert.IsNotNull(Resources.Load<Shader>("JellyPop/JellySurface"));
        Assert.IsNotNull(Resources.Load<Shader>("JellyPop/JellyBackdrop"));
    }
    [Test]
    public void EverySelectableBackgroundResolvesAndLegacyIndicesStayStable()
    {
        var assets = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JellyPopAssets")).First(t => t != null);
        var method = assets.GetMethod("Background");
        var names = new[] { "clouds", "candy", "sunset", "clouds", "icecream-coast", "strawberry-garden", "lavender-forest", "lemon-balloons", "moonlit-candy", "pearl-reef" };
        Assert.AreEqual(names.Length, assets.GetProperty("ThemeCount").GetValue(null));
        for (int i = 0; i < names.Length; i++)
            Assert.AreSame(Resources.Load<Texture2D>("JellyPop/Backgrounds/" + names[i]), method.Invoke(null, new object[] { i }), "Theme " + i);
        Assert.AreSame(method.Invoke(null, new object[] { 0 }), method.Invoke(null, new object[] { -1 }));
        Assert.AreSame(method.Invoke(null, new object[] { 9 }), method.Invoke(null, new object[] { 999 }));
    }
}
