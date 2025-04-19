using UnityEngine;

public class HandMeshPhysicsSetup : MonoBehaviour
{
    [Header("�ֲ����ڵ㣨������ʱ���ɵ� Left/Right Hand Tracking(Clone)��")]
    public Transform handRoot;

    [Header("Ҫ���������ײ������������ƣ�������׺ \"(Clone)\")")]
    public string[] meshNames = new[] { "Left Hand Tracking", "Right Hand Tracking" };

    [Header("��ײ��㣨������ Tags & Layers �ｨ����һ�㣩")]
    public string handPhysicsLayer = "Default";

    // ��¼�ļ����Ѿ������
    bool[] done;

    void Awake()
    {
        done = new bool[meshNames.Length];
    }
    Mesh GetMeshFromGameObjectOrChildren(GameObject go)
    {
        // ���ȼ��MeshFilter
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh;

        // ���SkinnedMeshRenderer��BakeMesh
        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            Mesh bakedMesh = new Mesh();
            smr.BakeMesh(bakedMesh);
            return bakedMesh;
        }

        // �����Ӷ���
        foreach (Transform child in go.transform)
        {
            Mesh childMesh = GetMeshFromGameObjectOrChildren(child.gameObject);
            if (childMesh != null)
                return childMesh;
        }

        return null; // û�ҵ�mesh
    }
    void Update()
    {
        int layer = LayerMask.NameToLayer(handPhysicsLayer);
        if (layer < 0)
        {
            Debug.LogError($"û�ҵ��� ��{handPhysicsLayer}�������ȴ���");
            enabled = false;
            return;
        }

        for (int i = 0; i < meshNames.Length; i++)
        {
            if (done[i]) continue;

            // ����ʱ���ɵ����ִ� ��(Clone)��
            string nameClone = meshNames[i] + "(Clone)";
            Transform tf = handRoot.Find(nameClone) ?? handRoot.Find(meshNames[i]);
            if (tf == null) continue;

            GameObject go = tf.gameObject;

            Mesh mesh = GetMeshFromGameObjectOrChildren(go);
            if (mesh != null && go.GetComponent<MeshCollider>() == null)
            {
                var mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = mesh;
                mc.convex = true;
                Debug.Log($"�ѳɹ�Ϊ {go.name} ��� MeshCollider");
            }
            else
            {
                Debug.LogError($"�޷����MeshCollider��Mesh����Ϊ�գ�GameObject���ƣ� {go.name}");
            }
            // ��� Rigidbody
            if (go.GetComponent<Rigidbody>() == null)
            {
                var rb = go.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            // ���ò�
            go.layer = layer;

            done[i] = true;
            Debug.Log($"[HandMeshPhysicsSetup] ��Ϊ {go.name} �����ײ��͸���");
        }

        // ���ȫ�� done���Զ�ͣ���Լ�
        bool all = true;
        foreach (bool d in done) if (!d) { all = false; break; }
        if (all) Destroy(this);
    }
}
