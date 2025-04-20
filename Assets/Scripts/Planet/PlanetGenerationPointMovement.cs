using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerationPointMovement : MonoBehaviour
{
    public Transform circleCentre;
    public Vector3 axisDirection = Vector3.up;
    public float axisLength = 1f;
    public float radius = 1f; //运动半径
    public float moveSpeed = 0.1f;
    public int circleSegments = 64;
    public int numOfSameOrbit;// 轨道上总共有几个

    public int OrbitIndex;

    void Start()
    {
        if (circleCentre == null)
        {
            circleCentre = GameObject.Find("FootBallWorldShell").transform;
        }

    }

    void Update()
    {
        Vector3 center = (circleCentre != null) ? circleCentre.position : Vector3.zero;
        Vector3 worldNormal = transform.TransformDirection(axisDirection.normalized);
        transform.position = MoveAmongCircle(center, worldNormal, radius, numOfSameOrbit, OrbitIndex);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 start;
        if (circleCentre)
        {
            start = circleCentre.position;
        }
        else
        {
            start = Vector3.zero;
        }
        Vector3 worldDir = transform.TransformDirection(axisDirection.normalized);
        Vector3 end = start + worldDir * axisLength;
        Gizmos.DrawLine(start, end);
        DrawCircle(Vector3.zero, worldDir, radius, circleSegments);
    }

    private void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments)
    {
        // 确保法线归一
        normal = normal.normalized;

        // 找到平面上的第一个切向量 u
        Vector3 u = Vector3.Cross(normal, Vector3.up);
        if (u.sqrMagnitude < 0.001f)
            u = Vector3.Cross(normal, Vector3.right);
        u.Normalize();

        // 再用法线叉 u 得到另一个正交切向量 v
        Vector3 v = Vector3.Cross(normal, u).normalized;

        // 画线段
        Vector3 prevPoint = center + u * radius;
        float step = 2 * Mathf.PI / segments;
        for (int i = 1; i <= segments; i++)
        {
            float theta = step * i;
            Vector3 nextPoint = center
                + (u * Mathf.Cos(theta) + v * Mathf.Sin(theta)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        float slotStep = 2 * Mathf.PI / numOfSameOrbit;
        Vector3 prevSlotPoint = center + u * radius;
        for (int i = 1; i <= numOfSameOrbit ; i++)
        {
            float theta = slotStep * i;
            Vector3 nextPoint = center
                + (u * Mathf.Cos(theta) + v * Mathf.Sin(theta)) * radius;
            if (i - 1 == OrbitIndex % numOfSameOrbit)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawSphere(prevSlotPoint, 0.02f);
            prevSlotPoint = nextPoint;
        }
    }

    private Vector3 MoveAmongCircle(Vector3 center, Vector3 normal, float radius, int total, int index)
    {
        // 1. 构造平面基向量 u, v
        normal = normal.normalized;
        Vector3 u = Vector3.Cross(normal, Vector3.up);
        if (u.sqrMagnitude < 0.001f)
            u = Vector3.Cross(normal, Vector3.right);
        u.Normalize();
        Vector3 v = Vector3.Cross(normal, u).normalized;

        // 2. 计算当前角度：初始偏移 + 时间增量
        //    - 每个实例初始偏移： index/total * 2π
        //    - 按 moveSpeed (rad/s) 随时间旋转
        float initPos = index % total;

        float initialAngle = initPos / total * 2f * Mathf.PI;
        float dynamicAngle = moveSpeed * Time.time;
        float theta = initialAngle + dynamicAngle;
        // Debug.Log(theta);

        // 3. 返回圆周上的点
        var result = center + (u * Mathf.Cos(theta) + v * Mathf.Sin(theta)) * radius;
        // Debug.Log(result);
        return result;
    }


}
