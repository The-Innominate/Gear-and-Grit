using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles blowing stuff up when the player hits it.
// Now uses a strategy pattern so we can swap out how things explode if needed.
public class Explode : MonoBehaviour
{
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionUpward;
    [SerializeField] float objectSize;
    [SerializeField] GameObject explosionPrefab;

    // Strategy that actually does the explosion logic
    private IExplosionStrategy explosionStrategy;

    private void Awake()
    {
        // Assign a default explosion behavior (can swap this out later if we want variety)
        explosionStrategy = new DefaultExplosionStrategy();
    }

    // When this object smacks into the player, do the boom thing
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Let the strategy handle the details of the explosion
            explosionStrategy.Explode(
                collision.gameObject,
                transform.position,
                explosionForce,
                explosionRadius,
                explosionUpward,
                objectSize,
                explosionPrefab
            );

            // Delete this object after it explodes
            Destroy(gameObject);
        }
    }
}
