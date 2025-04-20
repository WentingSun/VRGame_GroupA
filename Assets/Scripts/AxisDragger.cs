using UnityEngine;

public class AxisDragger : MonoBehaviour
{
    // 这是你想要拖拽编辑的局部方向向量（单位向量）
    public Vector3 axisDirection = Vector3.right;
    // 轴的长度
    public float axisLength = 1f;
    public float radius = 1f;
    public int circleSegments = 64;

    // 用 Gizmos 在 Scene 视图里简单地画出轴
    private void OnDrawGizmos()
    {
        // —— 1. 画原来的轴 ——
        Gizmos.color = Color.green;
        Vector3 start = Vector3.zero;
        Vector3 worldDir = transform.TransformDirection(axisDirection.normalized);
        Vector3 end = start + worldDir * axisLength;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);

        // —— 2. 画以世界原点为圆心、worldDir 为法线的圆 ——
        Gizmos.color = Color.yellow;
        DrawCircle(Vector3.zero, worldDir, radius, circleSegments);
    }

    /// <summary>
    /// 在 Scene 视图里，用 Gizmos 把一个圆画成 N 条线段
    /// </summary>
    /// <param name="center">圆心（世界空间）</param>
    /// <param name="normal">平面法线（世界空间）</param>
    /// <param name="radius">半径</param>
    /// <param name="segments">分段数</param>
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

