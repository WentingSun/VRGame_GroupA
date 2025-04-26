using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreTextEffect : MonoBehaviour
{
    public float appearDuration = 0.2f;//放大时间
    public float floatDuration = 1f;//上升时间
    public float finalScale = 0.1f;//大小
    public float floatHeight = 0.5f;//高度

    private Vector3 startScale = Vector3.zero;
    private Vector3 targetScale;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private TextMeshPro textMesh;
    private Color originalColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;

        startPos = transform.position;
        endPos = startPos + Vector3.up * floatHeight;
        targetScale = Vector3.one * finalScale;
        transform.localScale = startScale;

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;

        //放大
        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;

            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180f, 0);
            yield return null;
        }

        transform.localScale = targetScale;
        elapsed = 0f;

        //上升
        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
            elapsed += Time.deltaTime;

            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180f, 0);
            yield return null;
        }

    Destroy(gameObject);
    }

    public void SetText(string value)
    {
        textMesh.text = value;
    }
}
