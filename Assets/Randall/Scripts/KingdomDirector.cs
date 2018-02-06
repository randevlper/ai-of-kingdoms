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
    }

    // Update is called once per frame
    void Update()
    {
        CreateUnit(knightPrefab, knightCost, ref knights, knightSpawn);

        DecideTarget();

        if (attackPos != null)
        {
            for (int i = 0; i < knights.Count; i++)
            {
                if (knights[i] != null)
                {
                    knights[i].GetComponent<Knight>().Move(attackPos.transform.position);
                }
            }
        }
    }

    void DecideTarget()
    {
        if (enemyBases.Length > 0)
        {
            GameObject closestEnemy = null;
            for (int i = 0; i < enemyBases.Length; ++i)
            {
				if(enemyBases[i] == null) {continue;}
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
                objAI.SetAI(AINum, colorMat);
            }
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
}
