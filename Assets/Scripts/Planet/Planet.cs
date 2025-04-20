using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private int MaxHealth = 3;
    [SerializeField] private PlanetSpawner planetSpawner;

    void OnEnable()
    {
        health = MaxHealth;
    }

    public void SetPlanetSpawner(PlanetSpawner Spawner)
    {
        planetSpawner = Spawner;
    }

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

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            planetSpawner.isSpawner = false;
            planetSpawner.StartSpawnAPlanet();
            gameObject.SetActive(false);
            // Destroy(gameObject);// Wenting:这里是不是要改成SetActive(false)?
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