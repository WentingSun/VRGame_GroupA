using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] protected int health = 3;
    [SerializeField] protected int MaxHealth = 3;
    [SerializeField] private PlanetSpawner planetSpawner;
    [SerializeField] private GameObject RealGameobject;
    [SerializeField] private GameObject HitedGameobject;
    [SerializeField] private float FeedbackTime = 0.1f;
    [SerializeField] private SphereCollider sphereCollider;


    private IEnumerator GetHitedFeedback()
    {
        if (RealGameobject == null || HitedGameobject == null)
        {
            yield break;
        }

        RealGameobject.SetActive(false);
        HitedGameobject.SetActive(true);


        yield return new WaitForSeconds(FeedbackTime);

        RealGameobject.SetActive(true);
        HitedGameobject.SetActive(false);
        yield break;
    }

    protected IEnumerator GetDestroy()
    {
        planetSpawner.isSpawner = false;
        planetSpawner.StartSpawnAPlanet();
        RealGameobject.SetActive(false);
        HitedGameobject.SetActive(true);
        sphereCollider.enabled = false;

        yield return new WaitForSeconds(FeedbackTime);

        gameObject.SetActive(false);
        yield break;


    }

    public void StartDestroy()
    {
        StartCoroutine(GetDestroy());
    }

    protected virtual void OnEnable()
    {
        sphereCollider.enabled = true;
        health = MaxHealth;
        RealGameobject.SetActive(true);
        HitedGameobject.SetActive(false);
    }

    void OnDisable()
    {
        GameManager.Instance.destroyPlanetNum++;
    }

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
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
        StartCoroutine(GetHitedFeedback());
        health -= damage;
        if (health <= 0)
        {
            GameManager.Instance.addScore(1, 1);
            StartCoroutine(GetDestroy());
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