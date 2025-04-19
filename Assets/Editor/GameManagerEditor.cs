using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画默认 Inspector 内容
        DrawDefaultInspector();

        // 获取目标对象引用
        GameManager manager = (GameManager)target;

        // 空一行
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("↓ Switch GameState ↓", EditorStyles.boldLabel);

        // 添加测试按钮
        if (GUILayout.Button("Switch to GameStart"))
        {
           manager.UpdateGameState(GameState.GameStart);
        }

        if (GUILayout.Button("Switch to GamePause"))
        {
           manager.UpdateGameState(GameState.GamePause);
        }

        if (GUILayout.Button("Switch to GameOver"))
        {
           manager.UpdateGameState(GameState.GameOver);
        }

        EditorGUILayout.LabelField("↓ Switch PlayerState ↓", EditorStyles.boldLabel);

        if (GUILayout.Button("Switch to Idel"))
        {
           manager.UpdatePlayerState(PlayerState.Idel);
        }

        if (GUILayout.Button("Switch to Aiming"))
        {
           manager.UpdatePlayerState(PlayerState.Aiming);
        }

        if (GUILayout.Button("Switch to Dead"))
        {
           manager.UpdatePlayerState(PlayerState.Dead);
        }

        if (GUILayout.Button("Switch to ShootingABall"))
        {
           manager.UpdatePlayerState(PlayerState.ShootingABall);
        }
        
        if (GUILayout.Button("Switch to TakingDamage"))
        {
           manager.UpdatePlayerState(PlayerState.TakingDamage);
        }

        EditorGUILayout.LabelField("↓ Send GameEvent ↓", EditorStyles.boldLabel);

        if (GUILayout.Button("Send Null"))
        {
           manager.SendGameEvent(GameEvent.Null);
        }

        if (GUILayout.Button("Send ThreeComboHit"))
        {
           manager.SendGameEvent(GameEvent.ThreeComboHit);
        }

        if (GUILayout.Button("Send TenComboHit"))
        {
           manager.SendGameEvent(GameEvent.TenComboHit);
        }

        if (GUILayout.Button("Send AllBallUsed"))
        {
           manager.SendGameEvent(GameEvent.AllBallUsed);
        }

        if (GUILayout.Button("Send RewardABall"))
        {
           manager.SendGameEvent(GameEvent.RewardABall);
        }

    }
}
