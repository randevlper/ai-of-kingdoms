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
}
