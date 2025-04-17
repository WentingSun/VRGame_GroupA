using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandSlingshotInteractor : MonoBehaviour
{
    [Header("World References (from WorldManager)")]
    [Tooltip("����(С����) �C ������ֶ���ֵ�����Զ���WorldManager��ȡ FootBallWorld")]
    public Transform soccerBall;      // ����С�������磩
    [Tooltip("������(������) �C ������ֶ���ֵ�����Զ���WorldManager��ȡ RoomBallWorld")]
    public Transform roomBall;        // �����򣨴��������磩

    [Header("Prefabs / Pools")]
    [Tooltip("�����������ڲ����ݳ��ֵ�����С���������������")]
    public GameObject virtualBallPrefab;
    [Tooltip("��ʹ�ö���أ����Բ�ָ��Prefab��ͳһ��SmallBallPool��ȡ")]
    public GameObject realBallPrefab; // �����գ������ֻ���߶����

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Settings")]
    [Tooltip("������������漤��ľ��뷶Χ���������棩")]
    public float activationDistance = 0.03f;
    [Tooltip("ʳָĴָ�����ֵ")]
    public float pinchThreshold = 0.03f;
    [Tooltip("����������")]
    public float maxPullDistance = 0.3f;
    [Tooltip("��������")]
    public float launchForce = 10f;
    [Tooltip("����С��೤ʱ�����ʧ")]
    public float vanishDelay = 1f;
    [Tooltip("�Ƿ���������С��")]
    public bool spawnVirtualBall = true;
    [Tooltip("�Ƿ�ʹ�ö����������ʵС��")]
    public bool usePoolForRealBall = true;

    // XR Hands Subsystem
    private XRHandSubsystem handSubsystem;

    // ����״̬
    private bool isPinching = false;
    private bool hasLaunched = false;
    private Vector3 pinchStartPosition; // ��ϵ�

    void Start()
    {
        // ��û����ʽָ���������ã���� WorldManager ����
        if (!soccerBall && WorldManager.Instance && WorldManager.Instance.FootBallWorld)
        {
            soccerBall = WorldManager.Instance.FootBallWorld;
        }
        if (!roomBall && WorldManager.Instance && WorldManager.Instance.RoomBallWorld)
        {
            roomBall = WorldManager.Instance.RoomBallWorld;
        }

        // XRHands ��ʼ��
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem is missing! ��ȷ�������� XR Hands ���");
        }

        // ���ûָ��LineRenderer�������������ȡ
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer) lineRenderer.enabled = false;
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;

        // ����������
        XRHand[] hands = { handSubsystem.leftHand, handSubsystem.rightHand };
        foreach (var hand in hands)
        {
            if (!hand.isTracked) continue;

            // ��ȡ��ָ�ؼ��ؽ�
            XRHandJoint thumb = hand.GetJoint(XRHandJointID.ThumbTip);
            XRHandJoint index = hand.GetJoint(XRHandJointID.IndexTip);

            if (!thumb.TryGetPose(out Pose thumbPose) || !index.TryGetPose(out Pose indexPose))
                continue;

            Vector3 thumbPos = thumbPose.position;
            Vector3 indexPos = indexPose.position;
            float pinchDist = Vector3.Distance(indexPos, thumbPos);

            // ��Ҫȷ�����������������桱
            // ��������е㵽�������ĵľ��� vs. ����뾶
            if (!soccerBall) return; // �޷����
            float soccerRadius = soccerBall.localScale.x * 0.5f;
            Vector3 pinchMid = (thumbPos + indexPos) * 0.5f;
            float distToSoccerCenter = Vector3.Distance(pinchMid, soccerBall.position);
            bool onSoccerSurface = Mathf.Abs(distToSoccerCenter - soccerRadius) < activationDistance;

            // �ж����
            bool currentPinching = (pinchDist < pinchThreshold) && onSoccerSurface;

            if (currentPinching)
            {
                // �տ�ʼ���
                if (!isPinching)
                {
                    isPinching = true;
                    hasLaunched = false;
                    if (lineRenderer) lineRenderer.enabled = true;

                    pinchStartPosition = pinchMid; // ��¼��ϵ�
                }

                // ���ӻ�����: ����ϵ����������ķ���
                if (lineRenderer)
                {
                    Vector3 dirToCenter = (soccerBall.position - pinchStartPosition).normalized;
                    Vector3 pullEnd = pinchStartPosition - dirToCenter * maxPullDistance;
                    lineRenderer.SetPosition(0, pinchStartPosition);
                    lineRenderer.SetPosition(1, pullEnd);
                }

                // һ��ֻ����һֻ��
                return;
            }
            else
            {
                // ���֮ǰ����ϣ��������֣�ִ�з���
                if (isPinching && !hasLaunched)
                {
                    hasLaunched = true;
                    if (lineRenderer) lineRenderer.enabled = false;

                    // 1) �������ڲ�����һ��������С�򡱣���ѡ��
                    if (spawnVirtualBall && virtualBallPrefab)
                    {
                        Vector3 insideDir = (soccerBall.position - pinchStartPosition).normalized;
                        var virtualBall = Instantiate(
                            virtualBallPrefab,
                            pinchStartPosition,
                            Quaternion.LookRotation(insideDir)
                        );
                        Destroy(virtualBall, vanishDelay);
                    }

                    // 2) �����ڷ������Ǳ߶�Ӧλ�ò�������ʵС��
                    if (roomBall)
                    {
                        // ����ӳ��
                        float distInSoccer = distToSoccerCenter; // pinchStartPosition���������ľ���
                        float ratio = distInSoccer / soccerRadius;

                        float roomRadius = roomBall.localScale.x * 0.5f;
                        float distInRoom = ratio * roomRadius + 0.02f; // +0.02��΢������

                        // �����뷽��
                        Vector3 offsetDir = pinchStartPosition - soccerBall.position; // ��ϵ������������
                        Vector3 spawnPos = roomBall.position + offsetDir.normalized * distInRoom;
                        Vector3 flyDirection = -offsetDir.normalized; // �����

                        if (usePoolForRealBall)
                        {
                            // �Ӷ���ػ�ȡС��(С��ű�: SmallBallPool + PoolManager)
                            var smallBall = PoolManager.Instance.SmallBallPool.Get();
                            if (smallBall)
                            {
                                smallBall.transform.position = spawnPos;
                                // ����ű�SmallBall���� VelocityChange() �ȷ���������
                                // ����:
                                smallBall.VelocityChange(flyDirection, launchForce);
                            }
                        }
                        else
                        {
                            // ֱ�� Instantiate realBallPrefab
                            if (realBallPrefab)
                            {
                                var realObj = Instantiate(realBallPrefab, spawnPos, Quaternion.LookRotation(flyDirection));
                                Rigidbody rb = realObj.GetComponent<Rigidbody>();
                                if (rb) rb.velocity = flyDirection * launchForce;
                            }
                        }
                    }
                }

                // ����
                isPinching = false;
            }
        }
    }
}
