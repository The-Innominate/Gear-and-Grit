using UnityEngine;

public class PickItemDecorator : MonoBehaviour
{
    public PickItem pickItem;  // Reference to the original PickItem
    public bool applyRandomRotation = true;

    void Start()
    {
        if (pickItem == null)
            pickItem = GetComponent<PickItem>();

        if (pickItem != null)
        {
            pickItem.Pick(); // Call the original Pick method
            if (applyRandomRotation && pickItem.lastSpawned != null)
            {
                RandomizeRotation(pickItem.lastSpawned);
            }
        }
    }

    void RandomizeRotation(GameObject obj)
    {
        float randomY = Random.Range(0f, 360f);
        obj.transform.rotation = Quaternion.Euler(0f, randomY, 0f);
    }
}
