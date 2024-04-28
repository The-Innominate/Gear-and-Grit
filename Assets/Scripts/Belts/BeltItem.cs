using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltItem : MonoBehaviour
{
    public GameObject item;

    private void Awake()
    {
        item = gameObject;
    }
}
