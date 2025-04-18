using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

[RequireComponent(typeof(LineRenderer))]
public class IndexTipToCenterLine : MonoBehaviour
{
    [Header("XR Hands ��ϵͳ")]
    private XRHandSubsystem handSubsystem;

    [Header("��������")]
    [Tooltip("�� FootBallWorldShell �� Transform �ϵ�����")]
    public Transform soccerShell;

    [Header("ѡ����ֻ��")]
    public bool useLeftHand = false;

    [Header("��������")]
    [Tooltip("LineRenderer ���Զ��� Start() ����Ϊ 2 ������")]
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
        
        // ��ʼ�� XR Hands
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
                        ?.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null)
            Debug.LogError("[IndexTipToCenterLine] �Ҳ��� XRHandSubsystem������ XR Hands ���");
        if (soccerShell == null)
            Debug.LogError("[IndexTipToCenterLine] δ���� soccerShell������ Inspector ��������� Transform");

        // ��ʼ�� LineRenderer
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

        // ѡ��ֻ��
        XRHand hand = useLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;
        if (!hand.isTracked)
        {
            lineRenderer.enabled = false;
            return;
        }

        // ȡ IndexTip �ؽ�λ��
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose tipPose))
        {

            Vector3 worldTipPos = xrOriginTransform.TransformPoint(tipPose.position);
            Vector3 center = soccerShell.position;

            // ���ò����� LineRenderer
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
