using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerationPointMovement))]
public class PlanetGenerationPointMovementEditor : Editor
{



    void OnSceneGUI()
    {
        var ad = (PlanetGenerationPointMovement)target;
        var t = ad.transform;
        Vector3 worldCenter = ad.circleCentre.position;
        Vector3 worldNormal = t.TransformDirection(ad.axisDirection.normalized);
        Vector3 worldEnd = worldCenter + worldNormal * ad.axisLength;

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
            if (localVec.magnitude <= 1)
            {
                ad.axisLength = localVec.magnitude;
            }else{
                ad.axisLength = 1;
            }
            EditorUtility.SetDirty(ad);
        }

    }

}
