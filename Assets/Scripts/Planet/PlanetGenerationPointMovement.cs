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

    void Start()
    {
        if (circleCentre == null)
        {
            circleCentre = GameObject.Find("FootBallWorldShell").transform;
        }


        var planet = PoolManager.Instance.defaultPlanetPool.Get();
        planet.transform.parent = this.transform;
        planet.transform.position = this.transform.position;
    
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 start;
        if(circleCentre){
            start = circleCentre.position;
        }else{
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
    }




}
