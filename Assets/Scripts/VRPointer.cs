using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class VRPointer : MonoBehaviour
{
    [Header("手部射线设置")]
    public XRHandSubsystem handSubsystem;    // 手部子系统，Inspector 可留空自动获取
    public float rayLength = 5f;             // 射线长度
    public LayerMask uiLayer;                // UI 可交互层

    [Header("参考空间 (XR Origin，用于坐标转换)")]
    public Transform referenceSpace;         // 留空则使用局部坐标

    [Header("可视化射线 (请在场景中先拖一个带 LineRenderer 的空物体进来)")]
    public LineRenderer pointerLine;

    [Header("手势灵敏度")]
    [Range(0.5f, 1f)]
    public float pointingDotThreshold = 0.8f; // 点按手势阈值 (食指方向与指根前方向的点积)

    private bool isPointerEnabled = true;

    void Awake()
    {
        // 自动获取子系统
        if (handSubsystem == null)
        {
            var subs = new List<XRHandSubsystem>();
            SubsystemManager.GetInstances(subs);
            handSubsystem = subs.Count > 0 ? subs[0] : null;
        }

        // 确保 line 被你在 Inspector 里拖进来了
        if (pointerLine == null)
            Debug.LogError("[VRPointer] 请在 Inspector 里绑定 pointerLine (LineRenderer)");
        else
        {
            pointerLine.positionCount = 2;
            pointerLine.useWorldSpace = true;
            pointerLine.enabled = false;      // 一开始隐藏
        }

        // 监听游戏状态
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState newState)
    {
        // 只有在开始或结束时显示指针，其它状态一律隐藏
        isPointerEnabled = (newState == GameState.GameStart || newState == GameState.GameOver);
        if (pointerLine != null && !isPointerEnabled)
            pointerLine.enabled = false;
    }

    void Update()
    {
        if (!isPointerEnabled || handSubsystem == null || !handSubsystem.running)
            return;

        // 这里只检测左手，右手同理可加
        XRHand hand = handSubsystem.leftHand;

        // 要同时满足：追踪中 + 食指尖有 Pose + 指向手势
        if (hand.isTracked
            && hand.GetJoint(XRHandJointID.IndexTip)
                  .TryGetPose(out Pose tipPose)
            && hand.GetJoint(XRHandJointID.IndexProximal)
                  .TryGetPose(out Pose proxPose)
            && IsPointingGesture(tipPose, proxPose))
        {
            // 把局部坐标转换到世界空间
            Vector3 origin = tipPose.position;
            Vector3 forward = tipPose.rotation * Vector3.forward;
            if (referenceSpace != null)
            {
                origin = referenceSpace.TransformPoint(origin);
                forward = referenceSpace.TransformDirection(forward);
            }

            // 显示并更新射线
            pointerLine.enabled = true;
            pointerLine.SetPosition(0, origin);
            pointerLine.SetPosition(1, origin + forward * rayLength);

            // 发起物理射线检测 UI
            if (Physics.Raycast(origin, forward, out var hit, rayLength, uiLayer))
            {
                if (hit.collider.TryGetComponent<Button>(out Button btn))
                {
                    btn.onClick.Invoke();
                    Debug.Log($"[VRPointer] 点击按钮: {btn.name}");
                }
            }
        }
        else
        {
            // 手势不满足时隐藏
            pointerLine.enabled = false;
        }
    }

    /// <summary>
    /// 简单判断“点按手势”：食指尖到指根方向，与指根朝前方向点积须大于阈值
    /// </summary>
    bool IsPointingGesture(Pose tip, Pose prox)
    {
        Vector3 dir = (tip.position - prox.position).normalized;
        Vector3 handFwd = prox.rotation * Vector3.forward;
        return Vector3.Dot(dir, handFwd) > pointingDotThreshold;
    }
}
