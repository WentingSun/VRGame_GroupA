using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmallBall))]
public class SmallBallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画默认 Inspector 内容
        DrawDefaultInspector();

        // 获取目标对象引用
        SmallBall ball = (SmallBall)target;

        // 空一行
        EditorGUILayout.Space();

        // 添加测试按钮
        if (GUILayout.Button("Test ApplyForce"))
        {
            ball.applyForce();
        }

        if (GUILayout.Button("Test Release"))
        {
            ball.testRelease();
        }

    }
}
