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
        // 调用 GameManager 的方法增加小球数量
        GameManager.Instance.addSmallBallNum(rewardAmount);

        // 触发奖励事件（可选）
        GameManager.Instance.SendGameEvent(GameEvent.RewardABall);

        Debug.Log($"Reward triggered! Granted {rewardAmount} additional balls.");
    }
}