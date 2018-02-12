﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomDirector : MonoBehaviour, IDamageable, IStorage
{

    [Header("Kingdom Setting")]
    public int AINum;
    public float maxHP;
    [HideInInspector] [SerializeField] private float HP;
    [SerializeField] private float resources;
    public int resourceIncrease;

    public Material colorMat;

    [Header("Spawn Settings")]
    public GameObject knightPrefab;
    public int knightCost;
    public GameObject knightSpawn;

    public GameObject serfPrefab;
    public int serfCost;
    public GameObject serfSpawn;
    public int maxSerfs;

    [Header("Other Things")]
    public GameManager manager;
    //[SerializeField] private GameObject[] enemyBases;
    //[SerializeField] private List<GameObject> nodes;

    [Header("Spawned Things")]
    [SerializeField]
    public List<GameObject> knights;
    [SerializeField] private List<GameObject> serfs;

    public GameObject attackPos;

    [Header("Heal Settings")]
    public float healthPerSecond;

    [Header("Enemy Detection Settings")]
    public float detectionDistance;
    public float detectionDelay;
    public bool isEnemyAttacking;

    public bool isDebug = false;

    enum Personalities
    {
        DEFENSIVE,
        AGGRESSIVE
    }

    // Use this for initialization
    void Start()
    {
        AddResource();
        HP = maxHP;
        switch (AINum)
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
        CheckForEnemy();

        Invoke("SpawnKnight", 1f);
    }

    // Update is called once per frame
    void Update()
    {

        if (serfs.Count < maxSerfs)
        {
            CreateSerf();
        }

        SpawnKnight();

        //TODO: Decide between serf or Knight to create
        //DecideTarget();
        for (int i = 0; i < knights.Count; i++)
        {
            //TODO: Don't get components every frame]
            Knight knight = knights[i].GetComponent<Knight>();
            if (knight.currentState == Knight.States.IDLE)
            {
                GameObject baseToAttack = manager.GetRandomKingdom(this).gameObject;
                // for (int b = 0; b < manager.bases.Length; b++)
                // {
                //     if (manager.bases[b] != gameObject && manager.bases[b].activeInHierarchy)
                //     {
                //         baseToAttack = manager.bases[b];
                //     }
                // }
                if (baseToAttack != null)
                {
                    knight.SetAttackObjective(baseToAttack);
                }
                else
                {
                    Debug.Log("DEFEND ME");
                    knight.SetDefenseObjective(gameObject);
                }
            }
        }
    }

    void SpawnKnight()
    {
        CreateUnit(knightPrefab, knightCost, ref knights, knightSpawn);
    }

    void DecideTarget()
    {
    }

    void AddResource()
    {
        resources += resourceIncrease;
        Invoke("AddResource", 1f);
    }

    bool CreateUnit(GameObject prefab, int cost, ref List<GameObject> store, GameObject spawn)
    {
        if (resources >= cost)
        {
            GameObject obj = Instantiate(prefab, spawn.transform);

            if (!FindEmptySlot(obj, ref store))
            {
                store.Add(obj);
            }
            resources -= cost;

            IAI objAI = obj.GetComponent<IAI>();
            if (objAI != null)
            {
                objAI.SetAI(AINum, colorMat, this);
            }
            return true;
        }
        return false;
    }

    bool CreateSerf()
    {
        if (resources >= serfCost)
        {
            GameObject obj = Instantiate(serfPrefab, serfSpawn.transform);
            if (!FindEmptySlot(obj, ref serfs))
            {
                serfs.Add(obj);
            }
            resources -= serfCost;

            obj.tag = tag;
            obj.transform.Find("Cylinder").GetComponent<MeshRenderer>().material = colorMat;
            obj.transform.position = serfSpawn.transform.position;
            GetResource objScript = obj.GetComponent<GetResource>();
            objScript.node = manager.GetClosestNode(transform.position).transform;
            objScript.dropOff = gameObject.transform;

            return true;
        }
        return false;
    }

    bool FindEmptySlot(GameObject obj, ref List<GameObject> store)
    {
        for (int i = 0; i < store.Count; ++i)
        {
            if (store[i] == null)
            {
                store[i] = obj;
                return true;
            }
        }
        return false;
    }

    public void Damage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public void Insert(float num)
    {
        resources += num;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == tag)
        {
            IHealable otherHealth = other.GetComponent<IHealable>();
            if (otherHealth != null)
            {
                otherHealth.Heal(healthPerSecond * Time.deltaTime);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (isDebug)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        }
    }

    void CheckForEnemy()
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
                    //Debug.Log("Detected Enemy!");
                    isEnemyAttacking = true;
                    break;
                }
            }
            isEnemyAttacking = false;
        }

        Invoke("CheckForEnemy", detectionDelay);
    }
}
