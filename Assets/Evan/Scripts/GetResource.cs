﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetResource : MonoBehaviour,IDamageable
{
    public bool debug = false;

    public Animator anim;

    public GameObject explosion;

    public LayerMask mask;

    public Transform node;
    public Transform dropOff;
    NavMeshAgent agent;

    public bool toNode;
    public float resources;
    public float gatherSpeed;
    public int backpackCapacity;
    public float detectionDistanceFlee;
    public float detectionDistanceWork;
    float activeDetectionDistance;

    public float fleeSpeed;

    IStorage baseStorage;

    GameObject closestKnight = null;

    public float health;
    public void Damage(float damage)
    {
        health -= damage;
    }
    
    Stack<States> state;
    enum States
    {
        IDLE,
        GATHER,
        FLEE
    }

    void Detect()
    {
        
        Collider[] hits =
        Physics.OverlapSphere(transform.position, activeDetectionDistance, mask);
        for (int i = 0; i < hits.Length; ++i)
        {

            if (state.Peek() != States.FLEE
                &&
                hits[i].GetComponent<Collider>().gameObject.tag != tag
                &&
                hits[i].GetComponent<Collider>().gameObject.name.Split('(')[0] == "Knight")
            {
                state.Push(States.FLEE);
                //Debug.Log("Fleeing"); 
                GameObject other = hits[i].gameObject;
                //Tell worker to run away
                if (closestKnight == null)
                {
                    closestKnight = hits[i].gameObject;
                }
                else if (Vector3.Distance(other.transform.position, transform.position)
                        < Vector3.Distance(closestKnight.transform.position, transform.position))
                {
                    closestKnight = other;
                }
            }
        }
        if (hits.Length == 0
            &&
            state.Peek() != States.GATHER)
        {
            //Debug.Log("Gathering");
            state.Push(States.GATHER);
        }
    }

    void flee()
    //Vector3 dir = (myPos - otherGuy.pos).normalize
    //destination = myPos + dir
    {
        if (closestKnight != null)
        {
            Vector3 dir = (closestKnight.transform.position - agent.transform.position).normalized;
            agent.destination = agent.transform.position - (dir * fleeSpeed);

            activeDetectionDistance = detectionDistanceFlee;
        }
        else
        {
            state.Pop();
        }
    }

    void collectDeliver()
    {
        activeDetectionDistance = detectionDistanceWork;
        if (toNode == true)
        {
            agent.destination = node.position;
            if (pathComplete())
            {
                resources += (gatherSpeed * Time.deltaTime);
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
                if (baseStorage != null)
                {
                    baseStorage.Insert((gatherSpeed * Time.deltaTime));
                }


                resources -= (gatherSpeed * Time.deltaTime);
                if (resources <= 0)
                {
                    toNode = !false;
                }
            }
        }
    }
    
    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        baseStorage = dropOff.gameObject.GetComponent<IStorage>();

        state = new Stack<States>();
        state.Push(States.GATHER);
    }

    // Update is called once per frame
    void Update()
    {
        anim.gameObject.transform.localPosition = new Vector3(0, -1, 0);

        switch (state.Peek())
        {
            case States.GATHER:
                collectDeliver();
                break;
            case States.FLEE:
                flee();
                break;
            default:
                break;
        }
                      
        

        if (health <= 0)
        {
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
        Detect();
    }

    protected bool pathComplete()
    {
        if (!agent.pathPending)
        {
            if (agent.stoppingDistance >= agent.remainingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    anim.SetBool("isMoving",false);
                    return true;
                }
            }
        }
        anim.SetBool("isMoving", true);
        return false;
    }

    void OnDrawGizmos()
    {
        if (debug == !false)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, activeDetectionDistance);
        }
    }
}