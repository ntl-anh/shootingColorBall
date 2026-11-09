using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sample : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;

    

    public float sphereSize = 0.8f;
    public int sphereInRow = 3;

    public float spherePivotDistance;
    public Vector3 spherePivot;
    public float explosionradius;
    public float explosionForce;
    public float explosionUpward;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        spherePivotDistance = sphereSize * sphereInRow/2;
        spherePivot = new Vector3(spherePivotDistance, spherePivotDistance, spherePivotDistance);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="cube")
        {
            Debug.Log("triggered");
            explode();
            Destroy(gameObject);   
        }
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
        Debug.Log("piece");
        GameObject Piece;
        Piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Piece.transform.position = transform.position + new Vector3(sphereSize * x, sphereSize * y, sphereSize * z) - spherePivot;
        Piece.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        Piece.AddComponent<Rigidbody>();
        Piece.GetComponent<Rigidbody>().mass = sphereSize;
        Destroy(Piece, 1f);
    }
}

