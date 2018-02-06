using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Knight : MonoBehaviour, IDamageable, IHealable
{

    public float maxHP;
    private float _HP;
    public float attack;

    public float detectionDistance;
	public float detectionTime;
	public float meleeCooldown;
	public float meleeDistance;
    public LayerMask detectionMask;
    public NavMeshAgent navAgent;
    [HideInInspector] private Vector3 destination;
    [HideInInspector] public GameObject target;
    UnityEvent action;

    //Idle if destination = position
    //Detecting

    //Moving goto destination
    //Detecting

    //Attacking = goTo enemy


    // Use this for initialization
    void Start()
    {
        _HP = maxHP;
        action = new UnityEvent();
        action.AddListener(Idle);
		Invoke("Detect", detectionTime);
    }

    // Update is called once per frame
    void Update()
    {
        action.Invoke();
    }

    void Idle()
    {
        navAgent.destination = transform.position;
        //Stand still and detect
    }

    void Move()
    {
		//name.Split(' ')[0] = "Resource";
        //Move and detect
        navAgent.destination = destination;
    }

    void Move(Vector3 pos)
    {
        destination = pos;
    }

    void Attack()
    {
        if (target == null)
        {
            SetState(Idle);
			return;
        }

        navAgent.destination = target.transform.position;
		Melee();
        //Pursue and attack
    }

	void Melee()
	{
		if(Vector3.Distance(target.transform.position, transform.position) < meleeDistance)
		{
			target.GetComponent<IDamageable>().Damage(attack * Time.deltaTime);
		}
	}

    void Detect()
    {
        //Check if an enemy is visible and attack them
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, detectionDistance, transform.forward, detectionMask);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            //IDamageable other = hits[i].collider.gameObject.GetComponent<IDamageable>();
            if (hits[i].collider.gameObject.tag != tag)
            {
                //Attack only closest
                if (target != null)
                {
					if (Vector3.Distance(hits[i].collider.transform.position, transform.position) 
						< Vector3.Distance(target.transform.position, transform.position))
					{
						target = hits[i].collider.gameObject;
					}
                }
                else
                {
                    target = hits[i].collider.gameObject;
                }
                SetState(Attack);
            }
        }

		Invoke("Detect", 1f);
    }

    void SetState(UnityAction call)
    {
        action.RemoveAllListeners();
        action.AddListener(call);
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position,detectionDistance);
	}

    //Vector3

    public void Damage(float damage)
    {
        _HP -= damage;
        if (_HP <= 0)
        {
            Death();
        }
    }


    //TODO: Set false and the Kingdom director can reset it
    void Death()
    {
        Destroy(gameObject);
    }

    public void Heal(float health)
    {
        _HP += health;
        if (_HP > maxHP)
        {
            health = maxHP;
        }
    }

}
