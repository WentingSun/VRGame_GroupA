using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePlanet : Planet
{
    [SerializeField] private float explosionRadiusMultiplier = 3f;
    [SerializeField] private LayerMask explosionLayerMask;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (GetHealth() <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogWarning("ExplosivePlanet requires a SphereCollider to calculate explosion radius.");
            return;
        }

        float explosionRadius = sphereCollider.radius * explosionRadiusMultiplier;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);

        foreach (Collider collider in colliders)
        {
            Planet planet = collider.GetComponent<Planet>();
            if (planet != null && planet != this)
            {
                Destroy(planet.gameObject);
            }

            SmallBall smallBall = collider.GetComponent<SmallBall>();
            if (smallBall != null)
            {
                Destroy(smallBall.gameObject); 
            }
        }

        Destroy(gameObject);
    }
}