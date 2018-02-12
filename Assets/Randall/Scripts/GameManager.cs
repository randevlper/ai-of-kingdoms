using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] bases;
    public GameObject[] resourceNodes;
    public float serfCost;
    public float knightCost;

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
}
