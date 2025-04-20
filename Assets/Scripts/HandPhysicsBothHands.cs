using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandPhysicsBothHands : MonoBehaviour
{
    [Header("Collider Prefab (�� SphereCollider & Kinematic Rigidbody)")]
    public GameObject jointColliderPrefab;
    [Header("XR Origin (for world space)")]
    public Transform xrOrigin;

    [Header("Ҫ���ٵĹؽ��б�")]
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

    [Header("��ײ�� (��������Tags&Layers�ｨ��)")]
    public string handLayer = "PlayerHand";

    // �ڲ��ֵ䣺key = "L_IndexTip" �� "R_IndexTip"
    private Dictionary<string, GameObject> colliders = new Dictionary<string, GameObject>();
    private XRHandSubsystem handSub;

    void Start()
    {
        // ��ȡ XR Hands ��ϵͳ
        handSub = XRGeneralSettings.Instance.Manager
                 .activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSub == null)
        {
            Debug.LogError("�Ҳ��� XRHandSubsystem�������� XR Hands ���");
            enabled = false;
            return;
        }
        if (jointColliderPrefab == null || xrOrigin == null)
        {
            Debug.LogError("���� Inspector ��� jointColliderPrefab �� xrOrigin");
            enabled = false;
            return;
        }

        // ��� Layer
        int layerIndex = LayerMask.NameToLayer(handLayer);
        if (layerIndex < 0)
        {
            Debug.LogError($"δ�ҵ��� ��{handLayer}���������� Tags & Layers �����");
            enabled = false;
            return;
        }

        // ��������ֻ�֡�ÿ���ؽڷֱ�ʵ����
        foreach (var id in jointsToTrack)
        {
            // �� hand
            var goL = Instantiate(jointColliderPrefab, xrOrigin);
            goL.name = "L_" + id;
            goL.layer = layerIndex;
            colliders.Add(goL.name, goL);

            // �� hand
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
        // ����ʵ��
        foreach (var kv in colliders)
            if (kv.Value != null) Destroy(kv.Value);
        colliders.Clear();
    }
}
