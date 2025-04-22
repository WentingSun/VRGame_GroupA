using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class DualSphereSlingshotDynamic : MonoBehaviour
{
    [Header("球体引用")]
    public Transform soccerShell;
    public Transform roomShell;

    [Header("XR Origin")]
    public Transform xrOriginTransform;

    [Header("预制件与渲染")]
    public GameObject virtualBallPrefab;
    public GameObject realBallPrefab;
    public LineRenderer fingerLineThumb;
    public LineRenderer fingerLineIndex;
    public DottedLineRenderer internalDots;

    [Header("内部虚线链锚点")]
    public Transform pinchPointAnchor;
    public Transform pullEndAnchor;

    [Header("参数")]
    public float activationDistance = 0.05f;
    public float pinchThreshold = 0.03f;
    public float maxPullDistance = 0.3f;
    public float minLaunchForce = 2f;
    public float maxLaunchForce = 10f;

    private XRHandSubsystem handSub;
    private bool[] isPinching = new bool[2];
    private bool[] hasLaunched = new bool[2];
    private float[] pullAmt = new float[2];
    private GameObject[] currentVball = new GameObject[2];
    private Vector3[] pinchPoint = new Vector3[2];
    public AudioClip pinchClip;
    public AudioClip launchClip;
    void Start()
    {
        handSub = XRGeneralSettings.Instance.Manager
                  .activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        if (fingerLineThumb != null) fingerLineThumb.enabled = false;
        if (fingerLineIndex != null) fingerLineIndex.enabled = false;
        if (internalDots != null) internalDots.enabled = false;
        internalDots?.ClearDots();
    }

    void Update()
    {
        if (handSub == null || !handSub.running) return;
        if (soccerShell == null || roomShell == null) return;

        Vector3 center = soccerShell.position;
        float radius = soccerShell.localScale.x * 0.5f;
        Vector3 roomCenter = roomShell.position;
        float roomRadius = roomShell.localScale.x * 0.5f;

        XRHand[] hands = { handSub.leftHand, handSub.rightHand };
        for (int i = 0; i < 2; i++)
        {
            var hand = hands[i];
            if (!hand.isTracked) { EndPinch(i); continue; }

            if (!hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose tP) ||
                !hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose iP))
            {
                EndPinch(i);
                continue;
            }

            Vector3 thumb = xrOriginTransform.TransformPoint(tP.position);
            Vector3 index = xrOriginTransform.TransformPoint(iP.position);
            Vector3 mid = (thumb + index) * 0.5f;
            float dist = Vector3.Distance(thumb, index);

            bool onSurf = Mathf.Abs(Vector3.Distance(mid, center) - radius) < activationDistance;
            bool pinched = dist < pinchThreshold && onSurf;

            if (pinched)
            {
                // --- 捏合开始 ---
                if (!isPinching[i])
                {
                    isPinching[i] = true;
                    hasLaunched[i] = false;
                    //添加捏合并开始拉拽音效（Audio）



                    // 记录表面捏合点
                    pinchPoint[i] = center + (mid - center).normalized * radius;

                    // 生成虚拟小球
                    if (virtualBallPrefab != null)
                        currentVball[i] = Instantiate(virtualBallPrefab,
                                                      mid,
                                                      Quaternion.identity);

                    // 启用外部线
                    if (fingerLineThumb != null) { fingerLineThumb.positionCount = 2; fingerLineThumb.enabled = true; }
                    if (fingerLineIndex != null) { fingerLineIndex.positionCount = 2; fingerLineIndex.enabled = true; }

                    // 清空并启用内部虚线
                    if (internalDots != null)
                    {
                        internalDots.ClearDots();
                        internalDots.enabled = true;
                    }
                }

                // 更新外部线
                fingerLineThumb?.SetPosition(0, thumb);
                fingerLineThumb?.SetPosition(1, pinchPoint[i]);
                fingerLineIndex?.SetPosition(0, index);
                fingerLineIndex?.SetPosition(1, pinchPoint[i]);

                // 虚拟小球跟随
                if (currentVball[i] != null)
                    currentVball[i].transform.position = mid;

                // 计算拉拽量
                Vector3 externalDir = (mid - pinchPoint[i]).normalized;
                float rawPull = Vector3.Dot(mid - pinchPoint[i], externalDir);
                pullAmt[i] = Mathf.Clamp(rawPull, 0f, maxPullDistance);

                // 更新内部虚线链（沿 internalDir = -externalDir）
                Vector3 internalDir = -externalDir;
                Vector3 internalEnd = pinchPoint[i] + internalDir * pullAmt[i];

                pinchPointAnchor.position = pinchPoint[i];
                pullEndAnchor.position = internalEnd;
                if (internalDots != null)
                {
                    internalDots.startPoint = pinchPointAnchor;
                    internalDots.endPoint = pullEndAnchor;
                }

                continue;
            }

            // --- 松手发射 ---
            if (isPinching[i] && !hasLaunched[i])
            {
                hasLaunched[i] = true;
                isPinching[i] = false;
                //添加松手发射小球音效（Audio）



                // 隐藏外部线
                if (fingerLineThumb != null) fingerLineThumb.enabled = false;
                if (fingerLineIndex != null) fingerLineIndex.enabled = false;

                // 立即清理并禁用内部虚线
                if (internalDots != null)
                {
                    internalDots.ClearDots();
                    internalDots.enabled = false;
                }

                // 清理虚拟小球
                if (currentVball[i] != null) Destroy(currentVball[i]);

                // 计算发射方向：从当前手指中点指向首次捏合点
                Vector3 fireDir = (pinchPoint[i] - mid).normalized;

                // 保留原先的 spawnPos 逻辑
                Vector3 offsetDir = (pinchPoint[i] - center).normalized;
                Vector3 spawnPos = roomCenter + offsetDir * roomRadius;

                float pr = pullAmt[i] / maxPullDistance;
                float force = Mathf.Lerp(minLaunchForce, maxLaunchForce, pr);

                var real = Instantiate(realBallPrefab, spawnPos, Quaternion.identity);
                if (real.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.AddForce(fireDir * force, ForceMode.VelocityChange);
                }
            }
        }
    }

    void EndPinch(int i)
    {
        if (!isPinching[i]) return;
        isPinching[i] = false;
        if (fingerLineThumb != null) fingerLineThumb.enabled = false;
        if (fingerLineIndex != null) fingerLineIndex.enabled = false;
        if (internalDots != null)
        {
            internalDots.ClearDots();
            internalDots.enabled = false;
        }
        if (currentVball[i] != null) Destroy(currentVball[i]);
        pullAmt[i] = 0f;
    }
}
