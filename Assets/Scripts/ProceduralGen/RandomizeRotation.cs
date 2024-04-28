using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    private void Start()
    {
        RandomizeYRotation();
    }

    void RandomizeTheRotation()
    {
        //Random rotation
        float randomX = Random.Range(0, 360);
        float randomY = Random.Range(0, 360);
        float randomZ = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);
    }

    void RandomizeYRotation()
    {
        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.rotation = randomYRotation;
    }
}
