using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetSpawner))]
public class PlanetSpawnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 画默认 Inspector 内容
        DrawDefaultInspector();

        // 获取目标对象引用
        PlanetSpawner planetSpawner = (PlanetSpawner)target;

        // 空一行
        EditorGUILayout.Space();

        // 添加测试按钮
        if (GUILayout.Button("Test Planet Take Damage"))
        {
            planetSpawner.CurrentPlanetTakeDamage();
        }

    }
}
