using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private int health = 3; 

    public void SetHealth(int newHealth)
    {
        health = Mathf.Max(newHealth, 0); 
    }

    public int GetHealth()
    {
        return health;
    }

    // Update is called once per frame
    void Update()
    {
        // 可选：在这里添加星球的其他逻辑
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); 
        }
    }
    public virtual void OnBallCollision(SmallBall ball, Vector3 collisionNormal)
    {
        ball.Reflect(collisionNormal);
        TakeDamage(1);
    }
}
public class normalPlanet : Planet
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialize normal planet properties
    }

    // Update is called once per frame
    void Update()
    {
        // Update normal planet properties
    }
}

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
