using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPointer : MonoBehaviour
{
    [Header("手部射线设置")]
    public XRHandSubsystem handSubsystem;         // 手部子系统
    public float rayLength = 5f;                  // 射线长度
    public LayerMask uiLayer;                     // UI 层

    [Header("手动射线起点设置")]
    public Transform manualRayOrigin;             // 手动射线起点

    [Header("XR 控制器设置")]
    public XRController controller;               // XR 控制器（左手或右手）

    [Header("参考空间（XR Origin 根节点，用于将关节本地坐标转换为世界坐标）")]
    public Transform referenceSpace;              // 如果不需要可留空

    private LineRenderer lineRenderer;

    private void Start()
    {
        if (controller == null)
        {
            // 自动查找场景中的 Left Controller
            controller = GameObject.Find("Left Controller")?.GetComponent<XRController>();
            if (controller == null)
                Debug.LogError("Left Controller not found or missing XRController component.");
        }

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
        bool isManual = manualRayOrigin != null;

        if (isManual)
        {
            // 手动起点逻辑
            rayOrigin = manualRayOrigin.position;
            rayDirection = manualRayOrigin.forward;
        }
        else
        {
            // 动态获取手指尖 IndexTip
            XRHand leftHand = handSubsystem.leftHand;
            XRHand rightHand = handSubsystem.rightHand;
            Pose indexTipPose;

            if (leftHand.isTracked &&
                leftHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out indexTipPose))
            {
                // 左手 OK
            }
            else if (rightHand.isTracked &&
                     rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out indexTipPose))
            {
                // 右手 OK
            }
            else
            {
                // 两只手都没追踪到 → 隐藏射线
                lineRenderer.enabled = false;
                return;
            }

            // 先拿到手指尖的“局部”位移和朝向
            Vector3 localPos = indexTipPose.position;
            Vector3 localFwd = indexTipPose.rotation * Vector3.forward;

            // 如果 referenceSpace 不为空，就做一次 Transform；否则直接当世界坐标用
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
        }

        // —— 调试输出 & 可视化 —— 
        Debug.Log($"[VRPointer] branch={(isManual ? "Manual" : "Dynamic")}, origin={rayOrigin}");
        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength, Color.red);

        // LineRenderer 可视化射线
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rayOrigin);
        lineRenderer.SetPosition(1, rayOrigin + rayDirection * rayLength);

        // XR Controller 输入检测 & UI 点击
        if (controller != null &&
            controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) &&
            isPressed)
        {
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
    }
}
