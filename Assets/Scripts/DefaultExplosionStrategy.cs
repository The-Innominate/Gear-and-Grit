using UnityEngine;

// Default explosion behavior – same boom, cleaner code
public class DefaultExplosionStrategy : IExplosionStrategy
{
    public void Explode(
        GameObject target,
        Vector3 explosionOrigin,
        float force,
        float radius,
        float upwardModifier,
        float objectSize,
        GameObject explosionEffect
    )
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(
                force,
                new Vector3(explosionOrigin.x, explosionOrigin.y + (objectSize * 0.5f), explosionOrigin.z),
                radius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

        if (explosionEffect != null)
        {
            Object.Instantiate(explosionEffect, explosionOrigin, Quaternion.identity);
        }
    }
}
