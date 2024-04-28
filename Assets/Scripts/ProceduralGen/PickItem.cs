using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItem : MonoBehaviour
{
    public GameObject[] itemsToPickFrom;

    void Start()
    {
        Pick();
    }

    void Pick()
    {
        int randomIndex = Random.Range(0, itemsToPickFrom.Length);
        //quaternion.identity means no rotation
        GameObject clone = Instantiate(itemsToPickFrom[randomIndex], transform.position, Quaternion.identity);
    }
}
