using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float detectionDistance;

    //These are only guards of the same kingdom
    public float captureRate;
    private float _capturePoints;

    public float pointsToCapture;

    public GameObject[] guards;

    public enum States
    {
        NEUTRAL,
        CAPTURED,
        DEFENDING
    }

    Stack<States> _state;

    public States GetCurrentState()
    {
        return _state.Peek();
    }

    // Use this for initialization
    void Start()
    {
        _state = new Stack<States>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state.Peek())
        {
            case States.NEUTRAL:
                Neutral();
                break;
            case States.DEFENDING:
                Defending();
                break;
            case States.CAPTURED:
                Captured();
                break;
            default:
                break;
        }
    }

    void Neutral()
    {
        GameObject closestEnemy = Detect();
        if (closestEnemy != null)
        {
            //start capturing
        }

    }

    void Defending()
    {
        GameObject closestEnemy = Detect();
        if (closestEnemy != null)
        {
            //Tell guards to attack
        }

    }

    void Captured()
    {
        GameObject closestEnemy = Detect();
        if (closestEnemy != null)
        {
            //Tell guards to attack
        }

    }

    GameObject Detect()
    {
        GameObject enemy = null;
        //Check if an enemy is visible and attack them
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionDistance);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].gameObject.GetComponent<IDamageable>();
            if (hits[i].gameObject.tag != tag && other != null)
            {

                if (enemy != null)
                {
                    //Attack only closest
                    if (Vector3.Distance(hits[i].transform.position, transform.position)
                        < Vector3.Distance(enemy.transform.position, transform.position))
                    {
                        enemy = hits[i].gameObject;
                    }
                }
                else
                {
                    enemy = hits[i].gameObject;
                    //SetState(Attack);
                    // if (calls.Peek() != Attack && (!isRunning || kingdom.isEnemyAttacking))
                    // {
                    //     calls.Push(Attack);
                    // }
                }
            }
        }
        //Invoke("Detect", detectionTime);
        return enemy;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }

    //If AI in sphere overlap start capture
    //If two or more pause capture
    //Once captured heal AI
    void CheckCaptureStatus()
    {

    }

    //Send message on capture to guards of old kingdom that they are now IDLE
    //Clear guards array
    //Send message to kingdom that their node is caputred
    void Capture()
    {

    }
}
