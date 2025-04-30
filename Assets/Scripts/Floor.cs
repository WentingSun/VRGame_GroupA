using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float solidAngle = 30f; // 不透明起始角度
    [SerializeField] private float transparentAngle = 90f; // 完全透明角度
    [Range(0f,1f)]
    [SerializeField] private float alphaOffset;

    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null || rend == null) return;

        // 摄像机观察方向（反向 forward）
        Vector3 viewDir = -mainCamera.transform.forward.normalized;

        // 和 Vector3.up 的夹角（0 ~ 180）
        float angle = Vector3.Angle(viewDir, Vector3.up);

        // 计算 alpha（线性插值）
        float alpha = 1f;
        if (angle > solidAngle)
        {
            alpha = Mathf.Clamp01(Mathf.InverseLerp(transparentAngle, solidAngle, angle));
        }

        SetAlpha(Mathf.Max(0,alpha-alphaOffset));
    }
    void SetAlpha(float alpha)
    {
        foreach (var mat in rend.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;

                // 确保透明渲染设置
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
