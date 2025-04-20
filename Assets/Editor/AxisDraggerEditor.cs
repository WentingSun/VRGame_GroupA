using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AxisDragger))]
public class AxisDraggerEditor : Editor
{
    void OnSceneGUI()
    {
        var ad = (AxisDragger)target;
        var t  = ad.transform;

        // —— 1. 计算世界空间的数据 —— //
        Vector3 worldCenter = Vector3.zero;
        // 法线：把本地轴向量变换到世界空间
        Vector3 worldNormal = t.TransformDirection(ad.axisDirection.normalized);
        // 轴末端
        Vector3 worldEnd = worldCenter + worldNormal * ad.axisLength;

        // —— 2. 画圆 —— //
        Handles.color = Color.yellow;
        DrawCircleHandle(worldCenter, worldNormal, ad.radius, ad.circleSegments);

        // —— 3. 画轴线 & 末端球 —— //
        Handles.color = Color.green;
        Handles.DrawLine(worldCenter, worldEnd);
        // 末端小球，让它更醒目
        Handles.SphereHandleCap(0, worldEnd, Quaternion.identity, 0.05f, EventType.Repaint);

        // —— 4. 拖拽更新 —— //
        EditorGUI.BeginChangeCheck();
        Vector3 newEnd = Handles.PositionHandle(worldEnd, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(ad, "Move Axis End");

            // newEnd - worldCenter 就是新的世界向量
            Vector3 worldVec = newEnd - worldCenter;
            // 转回本地向量
            Vector3 localVec = t.InverseTransformVector(worldVec);

            ad.axisDirection = localVec.normalized;
            ad.axisLength    = localVec.magnitude;
            EditorUtility.SetDirty(ad);
        }
    }

    /// <summary>
    /// 在 Scene 视图里，用 Handles 把一个圆画成 N 条线段
    /// </summary>
    private void DrawCircleHandle(Vector3 center, Vector3 normal, float radius, int segments)
    {
        normal.Normalize();
        // 找到平面上的第一个切向量 u
        Vector3 u = Vector3.Cross(normal, Vector3.up);
        if (u.sqrMagnitude < 0.001f)
            u = Vector3.Cross(normal, Vector3.right);
        u.Normalize();
        // 再用法线叉 u 得到另一个正交切向量 v
        Vector3 v = Vector3.Cross(normal, u).normalized;

        float step = 2 * Mathf.PI / segments;
        Vector3 prev = center + u * radius;
        for (int i = 1; i <= segments; i++)
        {
            float theta = step * i;
            Vector3 next = center + (u * Mathf.Cos(theta) + v * Mathf.Sin(theta)) * radius;
            Handles.DrawLine(prev, next);
            prev = next;
        }
    }
}