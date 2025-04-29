using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Hands;

public class VRPointer : MonoBehaviour
{
    [Header("手部射线设置")]
    public XRHandSubsystem handSubsystem;         // 手部子系统
    public float rayLength = 5f;                  // 射线长度
    public LayerMask uiLayer;                     // UI 层

    [Header("参考空间（XR Origin 根节点，用于将关节本地坐标转换为世界坐标）")]
    public Transform referenceSpace;              // 如果不需要可留空

    private LineRenderer lineRenderer;
    private bool isPointerEnabled = true;        // 是否启用光线可视化

   private void Start()
    {
        // 添加 LineRenderer 用于可视化射线
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.001f; // 更细的光线
        lineRenderer.endWidth = 0.001f;
        lineRenderer.positionCount = 2;

        // 使用支持透明度的材质
        lineRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
        lineRenderer.material.color = new Color(0f, 0.3f, 0f, 0.2f); // 更暗的绿色，20%透明度

        // 设置渐变颜色
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0f, 0.3f, 0f, 0.5f), 0f), // 起点颜色
                new GradientColorKey(new Color(0f, 0.1f, 0f, 0.1f), 1f)  // 终点颜色
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.5f, 0f), // 起点透明度
                new GradientAlphaKey(0.1f, 1f)  // 终点透明度
            }
        );
        lineRenderer.colorGradient = gradient;

        // 世界空间下绘制，确保 SetPosition 用的是世界坐标
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;

        // 自动获取手部子系统
        if (handSubsystem == null)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            handSubsystem = subsystems.Count > 0 ? subsystems[0] : null;
        }

        // 监听游戏状态变化
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        // 移除监听
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState newState)
    {
        // 仅在 GameStart 和 GameOver 状态下启用光线可视化
        isPointerEnabled = (newState == GameState.GameStart || newState == GameState.GameOver);
        if (!isPointerEnabled)
        {
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!isPointerEnabled || handSubsystem == null || !handSubsystem.running)
            return;

        Vector3 rayOrigin;
        Vector3 rayDirection;

        // 仅检测左手
        XRHand leftHand = handSubsystem.leftHand;
        Pose indexTipPose;

        // 检测左手是否被追踪以及是否处于指向状态
        if (leftHand.isTracked &&
            leftHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out indexTipPose) &&
            IsPointingGesture(leftHand)) // 检测是否是指向手势
        {
            // 获取手指尖的局部位移和朝向
            Vector3 localPos = indexTipPose.position;
            Vector3 localFwd = indexTipPose.rotation * Vector3.forward;

            // 如果 referenceSpace 不为空，转换为世界坐标
            if (referenceSpace != null)
            {
                rayOrigin = referenceSpace.TransformPoint(localPos);
                rayDirection = referenceSpace.TransformDirection(localFwd);
            }
            else
            {
                rayOrigin = localPos;
                rayDirection = localFwd;
            }

            // 可视化射线
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, rayOrigin);
            lineRenderer.SetPosition(1, rayOrigin + rayDirection * rayLength);

            // 检测射线碰撞
            Ray ray = new Ray(rayOrigin, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, uiLayer))
            {
                if (hit.collider != null && hit.collider.TryGetComponent<Button>(out Button btn))
                {
                    btn.onClick.Invoke();
                    Debug.Log($"[VRPointer] Button {btn.name} clicked.");
                }
            }
        }
        else
        {
            // 如果未检测到指向手势，隐藏射线
            lineRenderer.enabled = false;
        }
    }

    private bool IsPointingGesture(XRHand hand)
    {
        // 检测食指是否伸直（可以根据具体需求调整逻辑）
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose) &&
            hand.GetJoint(XRHandJointID.IndexProximal).TryGetPose(out Pose indexProximalPose))
        {
            // 判断食指尖和食指根部的方向是否接近手的局部前方
            Vector3 direction = (indexTipPose.position - indexProximalPose.position).normalized;

            // 使用手部的局部前方向作为参考方向
            Vector3 handForward = (indexProximalPose.rotation * Vector3.forward).normalized;

            // 判断方向是否接近手部的局部前方向
            return Vector3.Dot(direction, handForward) > 0.8f; // 0.8 表示接近前方
        }
        return false;
    }
}