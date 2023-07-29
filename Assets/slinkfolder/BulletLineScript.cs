using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I TOOK THIS FROM SCASP!!
public class BulletLineScript : MonoBehaviour
{
    public Vector3 point0Point, point1Point;
    [SerializeField] float lifetime, zipSpeed;
    LineRenderer bulletRenderer;
    // Start is called before the first frame update
    void Start()
    {
        bulletRenderer = GetComponent<LineRenderer>();
        bulletRenderer.SetPosition(0, point0Point);
        bulletRenderer.SetPosition(1, point1Point);
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        //get zippy
        point0Point = Vector3.MoveTowards(point0Point, point1Point, zipSpeed);
        bulletRenderer.SetPosition(0, point0Point);
        //please work
    }
}
