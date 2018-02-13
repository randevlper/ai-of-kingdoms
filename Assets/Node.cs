using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask mask;
    public float detectionDistance;
    public GameObject deathEffect;

    //These are only guards of the same kingdom
    public float captureRate;
    private float _capturePoints;
    private string _capturingTag;
    private bool _isBeingCaptured;
    private int _numberKnightsCapping;

    public float pointsToCapture;

    public GameObject[] guards;

    public bool isDebug;

    public MeshRenderer meshRenderer;

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
        _state.Push(States.NEUTRAL);
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
                //Defending();
                break;
            case States.CAPTURED:
                //Captured();
                break;
            default:
                break;
        }
    }

    void Neutral()
    {
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionDistance, mask);
        int numCapturing = 0;

        if (hits.Length == 0)
        {
            _capturingTag = null;
            _capturePoints = 0;
            _isBeingCaptured = true;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (_capturingTag == null)
            {
                _capturingTag = hits[i].tag;
                _isBeingCaptured = true;
            }
            else if (hits[i].tag != _capturingTag)
            {
                _isBeingCaptured = false;
                //Pause capture
            }
            if (hits[i].tag == _capturingTag)
            {
                numCapturing++;
                
            }
        }

        if (_isBeingCaptured && _capturingTag != tag)
        {
            _capturePoints += Time.deltaTime * captureRate * numCapturing;
            if (_capturePoints >= pointsToCapture)
            {
                Capture();
            }
        }
    }
    
    //Send message on capture to guards of old kingdom that they are now IDLE
    //Clear guards array
    //Send message to kingdom that their node is caputred
    void Capture()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        tag = _capturingTag;
        meshRenderer.material = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().GetAIMaterial(tag);
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
            //Will only return a gameObject with a diffrent tag
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
        if (isDebug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        }
    }

    //If AI in sphere overlap start capture
    //If two or more pause capture
    //Once captured heal AI
    void CheckCaptureStatus()
    {

    }
}
