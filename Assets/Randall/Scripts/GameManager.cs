using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material[] aiMats;

    public GameObject[] bases;
    public GameObject[] resourceNodes;
    public int serfCost;
    public int knightCost;
    public int startingResources;



    void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    bool IsAnyEnemyLeft()
    {
        for (int i = 0; i < bases.Length; ++i)
        {
            if (bases[i] != null)
            {
                return true;
            }
        }
        return false;
    }

    public Node GetClosestNode(Vector3 position)
    {
        GameObject retval = null;
        for (int i = 0; i < resourceNodes.Length; i++)
        {
            if (retval == null)
            {
                retval = resourceNodes[i];
            }
            else if (Vector3.Distance(resourceNodes[i].transform.position, position)
                < Vector3.Distance(retval.transform.position, position))
            {
                retval = resourceNodes[i];
            }
        }

        return retval.GetComponent<Node>();
    }

    public Node GetClosestAlignedNode(Vector3 position, string t)
    {
        GameObject retval = null;
        for (int i = 0; i < resourceNodes.Length; i++)
        {
            if (retval == null && resourceNodes[i].tag == t)
            {
                retval = resourceNodes[i];
            }
            else if (retval == null && resourceNodes[i].tag != t)
            {
                continue;
            }

            if (Vector3.Distance(resourceNodes[i].transform.position, position)
                < Vector3.Distance(retval.transform.position, position)
                && resourceNodes[i].tag != t)
            {

                retval = resourceNodes[i];
            }
        }

        if (retval == null)
        {
            return null;
        }
        else
        {
            return retval.GetComponent<Node>();
        }
    }

    public Node GetClosestAlignedNodeWorker(Vector3 position, string t)
    {
        GameObject retval = null;
        for (int i = 0; i < resourceNodes.Length; i++)
        {
            Node nod = resourceNodes[i].GetComponent<Node>();
            if (retval == null && resourceNodes[i].tag == t && nod.currentWorkers < nod.maxGatherers)
            {
                retval = resourceNodes[i];
            }
            else if (retval == null && resourceNodes[i].tag != t)
            {
                continue;
            }

            if (Vector3.Distance(resourceNodes[i].transform.position, position)
                < Vector3.Distance(retval.transform.position, position)
                && resourceNodes[i].tag != t
                && nod.currentWorkers <= nod.maxGatherers)
            {

                retval = resourceNodes[i];
            }
        }

        if (retval == null)
        {
            return null;
        }
        else
        {
            return retval.GetComponent<Node>();
        }
    }

    public Node GetClosestNotAlignedNode(Vector3 position, string t)
    {
        GameObject retval = null;
        for (int i = 0; i < resourceNodes.Length; i++)
        {
            if (retval == null && resourceNodes[i].tag != t)
            {
                retval = resourceNodes[i];
            }
            else if (retval == null && resourceNodes[i].tag == t)
            {
                continue;
            }

            if (Vector3.Distance(resourceNodes[i].transform.position, position)
                < Vector3.Distance(retval.transform.position, position)
                && resourceNodes[i].tag != t)
            {

                retval = resourceNodes[i];
            }
        }
        return retval.GetComponent<Node>();
    }

    struct DistanceGameobject
    {
        public float distance;
        public GameObject gameObject;

        public DistanceGameobject(GameObject obj, Vector3 position)
        {
            gameObject = obj;
            distance = Vector3.Distance(gameObject.transform.position, position);
        }
    }

    public List<GameObject> GetSortedNodes(Vector3 position)
    {
        List<DistanceGameobject> nodes = new List<DistanceGameobject>();
        for (int i = 0; i < resourceNodes.Length; i++)
        {
            nodes.Add(new DistanceGameobject(resourceNodes[i], position));
        }

        while (CheckIfSorted(nodes))
        {
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                if (nodes[i].distance > nodes[i + 1].distance)
                {
                    DistanceGameobject temp = nodes[i];
                    nodes[i] = nodes[i + 1];
                    nodes[i + 1] = temp;
                }
            }
        }

        List<GameObject> sortedNodes = new List<GameObject>();
        for (int i = 0; i < nodes.Count; i++)
        {
            sortedNodes.Add(nodes[i].gameObject);
        }

        return sortedNodes;
    }

    bool CheckIfSorted(List<DistanceGameobject> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (!(list[i].distance < list[i + 1].distance))
            {
                return true;
            }
        }
        return false;
    }

    public KingdomDirector GetRandomKingdom(KingdomDirector kingdom)
    {
        List<GameObject> otherBases = new List<GameObject>();
        for (int i = 0; i < bases.Length; i++)
        {
            if (bases[i] != kingdom.gameObject && bases[i].activeInHierarchy == true)
            {
                otherBases.Add(bases[i]);
            }
        }

        KingdomDirector retval = null;
        if (otherBases.Count > 0)
        {
            retval = otherBases[Random.Range(0, otherBases.Count)].GetComponent<KingdomDirector>();
        }
        return retval;

    }

    public KingdomDirector GetKingdomByTag(string t)
    {
        for (int i = 0; i < bases.Length; i++)
        {
            if (bases[i].tag == t)
            {
                return bases[i].GetComponent<KingdomDirector>();
            }
        }

        return null;
    }

    public bool IsWinner(GameObject obj)
    {
        //List<GameObject> otherBases = new List<GameObject>();
        for (int i = 0; i < bases.Length; i++)
        {
            if (bases[i] != obj && bases[i].activeInHierarchy == true)
            {
                return false;
            }
        }

        return true;

    }

    public Material GetAIMaterial(string aiTag)
    {
        if (aiTag == "AI1")
        {
            return aiMats[0];
        }
        else if (aiTag == "AI2")
        {
            return aiMats[1];
        }
        else if (aiTag == "AI3")
        {
            return aiMats[2];
        }
        else if (aiTag == "AI4")
        {
            return aiMats[3];
        }
        return null;
    }

    [Header("Large Explosion")]
    public GameObject largeExplosionPrefab;
    public GameObject largeExplosionPrefabRed;
    public GameObject largeExplosionPrefabBlue;
    public GameObject largeExplosionPrefabGreen;

    public List<GameObject> largeExplosions = new List<GameObject>();
    public int maxLargeExplosions = 100;
    public List<AudioClip> largeExplosionSounds;

    public GameObject GetLargeExplosion(string t)
    {
        //Debug.Log("Getting Large Explosion");
        for (int i = 0; i < largeExplosions.Count; i++)
        {
            if (!largeExplosions[i].activeInHierarchy && largeExplosions[i].tag == t)
            {
                //return gameObject
                GameObject obj = largeExplosions[i];
                obj.SetActive(true);
                obj.GetComponent<RemoveOnParticleEnd>().Setup();
                return obj;
            }
        }

        if (largeExplosions.Count < maxLargeExplosions)
        {
            GameObject obj = null;
            switch (t)
            {
                case "AI1":
                    obj = Instantiate(largeExplosionPrefabRed);
                    break;
                case "AI2":
                    obj = Instantiate(largeExplosionPrefabBlue);
                    break;
                case "AI3":
                    obj = Instantiate(largeExplosionPrefabGreen);
                    break;
                default:
                    obj = Instantiate(largeExplosionPrefab);
                    break;
            }
            largeExplosions.Add(obj);
            return obj;
        }

        return null;
    }

    [Header("Medium Explosion")]
    public GameObject medExplosionPrefab;
    public GameObject medExplosionPrefabRed;
    public GameObject medExplosionPrefabBlue;
    public GameObject medExplosionPrefabGreen;

    public List<GameObject> medExplosions = new List<GameObject>();
    public int maxMedExplosions = 200;
    public List<AudioClip> medExplosionSounds;
    public GameObject GetMedExplosion(string t)
    {
        for (int i = 0; i < medExplosions.Count; i++)
        {
            if (!medExplosions[i].activeInHierarchy && medExplosions[i].tag == t)
            {
                //return gameObject
                GameObject obj = medExplosions[i];
                obj.SetActive(true);
                obj.GetComponent<AudioSource>().clip = medExplosionSounds[Random.Range(0, medExplosionSounds.Count - 1)];
                obj.GetComponent<RemoveOnParticleEnd>().Setup();
                return obj;
            }
        }

        if (medExplosions.Count < maxMedExplosions)
        {
            GameObject obj = null;
            switch (t)
            {
                case "AI1":
                    obj = Instantiate(medExplosionPrefabRed);
                    break;
                case "AI2":
                    obj = Instantiate(medExplosionPrefabBlue);
                    break;
                case "AI3":
                    obj = Instantiate(medExplosionPrefabGreen);
                    break;
                default:
                    obj = Instantiate(medExplosionPrefab);
                    break;
            }
            medExplosions.Add(obj);
            return obj;
        }

        return null;
    }
}
