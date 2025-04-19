using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画默认 Inspector 内容
        DrawDefaultInspector();

        // 获取目标对象引用
        PoolManager manager = (PoolManager)target;

        // 空一行
        EditorGUILayout.Space();

        // 添加测试按钮
        if (GUILayout.Button("Get a small ball"))
        {
            manager.testSmallBallPoolGet();
        }

        if (GUILayout.Button("Simulate Player Shooting a ball"))
        {
            if (GameManager.Instance.remainingSmallBallNum > 0)
            {
                manager.testSmallBallPoolGet();
                GameManager.Instance.UpdatePlayerState(PlayerState.ShootingABall);
            }else{
                Debug.Log("No Balls");
            }

        }
    }
}
