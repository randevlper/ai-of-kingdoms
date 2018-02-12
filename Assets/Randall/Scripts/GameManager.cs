using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] bases;
    public GameObject[] resourceNodes;

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
}
