using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehavior : MonoBehaviour
{

    public float speed;
    public float rad;
    public float jitter;
    public float dist; //projected unit circle center
    public Vector3 target;



    public Vector3 returnWanderPoints()
    {
        target = Vector3.zero;
        target = Random.insideUnitCircle.normalized * rad;
        target = (Vector2)target + Random.insideUnitCircle * jitter; //vec 2 because no fly

        target.z = target.y;
        target += transform.position;
        target += transform.forward * dist;


        target.y = transform.position.y;
        return target;
    }
}