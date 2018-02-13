using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Events;

public class Knight : MonoBehaviour, IDamageable, IHealable, IAI
{
    [Header("Stats")]
    public float maxHP;

    [SerializeField] public float _HP;
    public float attack;
    public float speed;
    public float runningMultiplier;

    [Header("Combat")]
    public float detectionDistance;
    public float detectionTime;
    private float detectionTimer;
    public float meleeCooldown;
    public float meleeDistance;
    private float meleeTimer;
    public float critChance;
    public float critMultiplier;

    public GameObject deathEffect;

    public KingdomDirector kingdom;
    public int AINum;
    //public LayerMask detectionMask;
    public NavMeshAgent navAgent;
    [HideInInspector] private Vector3 destination;

    public LayerMask detectMask;

    //specific thing the knight is attacking
    private GameObject target;

    //THe thing the knight is going to move towards
    private GameObject objective;

    [Header("Color Settings")]
    public Material flagMaterial;
    public GameObject flag;


    private bool needsHealing;
    [Header("Retreat Settings")]
    public float healPercent;
    private bool isRunning;
    public float criticalPercent;

    public bool isDebug = false;

    public enum States
    {
        IDLE, //Waiting for an order from the KingdomDirector
        ATTACK, //Told to ATTACK a point from the KingdomDirector
        ATTACK_THING, //Attack a specific Knight
        DEFEND, //Told to DEFEND a point from the KingdomDirector
        DEFEND_WORKER, //Later
        RETREAT, //Run from everything to nearest AI owned node to heal, 
                 //check what its fighting to see if it will die first
        NULL,
        HEAL // Just use Retreat when no longer fighting
    }

    Stack<States> _state;

    public States currentState
    {
        get
        {
            if (_state == null) { return States.NULL; }
            return _state.Peek();
        }
    }

    [SerializeField] private States state;

    [Header("Random Settings")]
    public float minAttack;
    public float maxAttack;

    public float minSpeed;
    public float maxSpeed;

    public float minHealth;
    public float maxHealth;

    // Use this for initialization
    void Start()
    {
        _HP = maxHP;
        //Invoke("Detect", 0);        
        needsHealing = false;
        _state = new Stack<States>();
        _state.Push(States.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        //action.Invoke();
        state = currentState;
        if (currentState == States.DEFEND || currentState == States.RETREAT)
        {
            navAgent.speed = speed * runningMultiplier;
        }
        else
        {
            navAgent.speed = speed;
        }

        CheckHealth();
        switch (currentState)
        {
            case States.IDLE:
                //navAgent stand still
                //Detect enemies around on a timer
                Idle();
                break;
            case States.ATTACK:
                Attack();
                break;
            case States.ATTACK_THING:
                AttackThing();
                //Needs a target to attack
                break;
            case States.DEFEND:
                Defend();
                break;
            case States.RETREAT:
                Retreat();
                break;
            default:
                break;
        }

        detectionTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;
        //if (kingdom.isEnemyAttacking) { Detect(true); }
    }

    void Idle()
    {
        navAgent.destination = transform.position;
        Detection();
        //Stand still and detect
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

    GameObject Detect()
    {
        GameObject knight = null;
        //Check if an enemy is visible and attack them
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionDistance, detectMask);

        for (int i = 0; i < hits.Length; ++i)
        {
            //Debug.Log(hits[i].collider.name);
            IDamageable other = hits[i].gameObject.GetComponent<IDamageable>();
            if (hits[i].gameObject.tag != tag && other != null)
            {

                if (knight != null)
                {
                    //Attack only closest
                    if (Vector3.Distance(hits[i].transform.position, transform.position)
                        < Vector3.Distance(knight.transform.position, transform.position))
                    {
                        knight = hits[i].gameObject;
                    }
                }
                else
                {
                    knight = hits[i].gameObject;
                    //SetState(Attack);
                    // if (calls.Peek() != Attack && (!isRunning || kingdom.isEnemyAttacking))
                    // {
                    //     calls.Push(Attack);
                    // }
                }
            }
        }
        //Invoke("Detect", detectionTime);
        return knight;
    }

    void Detection()
    {
        if (detectionTimer > detectionTime)
        {
            detectionTimer = 0;
            return;
        }
        GameObject other = Detect();
        if (other != null &&
            (currentState == States.ATTACK ||
             currentState == States.DEFEND ||
             currentState == States.IDLE))
        {
            //Debug.Log("Detected");
            target = other;
            _state.Push(States.ATTACK_THING);
        }
    }

    void OnDrawGizmos()
    {
        if (isDebug)
        {
            Gizmos.color = flagMaterial.color;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        }
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


    //Check health of self to decide wether to heal or not
    void CheckHealth()
    {
        if (_HP / maxHP < healPercent && !needsHealing)
        {
            needsHealing = true;
        }
        else
        {
            needsHealing = false;
        }
        if (_HP / maxHP < criticalPercent && needsHealing)
        {
            isRunning = true;
            if (currentState != States.RETREAT)
            {
                _state.Push(States.RETREAT);
            }
        }
        else
        {
            isRunning = false;
        }
    }


    //TODO: Set false and the Kingdom director can reset it
    //TODO: isDead
    void Death()
    {
        kingdom.knights.Remove(gameObject);
        GameObject explosion = kingdom.manager.GetMedExplosion();
        if (explosion != null)
        {
            explosion.transform.position = transform.position;
        }
        Destroy(gameObject);
    }

    public void Heal(float health)
    {
        _HP += health;
        if (_HP > maxHP)
        {
            _HP = maxHP;
        }
    }

    public bool GetIsHealing()
    {
        return needsHealing;
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


    //Command sent to knight
    //Kingdom or Node
    public void SetAttackObjective(GameObject thing)
    {
        objective = thing;
        _state.Push(States.ATTACK);
    }

    //Move to objective and scan for enemy
    void Attack()
    {
        //Node objScript = objective.GetComponent<Node>();
        if (objective != null)
        {
            if (objective.activeInHierarchy && objective.tag != tag)
            {
                //Debug.Log("Attacking base" + objective.name);
                Detection();
                navAgent.destination = objective.transform.position;
                return;
            }
        }
        _state.Pop();

        //When achomplished go IDLE, Check by tag?
    }

    void AttackThing()
    {
        //Go to thing and attack unless it does not exsist or isDead
        if (needsHealing)
        {
            _state.Pop();
            _state.Push(States.RETREAT);
            return;
        }
        //TODO: Is dead
        target = Detect();
        if (target != null)
        {
            if (target.activeInHierarchy)
            {
                navAgent.destination = target.transform.position;
                Melee();
                return;
            }
        }
        _state.Pop();

    }

    //Patrol objective, wait for orders from node or defend automatically
    public void SetDefenseObjective(GameObject thing)
    {
        _state.Push(States.DEFEND);
        objective = thing;
    }

    void Defend()
    {
        //Debug.Log("Defending");

        if (objective.tag == tag)
        {
            Detection();
            navAgent.destination = objective.transform.position;
        }
        else
        {
            _state.Pop();
        }
    }

    void Retreat()
    {
        if (_HP == maxHP)
        {
            _state.Pop();
        }
        if (!isRunning)
        {
            Detection();
        }
        navAgent.destination = kingdom.transform.position;
    }

}

// void Detect(bool isNotRecursive)
// {
//     CheckHealth();
//     //Check if an enemy is visible and attack them
//     Collider[] hits =
//         Physics.OverlapSphere(transform.position, detectionDistance);

//     for (int i = 0; i < hits.Length; ++i)
//     {
//         //Debug.Log(hits[i].collider.name);
//         IDamageable other = hits[i].gameObject.GetComponent<IDamageable>();
//         if (hits[i].gameObject.tag != tag && other != null)
//         {

//             if (target != null)
//             {
//                 //Attack only closest
//                 if (Vector3.Distance(hits[i].transform.position, transform.position)
//                     < Vector3.Distance(target.transform.position, transform.position))
//                 {
//                     target = hits[i].gameObject;
//                 }
//             }
//             else
//             {
//                 target = hits[i].gameObject;
//                 //SetState(Attack);
//                 if (calls.Peek() != Attack && (!isRunning || kingdom.isEnemyAttacking))
//                 {
//                     calls.Push(Attack);
//                 }
//             }
//         }
//     }
// }

// void SetState(UnityAction call)
// {
//     action.RemoveAllListeners();
//     action.AddListener(call);
// }

// void Move()
// {
//     navAgent.destination = destination;

//     if (isRunning || isHealing)
//     {
//         navAgent.speed = speed * runningMultiplier;
//     }
//     else
//     {
//         navAgent.speed = speed;
//     }

//     if (transform.position == destination)
//     {
//         calls.Pop();
//     }
// }

// public void Move(Vector3 pos)
// {
//     destination = pos;

//     if (calls == null) { return; }
//     if (!(calls.Peek() == Attack) && calls.Peek() != Move)
//     {
//         calls.Push(Move);
//     }
// }

// void Attack()
// {
//     if (target == null)
//     {
//         //SetState(Idle);
//         calls.Pop();
//         Detect(true);
//         return;
//     }
//     else
//     {
//         navAgent.speed = speed;
//         navAgent.destination = target.transform.position;
//         Melee();
//         //Pursue and attack
//     }
// }
