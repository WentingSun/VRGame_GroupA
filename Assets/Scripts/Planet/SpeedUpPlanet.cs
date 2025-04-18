using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpPlanet : Planet
{
    [SerializeField] private float speedMultiplier = 1.5f;

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        ball.Reflect(collisionNormal);

        ball.AdjustSpeed(speedMultiplier);

        TakeDamage(1);
    }
}
