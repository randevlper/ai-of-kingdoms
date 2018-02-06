using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PersuitBehavior : MonoBehaviour
{

    Rigidbody rb;

    Vector3 dVelocity;
    Vector3 ProjectedPos;

    NavMeshAgent targetrb;

    public float projdis;
    public float speed;
    public Transform target;

    float dist;
    public float acceptableDistance;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetrb = target.GetComponent<NavMeshAgent>();


    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != target.position)
        {
            acceptableDistance = (speed - targetrb.velocity.magnitude);

            projdis = dist;
            dist = Vector3.Distance(transform.position, target.position);

            if (dist > acceptableDistance)
            {
                ProjectedPos = target.position + (targetrb.velocity.normalized * (projdis - (speed - targetrb.velocity.magnitude)));
                dVelocity = speed * (ProjectedPos - transform.position).normalized;
                rb.AddForce(dVelocity - rb.velocity);
            }
            else
            {
                dVelocity = speed * (target.position - transform.position).normalized;

                rb.AddForce(dVelocity - rb.velocity);
            }
        }


        Debug.DrawLine(transform.position, ProjectedPos, Color.green);
        //Debug.DrawLine(transform.position, targetrb.position, Color.red);
        Debug.DrawLine(targetrb.transform.position, ProjectedPos, Color.blue);

    }
}