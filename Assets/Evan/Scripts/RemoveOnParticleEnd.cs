using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnParticleEnd : MonoBehaviour
{
    ParticleSystem ps;
	// Use this for initialization
	void Start ()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        ps.loop = false;


    }
	
	// Update is called once per frame
	void Update ()
    {
        if(ps.isStopped)
        {
            Destroy(gameObject);
        }
		
	}
}
