using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandSlingshotInteractor : MonoBehaviour
{
    [Header("World References (from WorldManager)")]
    [Tooltip("足球(小世界) – 如果不手动赋值，会自动从WorldManager获取 FootBallWorld")]
    public Transform soccerBall;      // 足球（小球形世界）
    [Tooltip("房间球(大世界) – 如果不手动赋值，会自动从WorldManager获取 RoomBallWorld")]
    public Transform roomBall;        // 房间球（大球形世界）

    [Header("Prefabs / Pools")]
    [Tooltip("用于在足球内部短暂出现的虚拟小球，若无需求可留空")]
    public GameObject virtualBallPrefab;
    [Tooltip("若使用对象池，可以不指定Prefab，统一从SmallBallPool获取")]
    public GameObject realBallPrefab; // 可留空，如果你只想走对象池

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Settings")]
    [Tooltip("允许在足球表面激活的距离范围（贴近表面）")]
    public float activationDistance = 0.03f;
    [Tooltip("食指拇指捏合阈值")]
    public float pinchThreshold = 0.03f;
    [Tooltip("拉线最大距离")]
    public float maxPullDistance = 0.3f;
    [Tooltip("发射力度")]
    public float launchForce = 10f;
    [Tooltip("虚拟小球多长时间后消失")]
    public float vanishDelay = 1f;
    [Tooltip("是否生成虚拟小球")]
    public bool spawnVirtualBall = true;
    [Tooltip("是否使用对象池生成真实小球")]
    public bool usePoolForRealBall = true;

    // XR Hands Subsystem
    private XRHandSubsystem handSubsystem;

    // 交互状态
    private bool isPinching = false;
    private bool hasLaunched = false;
    private Vector3 pinchStartPosition; // 捏合点

    void Start()
    {
        // 若没有显式指定球体引用，则从 WorldManager 里找
        if (!soccerBall && WorldManager.Instance && WorldManager.Instance.FootBallWorld)
        {
            soccerBall = WorldManager.Instance.FootBallWorld;
        }
        if (!roomBall && WorldManager.Instance && WorldManager.Instance.RoomBallWorld)
        {
            roomBall = WorldManager.Instance.RoomBallWorld;
        }

        // XRHands 初始化
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem is missing! 请确认已启用 XR Hands 插件");
        }

        // 如果没指定LineRenderer，尝试在自身获取
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer) lineRenderer.enabled = false;
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;

        // 遍历左右手
        XRHand[] hands = { handSubsystem.leftHand, handSubsystem.rightHand };
        foreach (var hand in hands)
        {
            if (!hand.isTracked) continue;

            // 获取手指关键关节
            XRHandJoint thumb = hand.GetJoint(XRHandJointID.ThumbTip);
            XRHandJoint index = hand.GetJoint(XRHandJointID.IndexTip);

            if (!thumb.TryGetPose(out Pose thumbPose) || !index.TryGetPose(out Pose indexPose))
                continue;

            Vector3 thumbPos = thumbPose.position;
            Vector3 indexPos = indexPose.position;
            float pinchDist = Vector3.Distance(indexPos, thumbPos);

            // 需要确定玩家贴近“足球表面”
            // 计算捏合中点到足球中心的距离 vs. 足球半径
            if (!soccerBall) return; // 无法检测
            float soccerRadius = soccerBall.localScale.x * 0.5f;
            Vector3 pinchMid = (thumbPos + indexPos) * 0.5f;
            float distToSoccerCenter = Vector3.Distance(pinchMid, soccerBall.position);
            bool onSoccerSurface = Mathf.Abs(distToSoccerCenter - soccerRadius) < activationDistance;

            // 判定捏合
            bool currentPinching = (pinchDist < pinchThreshold) && onSoccerSurface;

            if (currentPinching)
            {
                // 刚开始捏合
                if (!isPinching)
                {
                    isPinching = true;
                    hasLaunched = false;
                    if (lineRenderer) lineRenderer.enabled = true;

                    pinchStartPosition = pinchMid; // 记录捏合点
                }

                // 可视化拉线: 从捏合点向球体中心反向
                if (lineRenderer)
                {
                    Vector3 dirToCenter = (soccerBall.position - pinchStartPosition).normalized;
                    Vector3 pullEnd = pinchStartPosition - dirToCenter * maxPullDistance;
                    lineRenderer.SetPosition(0, pinchStartPosition);
                    lineRenderer.SetPosition(1, pullEnd);
                }

                // 一次只处理一只手
                return;
            }
            else
            {
                // 如果之前在捏合，现在松手，执行发射
                if (isPinching && !hasLaunched)
                {
                    hasLaunched = true;
                    if (lineRenderer) lineRenderer.enabled = false;

                    // 1) 在足球内部制造一个“虚拟小球”（可选）
                    if (spawnVirtualBall && virtualBallPrefab)
                    {
                        Vector3 insideDir = (soccerBall.position - pinchStartPosition).normalized;
                        var virtualBall = Instantiate(
                            virtualBallPrefab,
                            pinchStartPosition,
                            Quaternion.LookRotation(insideDir)
                        );
                        Destroy(virtualBall, vanishDelay);
                    }

                    // 2) 计算在房间球那边对应位置并生成真实小球
                    if (roomBall)
                    {
                        // 比例映射
                        float distInSoccer = distToSoccerCenter; // pinchStartPosition离足球中心距离
                        float ratio = distInSoccer / soccerRadius;

                        float roomRadius = roomBall.localScale.x * 0.5f;
                        float distInRoom = ratio * roomRadius + 0.02f; // +0.02稍微在外面

                        // 朝向与方向
                        Vector3 offsetDir = pinchStartPosition - soccerBall.position; // 捏合点相对足球中心
                        Vector3 spawnPos = roomBall.position + offsetDir.normalized * distInRoom;
                        Vector3 flyDirection = -offsetDir.normalized; // 往里飞

                        if (usePoolForRealBall)
                        {
                            // 从对象池获取小球(小组脚本: SmallBallPool + PoolManager)
                            var smallBall = PoolManager.Instance.SmallBallPool.Get();
                            if (smallBall)
                            {
                                smallBall.transform.position = spawnPos;
                                // 如果脚本SmallBall里有 VelocityChange() 等方法，可用
                                // 例如:
                                smallBall.VelocityChange(flyDirection, launchForce);
                            }
                        }
                        else
                        {
                            // 直接 Instantiate realBallPrefab
                            if (realBallPrefab)
                            {
                                var realObj = Instantiate(realBallPrefab, spawnPos, Quaternion.LookRotation(flyDirection));
                                Rigidbody rb = realObj.GetComponent<Rigidbody>();
                                if (rb) rb.velocity = flyDirection * launchForce;
                            }
                        }
                    }
                }

                // 重置
                isPinching = false;
            }
        }
    }
}
