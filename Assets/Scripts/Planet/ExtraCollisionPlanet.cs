using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraCollisionPlanet : Planet
{
    [SerializeField] private int extraCollisions = 5;

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        base.OnBallCollision(ball, collisionNormal);

        if (ball != null)
        {
            ball.SetCollisions(extraCollisions+ball.MaxHitShellNum);
        }
    }
}