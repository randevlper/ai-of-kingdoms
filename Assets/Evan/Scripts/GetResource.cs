using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetResource : MonoBehaviour
{
    public Transform node;
    public Transform dropOff;
    NavMeshAgent agent;

    public bool toNode;
    public float resources;
    public float gatherSpeed;
    public int backpackCapacity;

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
	}

    // Update is called once per frame
    void Update()
    {
        if (toNode == true)
        {
            agent.destination = node.position;
            if(pathComplete())
            {
                resources += gatherSpeed;
                if (resources >= backpackCapacity)
                {
                    toNode = !true;
                }
            }
        }
        else if (toNode == false)
        {
            agent.destination = dropOff.position;
            if (pathComplete())
            {
                //CHeck if this gameobject had a IStorage
                //Insert resources

                resources -= gatherSpeed;
                if (resources <= 0)
                {
                    toNode = !false;
                }
            }
        }
    }


    protected bool pathComplete()
    {
        if (!agent.pathPending)
        {
            if (agent.stoppingDistance >= agent.remainingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
