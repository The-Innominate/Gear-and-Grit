using UnityEngine;

public class PickItem : MonoBehaviour
{
    public GameObject[] itemsToPickFrom;
    public GameObject lastSpawned;  // expose last spawned clone

    public void Pick()
    {
        int randomIndex = Random.Range(0, itemsToPickFrom.Length);
        lastSpawned = Instantiate(itemsToPickFrom[randomIndex], transform.position, Quaternion.identity);
    }
}
