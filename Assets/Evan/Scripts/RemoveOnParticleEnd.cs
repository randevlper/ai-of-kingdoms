using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnParticleEnd : MonoBehaviour
{
    public ParticleSystem ps;
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
            gameObject.SetActive(false);
        }
		
	}

    public void Setup()
    {
        ps.Play();
    }
}
