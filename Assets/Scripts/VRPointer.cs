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

    private void Start()
    {
        // 添加 LineRenderer 用于可视化射线
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.green;

        // ★ 核心：世界空间下绘制，确保 SetPosition 用的是世界坐标
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;

        // 自动获取手部子系统
        if (handSubsystem == null)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            handSubsystem = subsystems.Count > 0 ? subsystems[0] : null;
        }
    }

    private void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
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
        else if (IsRockGesture(leftHand))
            {
                Debug.Log("Rock gesture detected! Triggering pause menu.");
                MenuManager.Instance.OnPauseGameButton(); // 触发暂停菜单
            }
        else
        {
            // 如果未检测到指向手势，隐藏射线
            lineRenderer.enabled = false;
        }
    }
     private bool IsRockGesture(XRHand hand)
    {
        // 检测中指和无名指是否弯曲
        bool isMiddleFingerBent = IsFingerBent(hand, XRHandJointID.MiddleTip, XRHandJointID.MiddleProximal);
        bool isRingFingerBent = IsFingerBent(hand, XRHandJointID.RingTip, XRHandJointID.RingProximal);

        // 检测食指和小指是否伸直
        bool isIndexFingerStraight = !IsFingerBent(hand, XRHandJointID.IndexTip, XRHandJointID.IndexProximal);
        bool isPinkyFingerStraight = !IsFingerBent(hand, XRHandJointID.LittleTip, XRHandJointID.LittleProximal);

        // 如果中指和无名指弯曲，且食指和小指伸直，则为“摇滚手势”
        return isMiddleFingerBent && isRingFingerBent && isIndexFingerStraight && isPinkyFingerStraight;
    }
    // 检测单个手指是否弯曲
    private bool IsFingerBent(XRHand hand, XRHandJointID tipJoint, XRHandJointID proximalJoint)
    {
        if (hand.GetJoint(tipJoint).TryGetPose(out Pose tipPose) &&
            hand.GetJoint(proximalJoint).TryGetPose(out Pose proximalPose))
        {
            // 计算手指尖和根部的方向
            Vector3 direction = (tipPose.position - proximalPose.position).normalized;

            // 判断手指尖是否靠近根部（弯曲）
            return Vector3.Dot(direction, Vector3.up) < 0.5f; // 0.5 表示接近弯曲状态
        }
        return false;
    }
    // 检测是否是指向手势
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