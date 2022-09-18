using UnityEngine;
using Common;
using DefferedRender;
using UnityEngine.Rendering;

public class HookRopeManage : GPUDravinBase
{
    public static HookRopeManage Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("HookRopeManage");
                go.AddComponent<HookRopeManage>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    private static HookRopeManage instance;
    

    /// <summary>    /// �洢�õ�����    /// </summary>
    PoolingList<Transform> poolingList;
    Transform target;   //�����Ŀ��
    Material material;  //��ʾ�õĲ���
    float particleSize; //���Ӵ�С����ȡ���ʵ�����

    bool isInsert;      //�Ƿ�������ջ��

    /// <summary>    /// �õ������Ŀ����󣬿���Ϊ��    /// </summary>
    public Transform Target
    {
        get
        {
            return target;
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        poolingList = new PoolingList<Transform>();
        material = Resources.Load<Material>("EffectMaterial/SimpleParticle");
        if (material == null)
            Debug.LogError("��Դ���ش���");
        particleSize = material.GetFloat("_ParticleSize");
        GPUDravinDrawStack.Instance.InsertRender(this);
        isInsert = true;
    }

    /// <summary>
    /// ����Ҫ��Ϊ�����ڵ��ģ��λ�����괫�룬��Ϊ�ж�λ�õĸ���
    /// </summary>
    public void AddNode(Transform pos)
    {
        poolingList.Add(pos);
    }

    public void RemoveNode(Transform pos)
    {
        poolingList.Remove(pos);
    }


    private void FixedUpdate()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            target = null;
            return;
        }
        Transform camTran = camera.transform;
        Vector4[] planes = GetFrustumPlane(camera);
        int minIndex = -1;
        for(int i=0; i<poolingList.size; i++)
        {
            for(int j=0; j < 6; j++)
            {
                Vector3 oriPos = poolingList.list[i].transform.position;
                //����false��Ҳ������������˳�
                if (IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * particleSize + camTran.up * -particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * -particleSize + camTran.up * -particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * -particleSize + camTran.up * particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * particleSize + camTran.up * particleSize))
                    break;
                
                if(j == 5)
                {
                    if (minIndex == -1)
                        minIndex = i;
                    else if((poolingList.list[i].transform.position - camera.transform.position).sqrMagnitude <
                        (poolingList.list[minIndex].transform.position - camera.transform.position).sqrMagnitude)
                    {
                        minIndex = i;
                    }
                }
            }
        }
        if(minIndex != -1)
        {
            target = poolingList.list[minIndex];
            return;
        }
        target = null;
    }

    private void OnDisable()
    {
        if (isInsert)
        {
            GPUDravinDrawStack.Instance.RemoveRender(this);
            isInsert = false;
        }
    }

    private void OnDestroy()
    {
        if (isInsert)
        {
            GPUDravinDrawStack.Instance.RemoveRender(this);
            isInsert = false;
        }
    }

    public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
    {
        if (material == null || Target == null)
            return;

        buffer.SetGlobalVector("_WorldPos", Target.position);
        buffer.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Points, 1);
        ExecuteBuffer(ref buffer, context);
        return;
    }

    public override void DrawByProjectMatrix(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Matrix4x4 projectMatrix)
    {
        return;
    }

    public override void DrawOtherSSS(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
    {
        return;
    }

    public override void DrawPreSSS(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
    {
        return;
    }

    public override void SetUp(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
    {
        return;
    }
}
