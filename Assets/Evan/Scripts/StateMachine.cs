using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum States
{
    wanderState,    //0
    seekState,      //1
    fleeState,      //2
    startoverState  //3

}

public class StateMachine : MonoBehaviour
{


    NavMeshAgent agent;
    WanderBehavior wander;
    SeekBehavior seek;
    FleeBehavior flee;
    public States currentState;//controls current state

    public float transitionTime;
    float timer;


    // Use this for initialization


    void Start()
    {
        timer = transitionTime;
        agent = GetComponent<NavMeshAgent>();
        wander = GetComponent<WanderBehavior>();
        seek = GetComponent<SeekBehavior>();
        flee = GetComponent<FleeBehavior>();
    }

    void switchStates()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            currentState++;
            if (currentState == States.startoverState)
            {
                currentState = States.wanderState;
            }
            timer = transitionTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.wanderState:
                agent.destination = wander.returnWanderPoints();
                wander.returnWanderPoints();
                break;
            case States.seekState:
                agent.destination = seek.returnTargetPos();
                seek.returnTargetPos();
                break;
            case States.fleeState:

                agent.destination = flee.fleeVector();
                break;
        }
        switchStates();
    }
}