using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreBallPlanet : Planet
{
    [SerializeField] private int rewardAmount = 3; // 奖励的小球数量

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        base.OnBallCollision(ball, collisionNormal);

        GrantReward();
    }

    private void GrantReward()
    {
        for (int i = 0; i < rewardAmount; i++)
        {
            GameManager.Instance.SendGameEvent(GameEvent.RewardABall);
        }
    }
}