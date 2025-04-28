using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Audio Request 还需要小球出现音效和动画特效, 发射音效, 连击音效, 爆炸音效
public class SmallBall : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Material OriginMaterial;
    [SerializeField] private Material FinalMaterial;
    [SerializeField] Rigidbody rb;
    [SerializeField] float StayTime;

    [SerializeField] float MaxSpeed = 3f;
    [SerializeField] float MinSpeed = 2f;

    [Header("Small Ball Rigidbody Information")]

    [SerializeField] Vector3 velocity;
    [SerializeField] float velocityMagnitude;
    public float bounceStrength = 1.2f;
    [Header("Game Logic Related")]
    [SerializeField] private int comboNum;
    [SerializeField] private int hitShellNum;
    [SerializeField] public int MaxHitShellNum = 10;
    [SerializeField] private int penetrationNum;
    [SerializeField] private bool isHarmful = false;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip appearAudio;//出现音效 备注音频文件名字

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.OnGameStateChange += GameStateChange;
    }

    private void GameStateChange(GameState state)
    {
        if (state == GameState.GameOver)
        {
            ReleaseItself(); // 当游戏结束时, 禁用自己
        }
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;
        velocityMagnitude = velocity.magnitude;
    }

    private void Initialise()
    {
        //gameObject.layer = 0;
        penetrationNum = 0;
        hitShellNum = 0;
        comboNum = 0;
        meshRenderer.material = OriginMaterial;
        isHarmful = false;
    }


    public void applyForce()
    {
        rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
    }

    public void VelocityChange(Vector3 Direction, float Magnitude)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Direction * Magnitude, ForceMode.VelocityChange);
    }

    #region Pool-Related

    public void ReleaseItself()
    {
        onRelease();
        if (gameObject.activeSelf)
        {
            PoolManager.Instance.SmallBallPool.Release(this);
        }

    }

    void OnEnable()
    {
        Initialise();
        GameManager.OnGameStateChange += GameStateChange;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChange -= GameStateChange;
    }

    public void onRelease()
    {

    }

    #endregion

    #region Collision Logic

    public void SetCollisions(int count)
    {
        MaxHitShellNum = count;
        Debug.Log($"The ball's collision count increased by {count}. Current total: {MaxHitShellNum}");
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter");
        if (other.gameObject.CompareTag("Planet"))
        {
            penetrationNum--;
            if (penetrationNum <= 0)
            {
                gameObject.layer = 0; //default
            }
        }

        if (other.gameObject.CompareTag("Player") && isHarmful) 
        { 
            Debug.Log("PlayerState.GetHit");
            GameManager.Instance.UpdatePlayerState(PlayerState.GetHit);
            this.ReleaseItself();
        }

        // Debug.Log("Trigger! "+ other.gameObject.name);
    }

    void OnCollisionEnter(Collision collision)
    {


        // Debug.Log($"SmallBall 碰到：{collision.gameObject.name} (layer={collision.gameObject.layer})");
        if (collision.gameObject.CompareTag("WorldShell"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Ball_Hit);
            HandleHitShell();
        }
        if (collision.gameObject.CompareTag("Planet"))
        {
            Planet planet = collision.gameObject.GetComponent<Planet>();
            if (planet != null)
            {
                Vector3 normal = collision.contacts[0].normal;
                planet.OnBallCollision(this, normal);
            }

            HandleCombo();
        }



        int handLayer = LayerMask.NameToLayer("PlayerHand");
        if (collision.gameObject.layer == handLayer)
        {
            Vector3 n = collision.contacts[0].normal;
            rb.velocity = Vector3.Reflect(rb.velocity, n) * 1.2f;

        }

    }


    public void AdjustSpeed(float multiplier)
    {
        velocityMagnitude *= multiplier;
        rb.velocity = rb.velocity.normalized * velocityMagnitude;
    }

    public void Reflect(Vector3 normal)
    {
        rb.velocity = Vector3.Reflect(rb.velocity, normal);
        rb.velocity = rb.velocity.normalized * velocityMagnitude;
    }



    void OnCollisionStay(Collision collision)
    {
        // Debug.Log("CollisionStay");
        if (collision.gameObject.CompareTag("WorldShell"))
        {
            StayTime += Time.deltaTime;
        }

        if (StayTime > 0.1f)
        {
            rb.velocity = Vector3.zero;
            if (WorldManager.Instance.RandomMoveTarget != null)
            {
                rb.AddForce((WorldManager.Instance.RandomMoveTarget.position - transform.position).normalized * MaxSpeed, ForceMode.VelocityChange);
            }
            else
            {
                rb.AddForce((-transform.position).normalized * MaxSpeed, ForceMode.VelocityChange);
            }

        }


    }

    void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("WorldShell"))
        {
            StayTime = 0;
        }

        if (rb.velocity.magnitude > MaxSpeed)
        {

            rb.velocity = rb.velocity.normalized * MaxSpeed;

        }

        if (rb.velocity.magnitude < MinSpeed)
        {

            rb.velocity = rb.velocity.normalized * MaxSpeed;

        }
    }

    #endregion

    private void HandleHitShell()
    {
        comboNum = 0;

        hitShellNum++;

        if (hitShellNum == MaxHitShellNum - 1)
        {
            isHarmful = true;
            meshRenderer.material = FinalMaterial;
        }

        if (hitShellNum >= MaxHitShellNum)
        {
            ReleaseItself();
        }
    }

    private void HandleCombo()
    {
        comboNum++;

        if (comboNum == 3)
        {
            GameManager.Instance.SendGameEvent(GameEvent.ThreeComboHit);
        }

        if (comboNum == 10)
        {
            GameManager.Instance.SendGameEvent(GameEvent.TenComboHit);
        }
    }

    public void SetPenetration(int Num)
    {
        penetrationNum = Num;
        gameObject.layer = 8; // PenetrationSmallBall
    }

}
