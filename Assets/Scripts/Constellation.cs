using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform[] zodiacPrefabs;        // 12个星座对象
    public float orbitRadius = 2f;          // 圆轨道半径
    public float orbitSpeed = 10f;           // 围绕中心旋转速度（度/秒）
    public float floatAmplitude = 0.3f;      // 上下浮动的高度幅度
    public float floatSpeed = 0.5f;            // 上下浮动速度
    public Camera mainCamera;                // 主摄像机引用

    private Vector3[] basePositions;         // 每个星座的初始位置（不含浮动）
    // Start is called before the first frame update
    void Start()
    {
        int count = zodiacPrefabs.Length;
        basePositions = new Vector3[count];

        // 均匀分布在圆上
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2 / count; // 角度
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * orbitRadius;
            zodiacPrefabs[i].position = pos;
            basePositions[i] = pos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < zodiacPrefabs.Length; i++)
        {
            Transform zodiac = zodiacPrefabs[i];

            // 轨道旋转（围绕原点Y轴旋转）
            zodiac.RotateAround(Vector3.zero, Vector3.up, orbitSpeed * Time.deltaTime);

            // 上下浮动（基于时间 + 每个星座错位一点）
            float floatOffset = Mathf.Sin(Time.time * floatSpeed + i) * floatAmplitude;
            Vector3 currentPos = zodiac.position;
            zodiac.position = new Vector3(currentPos.x, floatOffset, currentPos.z);

            // 距离摄像机 → 透明调节
            float distance = Vector3.Distance(zodiac.position, mainCamera.transform.position);
            float alpha = 1f;

            if (distance < 1f)
            {
                // 越接近越透明（0 = 完全透明，fadeStart = 完全不透明）
                alpha = Mathf.Clamp01(Mathf.InverseLerp(0.2f, 1f, distance));
            }
            //float alpha = Mathf.Clamp01(distance / 15f); // 越近越透明（你可调15这个范围）
            SetAlpha(zodiac, alpha);
        }
    }
    // 修改星座材质透明度（要求材质使用支持透明的Shader）
    void SetAlpha(Transform zodiac, float alpha)
    {
        Renderer rend = zodiac.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;

                    // 确保材质开启透明混合模式（建议提前设置）
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
            }
        }
    }
}
