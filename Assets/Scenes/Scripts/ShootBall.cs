using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;

    public bool isright;
    public bool isleft;
    public bool istop;
    public bool isdown;
    public Shootcolor Sb;

    public float sphereSize = 0.4f;
    public int sphereInRow = 3;

    public float spherePivotDistance;
    public Vector3 spherePivot;
    public float explosionradius;
    public float explosionForce;
    public float explosionUpward;
    public float speed;

    public GameObject particle;

    const float SpeedMultiplier = 1.8f;
    const float ScreenMargin = .1f;
    const float LaneTolerance = .5f;
    UnityEngine.Camera view;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        Sb = FindObjectOfType<Shootcolor>();
        spherePivotDistance = sphereSize * sphereInRow / 2;
        spherePivot = new Vector3(spherePivotDistance, spherePivotDistance, spherePivotDistance);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
        // The shot lives until it has flown off the screen.
        if (!view)
        {
            // Skip the preview camera: it renders into a texture and sees a different part of the world.
            foreach (var cam in UnityEngine.Camera.allCameras)
                if (!cam.targetTexture) { view = cam; break; }
            if (!view) return;
        }
        var screen = view.WorldToViewportPoint(transform.position);
        if (screen.x < -ScreenMargin || screen.x > 1 + ScreenMargin || screen.y < -ScreenMargin || screen.y > 1 + ScreenMargin)
            Destroy(gameObject);
        else if (!hidden && CalmGameUI.Instance && screen.y > CalmGameUI.Instance.ShotHiddenAboveViewportY)
        {
            // Past the board and into the strip behind the header/preview cards: keep flying (and hitting)
            // but stop drawing, so the ball is not seen peeking out of the gaps between the cards.
            hidden = true;
            GetComponent<Renderer>().enabled = false;
            foreach (var trail in GetComponentsInChildren<TrailRenderer>()) { trail.emitting = false; trail.enabled = false; }
        }
    }
    bool hidden;
    GameObject _particle;

    public void move()
    {


        if (isleft)
        {
            rb.linearVelocity = Vector3.left * speed * SpeedMultiplier / 60f;
            if (_particle)
                _particle.transform.rotation = Quaternion.Euler(90, 0, 90);
        }
        else if (isright)
        {
            rb.linearVelocity = Vector3.right * speed * SpeedMultiplier / 60f;
            if (_particle)
                _particle.transform.rotation = Quaternion.Euler(90, 0, -90);
        }
        else if (istop)
        {
            rb.linearVelocity = Vector3.forward * speed * SpeedMultiplier / 60f;
            if (_particle)
                _particle.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else if (isdown)
        {
            rb.linearVelocity = Vector3.back * speed * SpeedMultiplier / 60f;
            if (_particle)
                _particle.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cube")
        {
            // Ignore decorative/reference cubes outside the playable board.
            if (!GameManager.instance || !GameManager.instance._cubes.Contains(other.gameObject)) return;
            // Only paint cubes in this shot's own lane; a neighbouring row/column must never be touched.
            Vector3 offset = other.transform.position - transform.position;
            float sideways = isleft || isright ? Mathf.Abs(offset.z) : Mathf.Abs(offset.x);
            if (sideways > LaneTolerance) return;
            other.gameObject.GetComponent<MeshRenderer>().material.color = gameObject.GetComponent<MeshRenderer>().material.color;
            other.gameObject.name = gameObject.name;
            var color = GetComponent<MeshRenderer>().material.color;
            GameFeel.Burst(other.transform.position, color);
            GameFeel.Bounce(other.transform);
            GameManager.instance.RegisterHit(other.gameObject);
        }


        if (other.gameObject.tag == "Left")
        {
            isleft = true;
            istop = false;
            isdown = false;
            isright = false;
        }
        if (other.gameObject.tag == "Right")
        {
            isleft = false;
            istop = false;
            isdown = false;
            isright = true;
        }
        if (other.gameObject.tag == "Top")
        {
            isleft = false;
            istop = true;
            isdown = false;
            isright = false;
        }
        if (other.gameObject.tag == "Down")
        {
            isleft = false;
            istop = false;
            isdown = true;
            isright = false;

        }

        if (other.gameObject.tag == "Object")
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (GameManager.instance && GameManager.instance.isActiveAndEnabled) GameManager.instance.FinishShot(this);
    }
    public void explode()
    {
        Debug.Log("explode");
        for (int x = 0; x < sphereInRow; x++)
        {
            for (int y = 0; y < sphereInRow; y++)
            {
                for (int z = 0; z < sphereInRow; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionradius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionradius, explosionUpward);
            }
        }
    }
    public void createPiece(int x, int y, int z)
    {
        GameObject Piece;
        Piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Piece.transform.position = transform.position + new Vector3(sphereSize * x, sphereSize * y, sphereSize * z) - spherePivot;
        Piece.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        Piece.AddComponent<Rigidbody>();
        Piece.GetComponent<Rigidbody>().mass = sphereSize;
        Piece.GetComponent<MeshRenderer>().material.color = Sb.shootcolor;
        Destroy(Piece, 0.3f);
    }
}

