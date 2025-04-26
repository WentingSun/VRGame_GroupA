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
        health = MaxHealth;


        hitCount++;

        if (hitCount >= maxHitsForMultiplier)
        {
            scoreMultiplier = 2f;
        }

        // 调用得分逻辑（留作接口）
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.addScore(1, scoreMultiplier); // 这里的1可以替换为实际得分
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }

        Debug.Log($"ExtraScorePlanet hit! Current hit count: {hitCount}, Score multiplier: {scoreMultiplier}");
    }


    private void Start()
    {
        isActive = true;
        hitCount = 0;
        // StartCoroutine(DeactivateAfterDuration());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isActive = true;
        hitCount = 0;
        StartCoroutine(DeactivateAfterDuration());
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        StartCoroutine(GetDestroy());
        // gameObject.SetActive(false);
        // Destroy(gameObject);
    }
}