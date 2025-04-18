using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraScorePlanet : Planet
{
    [SerializeField] private float duration = 7f; // 星球保留的时间
    [SerializeField] private int maxHitsForMultiplier = 5; // 达到翻倍得分所需的碰撞次数
    [SerializeField] private float scoreMultiplier = 1f; // 当前得分倍率

    private int hitCount = 0;
    private bool isActive = true;

    public override void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        if (!isActive) return;

        base.OnBallCollision(ball, collisionNormal);

        hitCount++;

        if (hitCount >= maxHitsForMultiplier)
        {
            scoreMultiplier = 2f;
        }

        // 调用得分逻辑（留作接口）
        GrantScore();

        Debug.Log($"ExtraScorePlanet hit! Current hit count: {hitCount}, Score multiplier: {scoreMultiplier}");
    }

    private void GrantScore()
    {
        // TODO: 实现得分逻辑
    }

    private void Start()
    {
        StartCoroutine(DeactivateAfterDuration());
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        isActive = false; 
        Destroy(gameObject);
    }
}