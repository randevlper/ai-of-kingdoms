using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior : MonoBehaviour
{
    Vector3 dVelocity;
    public float speed;
    public Transform target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update ()
    //   {
    //       dVelocity = speed * (target.position - transform.position).normalized;

    //       rb.AddForce(dVelocity - rb.velocity);
    //}

    public Vector3 returnTargetPos()
    {
        return target.position;
    }
}