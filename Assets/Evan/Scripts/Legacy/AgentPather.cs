using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentPather : MonoBehaviour
{
    NavMeshAgent agent;
    public Rigidbody target;
    Rigidbody rb;

    Vector3 dVelocity;
    Vector3 ProjectedPos;

    Rigidbody targetrb;

    public float projdis;
    public float speed;

    float dist;
    public float acceptableDistance;


    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        targetrb = target.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
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
                agent.destination = ProjectedPos;
            }
            else
            {
                agent.destination = target.transform.position;
            }
        }


        Debug.DrawLine(transform.position, ProjectedPos, Color.green);
        Debug.DrawLine(transform.position, targetrb.position, Color.red);
        Debug.DrawLine(targetrb.position, ProjectedPos, Color.blue);
        

	}
}
