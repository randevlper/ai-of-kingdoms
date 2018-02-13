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

    [Header("Spawn Settings")]
    public GameObject knightPrefab;
    //public int knightCost;
    public GameObject knightSpawn;
    public int maxKnights;

    public GameObject serfPrefab;
    //public int serfCost;
    public GameObject serfSpawn;
    public int maxSerfs;

    //1 to 1
    public float serfsToKnights;

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
    public List<GameObject> guards;
    public int guardsNum;
    public GameObject deathEffect;

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
        resources = manager.startingResources;
        GetComponentInChildren<MeshRenderer>().material = manager.GetAIMaterial(tag);
    }

    // Update is called once per frame
    void Update()
    {

        FindEmptySlot(null, ref serfs);
        FindEmptySlot(null, ref knights);
        FindEmptySlot(null, ref guards);
        
        if (((float)serfs.Count < (float)knights.Count / serfsToKnights) && serfs.Count < maxSerfs)
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
                if (guards.Count < guardsNum)
                {
                    FindEmptySlot(null, ref guards);
                    guards.Add(knight.gameObject);
                    knight.SetDefenseObjective(gameObject);
                }
                else
                {
                    GameObject baseToAttack = null;
                    if (!manager.IsWinner(gameObject))
                    {
                        //baseToAttack = manager.GetRandomKingdom(this).gameObject;
                        baseToAttack = manager.GetClosestNotAlignedNode(transform.position, tag).gameObject;
                    }

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
    }

    void SpawnKnight()
    {
        CreateUnit(knightPrefab, manager.knightCost, ref knights, knightSpawn);
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
                objAI.SetAI(AINum, manager.GetAIMaterial(tag), this);
            }
            return true;
        }
        return false;
    }

    bool CreateSerf()
    {
        if (resources >= manager.serfCost)
        {
            GameObject obj = Instantiate(serfPrefab, serfSpawn.transform);
            if (!FindEmptySlot(obj, ref serfs))
            {
                serfs.Add(obj);
            }
            resources -= manager.serfCost;

            obj.tag = tag;
            obj.transform.Find("Cylinder").GetComponent<MeshRenderer>().material = manager.GetAIMaterial(tag);
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
        Instantiate(deathEffect, transform.position, Quaternion.identity);
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

    public GameObject Detect()
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
}
