using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float detectionDistance;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Detect()
    {
        Collider[] hits =
    Physics.OverlapSphere(transform.position, detectionDistance);

        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].tag != tag)
            {
                IDamageable enemy = hits[i].GetComponent<IDamageable>();
                if (enemy != null)
                {

                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }

    //If AI in sphere overlap start capture
    //If two or more pause capture
    //Once captured heal AI
}
