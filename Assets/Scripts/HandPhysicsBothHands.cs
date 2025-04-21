using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandPhysicsBothHands : MonoBehaviour
{
    [Header("Collider Prefab (带 SphereCollider & Kinematic Rigidbody)")]
    public GameObject jointColliderPrefab;
    [Header("XR Origin (for world space)")]
    public Transform xrOrigin;

    [Header("要跟踪的关节列表")]
    public XRHandJointID[] jointsToTrack = new[]
    {
        XRHandJointID.Wrist,
        XRHandJointID.Palm,
        XRHandJointID.ThumbTip,
        XRHandJointID.IndexTip,
        XRHandJointID.MiddleTip,
        XRHandJointID.RingTip,
        XRHandJointID.LittleTip
    };

    [Header("碰撞层 (必须先在Tags&Layers里建好)")]
    public string handLayer = "PlayerHand";

    // 内部字典：key = "L_IndexTip" 或 "R_IndexTip"
    private Dictionary<string, GameObject> colliders = new Dictionary<string, GameObject>();
    private XRHandSubsystem handSub;

    void Start()
    {
        // 获取 XR Hands 子系统
        handSub = XRGeneralSettings.Instance.Manager
                 .activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSub == null)
        {
            Debug.LogError("找不到 XRHandSubsystem，请启用 XR Hands 插件");
            enabled = false;
            return;
        }
        if (jointColliderPrefab == null || xrOrigin == null)
        {
            Debug.LogError("请在 Inspector 填好 jointColliderPrefab 和 xrOrigin");
            enabled = false;
            return;
        }

        // 检查 Layer
        int layerIndex = LayerMask.NameToLayer(handLayer);
        if (layerIndex < 0)
        {
            Debug.LogError($"未找到层 “{handLayer}”，请先在 Tags & Layers 中添加");
            enabled = false;
            return;
        }

        // 对左右两只手、每个关节分别实例化
        foreach (var id in jointsToTrack)
        {
            // 左 hand
            var goL = Instantiate(jointColliderPrefab, xrOrigin);
            goL.name = "L_" + id;
            goL.layer = layerIndex;
            colliders.Add(goL.name, goL);

            // 右 hand
            var goR = Instantiate(jointColliderPrefab, xrOrigin);
            goR.name = "R_" + id;
            goR.layer = layerIndex;
            colliders.Add(goR.name, goR);
        }
    }

    void Update()
    {
        if (handSub == null || !handSub.running)
            return;

        UpdateHandColliders(handSub.leftHand, "L");
        UpdateHandColliders(handSub.rightHand, "R");
    }

    void UpdateHandColliders(XRHand hand, string prefix)
    {
        if (!hand.isTracked) return;

        foreach (var id in jointsToTrack)
        {
            string key = $"{prefix}_{id}";
            if (!colliders.TryGetValue(key, out var go))
                continue;

            if (hand.GetJoint(id).TryGetPose(out Pose pose))
            {
                Vector3 worldPos = xrOrigin.TransformPoint(pose.position);
                go.transform.position = worldPos;
                go.transform.rotation = pose.rotation;
            }
        }
    }

    void OnDisable()
    {
        // 清理实例
        foreach (var kv in colliders)
            if (kv.Value != null) Destroy(kv.Value);
        colliders.Clear();
    }
}
