using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBehavior : MonoBehaviour
{
    Vector3 dVelocity;
    public float speed;
    public Transform target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{
    //    dVelocity = speed * ((target.position + transform.position).normalized);

    //    rb.AddForce(dVelocity - rb.velocity);
    //}

    public Vector3 fleeVector()
    {
        return (target.position - transform.position) * -1 + transform.position;
    }
}