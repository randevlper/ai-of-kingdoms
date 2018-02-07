using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomDirector : MonoBehaviour, IDamageable, IStorage
{
    public int AINum;
    public float maxHP;
    [HideInInspector] [SerializeField] private float HP;
    [SerializeField] private int resources;
    public int resourceIncrease;

    public Material colorMat;

    public GameObject knightPrefab;
    public int knightCost;
    public GameObject knightSpawn;

    public GameObject serfPrefab;
    public int serfCost;
    public GameObject serfSpawn;

    [SerializeField] private GameObject[] enemyBases;
    [SerializeField] private List<GameObject> nodes;
    [SerializeField] private List<GameObject> knights;
    [SerializeField] private List<GameObject> serfs;

    public GameObject attackPos;

    public int maxSerfs;
    public float healthPerSecond;
    public float detectionDistance;
    public float detectionDelay;
    public bool isEnemyAttacking;

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
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Decide between serf or Knigh

        if (serfs.Count < maxSerfs)
        {
            CreateSerf();
        }
        else
        {
            SpawnKnight();
        }

        DecideTarget();
        if (attackPos != null)
        {
            for (int i = 0; i < knights.Count; i++)
            {
                if (knights[i] != null)
                {
                    IHealable health = knights[i].GetComponent<IHealable>();
                    Knight knight = knights[i].GetComponent<Knight>();

                    if (health.GetIsHealing())
                    {
                        knight.Move(transform.position);
                    }
                    //if (attackPos.transform.position != Vector3.zero)
                    else if (IsAnyEnemyLeft())
                    {
                        knight.Move(attackPos.transform.position);
                    }
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
        if (enemyBases.Length > 0)
        {
            GameObject closestEnemy = null;
            for (int i = 0; i < enemyBases.Length; ++i)
            {
                if (enemyBases[i] == null) { continue; }
                if (closestEnemy == null)
                {
                    closestEnemy = enemyBases[i];
                }
                else
                {
                    if (Vector3.Distance(enemyBases[i].transform.position, transform.position) <
                        Vector3.Distance(closestEnemy.transform.position, transform.position))
                    {
                        closestEnemy = enemyBases[i];
                    }
                }
            }
            attackPos = closestEnemy;
        }
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
            GetResource objScript = obj.GetComponent<GetResource>();
            objScript.node = nodes[0].transform;
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
        Destroy(gameObject);
    }

    public void Insert(int num)
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
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

    bool IsAnyEnemyLeft()
    {
        for (int i = 0; i < enemyBases.Length; ++i)
        {
            if(enemyBases[i] != null)
            {
                return true;
            }
        }
        return false;
    }
}
