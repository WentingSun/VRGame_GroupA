using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DottedLineRenderer : MonoBehaviour
{
    [Header("Dot Settings")]
    [Tooltip("Prefab for each dot (e.g. small sphere)")]
    public GameObject dotPrefab;
    [Tooltip("Spacing between dots in world units")]
    public float spacing = 0.05f;
    [Tooltip("Maximum number of dots to render")]
    public int maxDots = 50;

    [Header("Size Settings")]
    [Tooltip("Size of the first (largest) dot")]
    public float maxDotSize = 0.1f;
    [Tooltip("Base size of the last (smallest) dot when no pull")]
    public float minDotSize = 0.02f;
    [Range(0f, 1f)]
    [Tooltip("Normalized pull amount (0 = no pull, 1 = max pull)")]
    public float normalizedPull = 0f;

    [Header("Anchors")]
    [Tooltip("Start point of the dotted line")]
    public Transform startPoint;
    [Tooltip("End point of the dotted line")]
    public Transform endPoint;

    private List<GameObject> dots = new List<GameObject>();

    void Update()
    {
        if (dotPrefab == null || startPoint == null || endPoint == null)
        {
            ClearDots();
            return;
        }

        Vector3 A = startPoint.position;
        Vector3 B = endPoint.position;
        float totalDist = Vector3.Distance(A, B);
        int needed = Mathf.Clamp(Mathf.FloorToInt(totalDist / spacing), 0, maxDots);

        // Ensure pool size
        while (dots.Count < needed)
        {
            var go = Instantiate(dotPrefab, transform);
            go.hideFlags = HideFlags.DontSave;
            dots.Add(go);
        }
        while (dots.Count > needed)
        {
            DestroyImmediate(dots[dots.Count - 1]);
            dots.RemoveAt(dots.Count - 1);
        }

        Vector3 dir = (B - A).normalized;

        // 动态计算末端球最小尺寸：随 normalizedPull 增大而减小
        float dynamicMinSize = minDotSize * (1f - normalizedPull);
        dynamicMinSize = Mathf.Clamp(dynamicMinSize, 0f, minDotSize);

        for (int i = 0; i < dots.Count; i++)
        {
            float dist = (i + 1) * spacing;
            if (dist > totalDist)
            {
                dots[i].SetActive(false);
                continue;
            }
            Vector3 pos = A + dir * dist;
            var dot = dots[i];
            dot.SetActive(true);
            dot.transform.position = pos;
            dot.transform.rotation = Quaternion.LookRotation(dir);

            // 大小渐变：第一个为 maxDotSize，最后一个为 dynamicMinSize
            float t = (dots.Count == 1) ? 0f : i / (float)(dots.Count - 1);
            float size = Mathf.Lerp(maxDotSize, dynamicMinSize, t);
            dot.transform.localScale = Vector3.one * size;
        }
    }

    /// <summary>
    /// Removes all instantiated dots.
    /// </summary>
    public void ClearDots()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] != null)
                DestroyImmediate(dots[i]);
        }
        dots.Clear();
    }

    void OnDisable()
    {
        ClearDots();
    }

    void OnDestroy()
    {
        ClearDots();
    }
}
