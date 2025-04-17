using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class DualSphereSlingshotInteractor_Handed : MonoBehaviour
{
    [Header("Shell References (pivot at true center)")]
    public Transform soccerShell;   // FootBallWorldShell
    public Transform roomShell;     // RoomBallWorldShell

    [Header("Prefabs")]
    public GameObject virtualBallPrefab;
    public GameObject realBallPrefab;  // 带 Rigidbody

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Settings")]
    public float activationDistance = 0.05f;  // 指尖到表面最大误差
    public float pinchThreshold = 0.03f;  // 食指-拇指阈值
    public float maxPullDistance = 0.30f;
    public float minLaunchForce = 2f;
    public float maxLaunchForce = 10f;
    public float virtualBallDepthRatio = 0.2f;   // 相对于小球半径的内部深度
    public float virtualBallLife = 1f;

    private XRHandSubsystem handSubsystem;

    // 分手状态：索引 0 = left, 1 = right
    private bool[] isPinching = new bool[2];
    private bool[] hasLaunched = new bool[2];
    private float[] pullAmt = new float[2];

    void Start()
    {
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
                        ?.GetLoadedSubsystem<XRHandSubsystem>();

        if (!soccerShell && WorldManager.Instance != null)
            soccerShell = WorldManager.Instance.FootBallWorldShell;
        if (!roomShell && WorldManager.Instance != null)
            roomShell = WorldManager.Instance.RoomBallWorldShell;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;
        if (soccerShell == null || roomShell == null) return;

        Vector3 soccerCenter = soccerShell.position;
        float soccerRadius = soccerShell.localScale.x * 0.5f;
        Vector3 roomCenter = roomShell.position;
        float roomRadius = roomShell.localScale.x * 0.5f;

        // 左右手数组
        XRHand[] hands = new[] { handSubsystem.leftHand, handSubsystem.rightHand };

        for (int i = 0; i < 2; i++)
        {
            var hand = hands[i];
            if (!hand.isTracked)
            {
                // 松手状态重置
                isPinching[i] = false;
                continue;
            }

            // 拇指 & 食指
            var thumb = hand.GetJoint(XRHandJointID.ThumbTip);
            var index = hand.GetJoint(XRHandJointID.IndexTip);
            if (!thumb.TryGetPose(out Pose tPose) ||
                !index.TryGetPose(out Pose iPose))
            {
                isPinching[i] = false;
                continue;
            }

            Vector3 thumbPos = tPose.position;
            Vector3 indexPos = iPose.position;
            float pinchDist = Vector3.Distance(thumbPos, indexPos);

            // 计算表面点
            Vector3 pinchMid = (thumbPos + indexPos) * 0.5f;
            Vector3 offsetDir = (pinchMid - soccerCenter).normalized;
            Vector3 surfacePoint = soccerCenter + offsetDir * soccerRadius;

            // 调试射线 (Scene 视图可见)
            Debug.DrawLine(soccerCenter, surfacePoint, Color.green, 0.1f);
            Debug.DrawLine(roomCenter, roomCenter + offsetDir * roomRadius, Color.blue, 0.1f);

            // 判定贴近表面：任意指尖
            bool thumbNear = Vector3.Distance(thumbPos, surfacePoint) < activationDistance;
            bool indexNear = Vector3.Distance(indexPos, surfacePoint) < activationDistance;
            bool onSurface = thumbNear || indexNear;

            bool currentPinch = (pinchDist < pinchThreshold) && onSurface;

            // 捏合：显示拉线
            if (currentPinch)
            {
                if (!isPinching[i])
                {
                    isPinching[i] = true;
                    hasLaunched[i] = false;
                    if (lineRenderer != null)
                        lineRenderer.enabled = true;
                }
                // 计算拉距
                pullAmt[i] = Mathf.Clamp(Vector3.Dot(pinchMid - surfacePoint, offsetDir),
                                        0f, maxPullDistance);

                // 更新线
                if (lineRenderer != null)
                {
                    Vector3 endPt = surfacePoint + offsetDir * pullAmt[i];
                    lineRenderer.SetPosition(0, surfacePoint);
                    lineRenderer.SetPosition(1, endPt);
                }
                // 一帧只处理一只手
                return;
            }

            // 松手发射
            if (isPinching[i] && !hasLaunched[i])
            {
                hasLaunched[i] = true;
                isPinching[i] = false;
                if (lineRenderer != null)
                    lineRenderer.enabled = false;

                // 虚拟球 (内部)
                if (virtualBallPrefab != null)
                {
                    float depth = soccerRadius * Mathf.Clamp01(virtualBallDepthRatio);
                    Vector3 insideDir = -offsetDir;
                    Vector3 spawnInside = surfacePoint + insideDir * depth;
                    var vball = Instantiate(virtualBallPrefab,
                                             spawnInside,
                                             Quaternion.LookRotation(insideDir));
                    Destroy(vball, virtualBallLife);
                }

                // 实体球 (外部)
                if (realBallPrefab != null)
                {
                    Vector3 spawnRoom = roomCenter + offsetDir * roomRadius;
                    Vector3 flyDir = -offsetDir;
                    float pr = pullAmt[i] / maxPullDistance;
                    float force = Mathf.Lerp(minLaunchForce, maxLaunchForce, pr);

                    var obj = Instantiate(realBallPrefab,
                                          spawnRoom,
                                          Quaternion.LookRotation(flyDir));
                    if (obj.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddForce(flyDir * force, ForceMode.VelocityChange);
                    }
                }
            }

            // 重置
            isPinching[i] = false;
        }
    }
}
