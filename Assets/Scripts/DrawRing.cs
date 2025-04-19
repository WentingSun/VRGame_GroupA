using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DynamicPulseRing : MonoBehaviour
{
    [Header("足球壳 (World Center)")]
    [Tooltip("Drag the soccer shell (FootBallWorldShell) Transform here.")]
    public Transform soccerShell;

    [Header("Ring Geometry")]
    [Tooltip("Radius offset outside soccer shell surface.")]
    public float offset = 0.02f;
    [Tooltip("Number of segments (higher = smoother).")]
    public int segments = 64;
    [Tooltip("LineRenderer start/end width.")]
    public float ringWidth = 0.01f;

    [Header("Dynamic Effects")]
    [Tooltip("Rotation speed (degrees/sec)")]
    public float rotateSpeed = 30f;
    [Tooltip("Seconds between axis randomizations.")]
    public float axisChangeInterval = 3f;

    [Header("Pulse Emission")]
    [Tooltip("Pulse speed (oscillations/sec)")]
    public float pulseSpeed = 1f;
    [Tooltip("Min alpha in pulse")]
    public float minAlpha = 0.2f;
    [Tooltip("Max alpha in pulse")]
    public float maxAlpha = 1f;

    private LineRenderer lr;
    private Material mat;
    private Color baseColor;
    private List<Vector3> baseDirs;
    private Quaternion ringRotation = Quaternion.identity;

    private Vector3 currentAxis;
    private Vector3 targetAxis;
    private float axisTimer;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = segments;
        lr.startWidth = ringWidth;
        lr.endWidth = ringWidth;

        mat = lr.material;
        baseColor = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;

        // Precompute unit circle dirs
        baseDirs = new List<Vector3>(segments);
        for (int i = 0; i < segments; i++)
        {
            float ang = 2f * Mathf.PI * i / segments;
            baseDirs.Add(new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)));
        }

        // Init axes
        currentAxis = Random.onUnitSphere;
        targetAxis = Random.onUnitSphere;
        axisTimer = axisChangeInterval;
    }

    void Update()
    {
        if (soccerShell == null) return;

        // Update axis interpolation
        axisTimer += Time.deltaTime;
        if (axisTimer >= axisChangeInterval)
        {
            axisTimer -= axisChangeInterval;
            currentAxis = targetAxis;
            targetAxis = Random.onUnitSphere;
        }
        float t = axisTimer / axisChangeInterval;
        Vector3 axis = Vector3.Slerp(currentAxis, targetAxis, t).normalized;

        // Accumulate ring rotation
        ringRotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, axis) * ringRotation;

        // Compute world positions for each segment
        Vector3 center = soccerShell.position;
        float radius = soccerShell.localScale.x * 0.5f + offset;
        for (int i = 0; i < segments; i++)
        {
            Vector3 dir = ringRotation * baseDirs[i];
            lr.SetPosition(i, center + dir * radius);
        }

        // Pulse alpha
        float pulse = (Mathf.Sin(Time.time * pulseSpeed * 2f * Mathf.PI) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);
        if (mat.HasProperty("_Color"))
        {
            Color c = baseColor;
            c.a = alpha;
            mat.SetColor("_Color", c);
        }
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", baseColor * alpha);
        }
    }
}
