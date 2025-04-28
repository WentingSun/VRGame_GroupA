using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitPlanet : Planet
{
    [SerializeField] private int fragmentCount = 4; // 分裂成的碎片数量
    [SerializeField] private float fragmentSpeed = 1.5f; // 碎片的初速度

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        base.OnBallCollision(ball, collisionNormal);

        if (GetHealth() <= 0)
        {
            Split();
        }
    }

    private void Split()
    {
        for (int i = 0; i < fragmentCount; i++)
        {
            SmallBall fragment = PoolManager.Instance.SmallBallPool.Get();
            fragment.SetCollisions(1);
            if (fragment != null)
            {
                fragment.transform.position = transform.position;

                Rigidbody rb = fragment.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDirection = Random.onUnitSphere;
                    rb.velocity = randomDirection * fragmentSpeed;
                }
            }
        }
        StartCoroutine(GetDestroy());
        // Destroy(gameObject);
    }
}