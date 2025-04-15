using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandSlingshotInteractor : MonoBehaviour
{
    [Header("References")]
    public Transform targetBall;                     // �̶�����
    public GameObject projectilePrefab;              // �ӵ�Ԥ���壨����
    public LineRenderer lineRenderer;                // ���߿��ӻ�

    [Header("Settings")]
    public float maxPullDistance = 0.3f;             // �������
    public float launchForce = 10f;                  // ������
    public float activationDistance = 0.15f;         // ������������������ϼ��Χ

    private XRHandSubsystem handSubsystem;
    private bool isPinching = false;
    private bool hasLaunched = false;
    private Vector3 pinchStartPosition;

    void Start()
    {
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem is missing! ��ȷ�������� XR Hands ���");
        }

        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
        hasLaunched = false;
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        // ����������
        XRHand[] hands = new XRHand[] { handSubsystem.leftHand, handSubsystem.rightHand };

        foreach (var hand in hands)
        {
            if (!hand.isTracked)
                continue;

            XRHandJoint thumb = hand.GetJoint(XRHandJointID.ThumbTip);
            XRHandJoint index = hand.GetJoint(XRHandJointID.IndexTip);

            if (!thumb.TryGetPose(out Pose thumbPose) || !index.TryGetPose(out Pose indexPose))
                continue;

            Vector3 thumbPos = thumbPose.position;
            Vector3 indexPos = indexPose.position;
            float pinchDistance = Vector3.Distance(indexPos, thumbPos);

            // ����������淶Χ�ڲ�������
            float distanceToBall = Vector3.Distance(indexPos, targetBall.position);
            bool nearBallSurface = distanceToBall <= activationDistance;

            bool currentPinching = pinchDistance < 0.03f && nearBallSurface;

            if (currentPinching)
            {
                if (!isPinching)
                {
                    isPinching = true;
                    lineRenderer.enabled = true;
                    hasLaunched = false;
                    pinchStartPosition = indexPos;
                }

                // ׼�ǳ������ڲ���������
                Vector3 targetDir = (targetBall.position - pinchStartPosition).normalized;
                Vector3 pullPoint = pinchStartPosition - targetDir * maxPullDistance * 0.5f;

                lineRenderer.SetPosition(0, pinchStartPosition);
                lineRenderer.SetPosition(1, pullPoint);

                return; // һ��ֻ����һֻ��
            }
            else
            {
                if (isPinching && !hasLaunched)
                {
                    Vector3 direction = (targetBall.position - pinchStartPosition).normalized;

                    GameObject projectile = Instantiate(projectilePrefab, pinchStartPosition, Quaternion.LookRotation(direction));
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.velocity = direction * launchForce;
                    }

                    hasLaunched = true;
                    lineRenderer.enabled = false;
                }

                isPinching = false;
            }
        }
    }
}
