using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraB : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform bigBall;//获取大球
    [SerializeField] private float Multiplier = 10f;//倍数距离

    private float BigBallRadius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (bigBall != null)
        {
            BigBallRadius = bigBall.localScale.x / 2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            Vector3 direction = (mainCamera.transform.position - Vector3.zero).normalized;//方向
            float distance = BigBallRadius * Multiplier;//距离
            transform.position = direction * distance;
            transform.LookAt(Vector3.zero);
        }
    }
}
