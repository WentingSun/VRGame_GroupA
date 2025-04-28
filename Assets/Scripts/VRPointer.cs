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
    public XRHandSubsystem handSubsystem; // 手部子系统
    public float rayLength = 5f; // 射线长度
    public LayerMask uiLayer; // UI 层

    [Header("手动射线起点设置")]
    public Transform manualRayOrigin; // 手动射线起点

    [Header("XR 控制器设置")]
    public XRController controller; // XR 控制器（左手或右手）

    private LineRenderer lineRenderer;

    private void Start()
    {
    if (controller == null)
    {
        // 自动查找场景中的 Left Controller
        controller = GameObject.Find("Left Controller")?.GetComponent<XRController>();

        if (controller == null)
        {
            Debug.LogError("Left Controller not found or missing XRController component.");
        }
    }

        // 添加 LineRenderer 用于可视化射线
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.green;

        // 获取手部子系统
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

        // 如果手动设置了射线起点，则使用手动设置的 Transform
        if (manualRayOrigin != null)
        {
            rayOrigin = manualRayOrigin.position;
            rayDirection = manualRayOrigin.forward;
        }
        else
        {
            // 动态获取手部关节数据
            XRHand leftHand = handSubsystem.leftHand;
            XRHand rightHand = handSubsystem.rightHand;

            Pose indexTipPose = default; // 初始化为默认值
            Pose leftPose = default; // 初始化为默认值
            Pose rightPose = default; // 初始化为默认值
            bool isLeftHandTracked = leftHand.isTracked && leftHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out leftPose);
            bool isRightHandTracked = rightHand.isTracked && rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out rightPose);

            // 如果左手被追踪，使用左手的 IndexTip 数据
            if (isLeftHandTracked)
            {
                indexTipPose = leftPose;
            }
            // 如果右手被追踪，使用右手的 IndexTip 数据
            else if (isRightHandTracked)
            {
                indexTipPose = rightPose;
            }
            // 如果两只手都未被追踪，隐藏射线并返回
            else
            {
                lineRenderer.enabled = false;
                return;
            }

            rayOrigin = indexTipPose.position;
            rayDirection = indexTipPose.rotation * Vector3.forward;
        }

        // 确保可视化打开
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rayOrigin);
        lineRenderer.SetPosition(1, rayOrigin + rayDirection * rayLength);

        // 检测 XR 控制器输入
        if (controller != null && controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed)
        {
            // 发射射线并检测 UI
            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLength, uiLayer))
            {
                if (hit.collider != null && hit.collider.TryGetComponent<Button>(out Button button))
                {
                    button.onClick.Invoke(); // 触发按钮点击事件
                    Debug.Log($"Button {button.name} clicked.");
                }
            }
        }
    }
}