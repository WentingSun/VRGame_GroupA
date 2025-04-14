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


    public void applyForce()
    {
        rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
    }

    #region Pool-Related

    public void ReleaseItself()
    {
        onRelease();
        PoolManager.Instance.SmallBallPool.Release(this);
    }

    void OnEnable()
    {
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

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("SmallBall Enter");
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

}
