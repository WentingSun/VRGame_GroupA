using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreBallPlanet : Planet
{
    [SerializeField] private int rewardAmount = 3;

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        base.OnBallCollision(ball, collisionNormal);

        GrantReward();
    }

    private void GrantReward()
    {
        // TODO: 实现奖励逻辑，例如增加小球数量

        Debug.Log($"Reward triggered! Granting {rewardAmount} additional balls (logic not implemented yet).");
    }
}