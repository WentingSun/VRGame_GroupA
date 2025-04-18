using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

[RequireComponent(typeof(LineRenderer))]
public class IndexTipToCenterLine : MonoBehaviour
{
    [Header("XR Hands 子系统")]
    private XRHandSubsystem handSubsystem;

    [Header("足球球心")]
    [Tooltip("把 FootBallWorldShell 的 Transform 拖到这里")]
    public Transform soccerShell;

    [Header("选择哪只手")]
    public bool useLeftHand = false;

    [Header("线条设置")]
    [Tooltip("LineRenderer 会自动在 Start() 设置为 2 个顶点")]
    public LineRenderer lineRenderer;
    public Color lineColor = Color.green;


    public Transform xrOriginTransform;
    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        
        // 初始化 XR Hands
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
                        ?.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null)
            Debug.LogError("[IndexTipToCenterLine] 找不到 XRHandSubsystem，请检查 XR Hands 插件");
        if (soccerShell == null)
            Debug.LogError("[IndexTipToCenterLine] 未设置 soccerShell，请在 Inspector 填入足球的 Transform");

        // 初始化 LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
        {
            lineRenderer.enabled = false;
            return;
        }
        if (soccerShell == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 选哪只手
        XRHand hand = useLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;
        if (!hand.isTracked)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 取 IndexTip 关节位置
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose tipPose))
        {

            Vector3 worldTipPos = xrOriginTransform.TransformPoint(tipPose.position);
            Vector3 center = soccerShell.position;

            // 启用并设置 LineRenderer
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, worldTipPos);
            lineRenderer.SetPosition(1, center);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
