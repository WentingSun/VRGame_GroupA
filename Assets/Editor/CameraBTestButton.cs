using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraBMovement))]
public class CameraBMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画默认 Inspector 内容
        DrawDefaultInspector();

        // 获取目标对象引用
        CameraBMovement cameraB = (CameraBMovement)target;

        // 空一行
        EditorGUILayout.Space();

        // 添加测试按钮
        if (GUILayout.Button("更新CameraB位置"))
        {
            cameraB.UpdateCameraBPosition();
        }
    }
}
