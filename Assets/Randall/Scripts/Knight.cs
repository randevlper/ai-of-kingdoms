using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Knight : MonoBehaviour, IDamageable, IHealable, IAI
{
    [Header("Stats")]
    public float maxHP;

    [SerializeField] private float _HP;
    public float attack;
    public float speed;
    public float runningMultiplier;

    [Header("Combat")]
    public float detectionDistance;
    public float detectionTime;
    public float meleeCooldown;
    public float meleeDistance;
    private float meleeTimer;
    public float critChance;
    public float critMultiplier;


    public KingdomDirector kingdom;
    public int AINum;
    //public LayerMask detectionMask;
    public NavMeshAgent navAgent;
    [HideInInspector] private Vector3 destination;
    public GameObject target;
    UnityEvent action;

    Stack<UnityAction> calls;

    [Header("Color Settings")]
    public Material flagMaterial;
    public GameObject flag;


    private bool isHealing;
    [Header("Retreat Settings")]
    public float healPercent;
    private bool isRunning;
    public float criticalPercent;

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

        isHealing = false;
    }

    // Update is called once per frame
    void Update()
    {
        //action.Invoke();
        calls.Peek().Invoke();
        meleeTimer += Time.deltaTime;
        if (kingdom.isEnemyAttacking) { Detect(true); }
    }

    void Idle()
    {
        navAgent.destination = transform.position;
        //Stand still and detect
    }

    void Move()
    {
        navAgent.destination = destination;

        if (isRunning || isHealing)
        {
            navAgent.speed = speed * runningMultiplier;
        }
        else
        {
            navAgent.speed = speed;
        }

        if (transform.position == destination)
        {
            calls.Pop();
        }
    }

    public void Move(Vector3 pos)
    {
        destination = pos;

        if (calls == null) { return; }
        if (!(calls.Peek() == Attack) && calls.Peek() != Move)
        {
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
            navAgent.speed = speed;
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
        CheckHealth();
        //Check if an enemy is visible and attack them
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionDistance);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].gameObject.GetComponent<IDamageable>();
            if (hits[i].gameObject.tag != tag && other != null)
            {

                if (target != null)
                {
                    //Attack only closest
                    if (Vector3.Distance(hits[i].transform.position, transform.position)
                        < Vector3.Distance(target.transform.position, transform.position))
                    {
                        target = hits[i].gameObject;
                    }
                }
                else
                {
                    target = hits[i].gameObject;
                    //SetState(Attack);
                    if (calls.Peek() != Attack && (!isRunning || kingdom.isEnemyAttacking))
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
        CheckHealth();
        //Check if an enemy is visible and attack them
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionDistance);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].gameObject.GetComponent<IDamageable>();
            if (hits[i].gameObject.tag != tag && other != null)
            {

                if (target != null)
                {
                    //Attack only closest
                    if (Vector3.Distance(hits[i].transform.position, transform.position)
                        < Vector3.Distance(target.transform.position, transform.position))
                    {
                        target = hits[i].gameObject;
                    }
                }
                else
                {
                    target = hits[i].gameObject;
                    //SetState(Attack);
                    if (calls.Peek() != Attack && (!isRunning || kingdom.isEnemyAttacking))
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

    void CheckHealth()
    {
        if (_HP / maxHP < healPercent && !isHealing)
        {
            isHealing = true;
        }
        if (_HP / maxHP < criticalPercent && isHealing)
        {
            isRunning = true;
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
            _HP = maxHP;
            isHealing = false;
            isRunning = false;
        }
    }

    public bool GetIsHealing()
    {
        return isHealing;
    }

    public void SetAI(int num, Material mat, KingdomDirector k)
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
        kingdom = k;
    }

}
