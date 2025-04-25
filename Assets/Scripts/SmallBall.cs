using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float StayTime;

    [SerializeField] float MaxSpeed = 3f;
    [SerializeField] float MinSpeed = 2f;

    [SerializeField] Vector3 velocity;
    [SerializeField] float velocityMagnitude;

    [Header("Game Logic Related")]
    [SerializeField] private int comboNum;
    [SerializeField] private int hitShellNum;
    [SerializeField] private int MaxHitShellNum = 10;
    [SerializeField] private int penetrationNum;

    [SerializeField] private GameObject rippleEffectPrefab;//特效
    [SerializeField] private GameObject scoreTextPrefab;//score

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
        PoolManager.Instance.SmallBallPool.Release(this);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Planet"))
        {
            penetrationNum--;
            if (penetrationNum <= 0)
            {
                gameObject.layer = 0; //default
            }
        }

        // Debug.Log("Trigger! "+ other.gameObject.name);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WorldShell"))
        {
            //HandleHitShell();

            // 获取撞击点
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPos = contact.point;
            Vector3 normal = contact.normal;


            //Debug.DrawRay(hitPos, normal * 0.2f, Color.red, 2f);

            // 生成波纹粒子
            GameObject ripple = Instantiate(rippleEffectPrefab, hitPos + normal * 0.01f, Quaternion.LookRotation(normal));
            Destroy(ripple, 2f);

            HandleHitShell();
        }
        if (collision.gameObject.CompareTag("Planet"))
        {
            HandleCombo();
        }
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
        //ShowScoreText(+1);//测试

        hitShellNum++;
        if (hitShellNum >= MaxHitShellNum)
        {
            ReleaseItself();
        }
    }

    private void HandleCombo()
    {
        comboNum++;
        ShowScoreText(+1);

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

    private void ShowScoreText(int value)
    {
        GameObject obj = Instantiate(scoreTextPrefab, transform.position, Quaternion.identity);
        var effect = obj.GetComponent<ScoreTextEffect>();
        effect.SetText(value.ToString());
    }

}
