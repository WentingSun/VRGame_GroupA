using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownPlanet : Planet
{
    [SerializeField] private float speedMultiplier = 0.8f;

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        ball.Reflect(collisionNormal);

        ball.AdjustSpeed(speedMultiplier);

        TakeDamage(1);
    }
}
