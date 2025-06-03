using UnityEngine;

public interface IExplosionStrategy
{
    void Explode(
        GameObject target,
        Vector3 explosionOrigin,
        float force,
        float radius,
        float upwardModifier,
        float objectSize,
        GameObject explosionEffect
    );
}
