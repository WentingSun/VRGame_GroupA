using UnityEngine;

public class HandMeshPhysicsSetup : MonoBehaviour
{
    [Header("手部根节点（含运行时生成的 Left/Right Hand Tracking(Clone)）")]
    public Transform handRoot;

    [Header("要给其添加碰撞体的子物体名称（不含后缀 \"(Clone)\")")]
    public string[] meshNames = new[] { "Left Hand Tracking", "Right Hand Tracking" };

    [Header("碰撞体层（需先在 Tags & Layers 里建好这一层）")]
    public string handPhysicsLayer = "Default";

    // 记录哪几个已经处理过
    bool[] done;

    void Awake()
    {
        done = new bool[meshNames.Length];
    }
    Mesh GetMeshFromGameObjectOrChildren(GameObject go)
    {
        // 首先检查MeshFilter
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh;

        // 检查SkinnedMeshRenderer并BakeMesh
        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            Mesh bakedMesh = new Mesh();
            smr.BakeMesh(bakedMesh);
            return bakedMesh;
        }

        // 遍历子对象
        foreach (Transform child in go.transform)
        {
            Mesh childMesh = GetMeshFromGameObjectOrChildren(child.gameObject);
            if (childMesh != null)
                return childMesh;
        }

        return null; // 没找到mesh
    }
    void Update()
    {
        int layer = LayerMask.NameToLayer(handPhysicsLayer);
        if (layer < 0)
        {
            Debug.LogError($"没找到层 “{handPhysicsLayer}”，请先创建");
            enabled = false;
            return;
        }

        for (int i = 0; i < meshNames.Length; i++)
        {
            if (done[i]) continue;

            // 运行时生成的名字带 “(Clone)”
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
                Debug.Log($"已成功为 {go.name} 添加 MeshCollider");
            }
            else
            {
                Debug.LogError($"无法添加MeshCollider，Mesh数据为空，GameObject名称： {go.name}");
            }
            // 添加 Rigidbody
            if (go.GetComponent<Rigidbody>() == null)
            {
                var rb = go.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            // 设置层
            go.layer = layer;

            done[i] = true;
            Debug.Log($"[HandMeshPhysicsSetup] 已为 {go.name} 添加碰撞体和刚体");
        }

        // 如果全部 done，自动停用自己
        bool all = true;
        foreach (bool d in done) if (!d) { all = false; break; }
        if (all) Destroy(this);
    }
}
