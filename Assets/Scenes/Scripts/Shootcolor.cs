using UnityEngine;
using UnityEngine.EventSystems;

public class Shootcolor : MonoBehaviour
{
    public GameObject ball;
    public Color shootcolor;
    public string ColorName;
    public GameObject newCanon;
    public bool isright, isleft, istop, isdown;
    public bool canShoot = true;
    public ShootBall Sb;
    GameObject model;
    void Start()
    {
        model = Instantiate(newCanon, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localScale = Vector3.one;
        model.transform.localRotation = Quaternion.identity;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        SetColor(ColorName, shootcolor);
        GetComponent<BoxCollider>().size = new Vector3(1, 1, 3);
        GetComponent<BoxCollider>().center = new Vector3(0, 0, .3f);
    }
    public void SetColor(string colorName, Color color)
    {
        ColorName = colorName; shootcolor = color;
        if (model && model.transform.childCount > 0)
        {
            var renderer = model.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (renderer) renderer.material.color = color;
        }
    }
    // Taps are routed by CannonInput (every finger, and balls in flight never swallow a tap),
    // so Unity's single-pointer OnMouseDown is intentionally empty.
    public void OnMouseDown() { }
    public void Fire()
    {
        if (!canShoot || !GameManager.instance || GameManager.instance.once || CalmGameUI.InputBlocked) return;
        GameManager.instance.SaveState(this);
        var projectile = Instantiate(ball, transform.position, Quaternion.identity);
        projectile.GetComponent<MeshRenderer>().material.color = shootcolor;
        projectile.name = ColorName;
        SetDirection(projectile);
        GameManager.instance.RegisterShot(projectile.GetComponent<ShootBall>());
        GameFeel.Trail(projectile, shootcolor);
        GameFeel.Muzzle(projectile.transform.position, shootcolor);
        GameFeel.Bounce(model ? model.transform : transform);
        GameHaptics.Pulse(.35f);
        if (soundmanager.instance) soundmanager.instance.playmysound(soundmanager.instance.shooterClick);
        canShoot = false;
    }
    public void SetDirection(GameObject projectile)
    {
        var shot = projectile.GetComponent<ShootBall>();
        shot.isleft = isleft; shot.isright = isright; shot.istop = istop; shot.isdown = isdown;
        Destroy(projectile, 8);
    }
    public void OnMouseUp() { }
}
