using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Knight : MonoBehaviour, IDamageable, IHealable, IAI
{

    public float maxHP;

    [SerializeField] private float _HP;
    public float attack;

    public float detectionDistance;
    public float detectionTime;
    public float meleeCooldown;
    public float meleeDistance;
    private float meleeTimer;
    public float critChance;
    public float critMultiplier;

    public int AINum;
    //public LayerMask detectionMask;
    public NavMeshAgent navAgent;
    [HideInInspector] private Vector3 destination;
    public GameObject target;
    UnityEvent action;

    Stack<UnityAction> calls;

    public Material flagMaterial;
    public GameObject flag;

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
        Invoke("Detect", 0);

        calls = new Stack<UnityAction>();
        calls.Push(Idle);
        calls.Push(Move);
    }

    // Update is called once per frame
    void Update()
    {
        //action.Invoke();
        calls.Peek().Invoke();
        meleeTimer += Time.deltaTime;
    }

    void Idle()
    {
        navAgent.destination = transform.position;
        //Stand still and detect
    }

    void Move()
    {
        navAgent.destination = destination;

        if (transform.position == destination)
        {
            calls.Pop();
        }
    }

    public void Move(Vector3 pos)
    {
        destination = pos;

        if (calls == null) { return; }
        if (!(calls.Peek() == Attack))
        {
            if (calls.Peek() == Move)
            {
                calls.Pop();
            }
            calls.Push(Move);
        }
    }

    void Attack()
    {
        if (target == null)
        {
            //SetState(Idle);
            calls.Pop();
            Detect(true);
            return;
        }
        else
        {
            navAgent.destination = target.transform.position;
            Melee();
            //Pursue and attack
        }
    }

    void Melee()
    {

        if (Vector3.Distance(target.transform.position, transform.position) < meleeDistance)
        {
            IDamageable other = target.GetComponent<IDamageable>();
            if (other != null && meleeTimer > meleeCooldown)
            {
                if (Random.Range(0f, 1f) < critChance)
                {
                    other.Damage(critMultiplier * attack);
                }
                else
                {
                    other.Damage(attack);
                }
                meleeTimer = 0;
            }
        }
    }

    void Detect()
    {
        //Check if an enemy is visible and attack them
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, detectionDistance, transform.forward, detectionDistance);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].collider.gameObject.GetComponent<IDamageable>();
            if (hits[i].collider.gameObject.tag != tag && other != null)
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
                    //SetState(Attack);
                    if (calls.Peek() != Attack)
                    {
                        calls.Push(Attack);
                    }
                }
            }
        }

        Invoke("Detect", detectionTime);
    }

    void Detect(bool isNotRecursive)
    {
        //Check if an enemy is visible and attack them
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, detectionDistance, transform.forward);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].collider.gameObject.GetComponent<IDamageable>();
            if (hits[i].collider.gameObject.tag != tag && other != null)
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
                    //SetState(Attack);
                    if (calls.Peek() != Attack)
                    {
                        calls.Push(Attack);
                    }
                }
            }
        }
    }

    // void SetState(UnityAction call)
    // {
    //     action.RemoveAllListeners();
    //     action.AddListener(call);
    // }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
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

    public void SetAI(int num, Material mat)
    {
        switch (num)
        {
            case 1:
                tag = "AI1";
                break;
            case 2:
                tag = "AI2";
                break;
            case 3:
                tag = "AI3";
                break;
            case 4:
                tag = "AI4";
                break;
            default:
                break;
        }

        flag.GetComponent<MeshRenderer>().material = mat;
    }

}
