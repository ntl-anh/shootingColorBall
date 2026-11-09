using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideCorner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.right,out hit,100))
        {
            if (hit.collider.gameObject.tag=="Ball")
            {
                gameObject.name = "Down";
                hit.collider.gameObject.tag = "RightSide";
            }
            if (hit.collider.gameObject.tag == "LeftSide")
            {
                gameObject.name = "Down";
            }
            
        }
        if(Physics.Raycast(transform.position, Vector3.back, out hit,100))
        {
            if (hit.collider.gameObject.tag == "Ball")
            {
                gameObject.name = "Right";
                hit.collider.gameObject.tag = "LeftSide";
            }
            if (hit.collider.gameObject.tag == "RightSide")
            {
                gameObject.name = "Right";
            }
            
        }
    }
}
