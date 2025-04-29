using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

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

        if (GUILayout.Button("Send GetProtectShell"))
        {
           manager.SendGameEvent(GameEvent.GetProtectShell);
        }

        if (GUILayout.Button("Send GetResurrection"))
        {
           manager.SendGameEvent(GameEvent.GetResurrection);
        }

        if (GUILayout.Button("Send ProtectShellBreak"))
        {
           manager.SendGameEvent(GameEvent.ProtectShellBreak);
        }

        if (GUILayout.Button("Send ResurrectionUsed"))
        {
           manager.SendGameEvent(GameEvent.ResurrectionUsed);
        }

        GUILayout.Space(10);
        GUILayout.Label("音效");
        if (GUILayout.Button("死亡"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Death);
        }
        if (GUILayout.Button("受伤"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Hurt);
        }
        if (GUILayout.Button("命中"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Hit);
        }
        if (GUILayout.Button("三连击"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Hit3);
        }
        if (GUILayout.Button("护盾破坏"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Shield_Break);
        }
        if (GUILayout.Button("小球出现"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Ball_Appear);
        }
        if (GUILayout.Button("小球命中"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Ball_Hit);
        }
        if (GUILayout.Button("小球爆炸"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Ball_Bomb);
        }
        if (GUILayout.Button("复活"))
        {
            AudioManager.Instance.PlayAudio(AudioManager.Recursion);
        }
        
    }
}
