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
    

    /// <summary>    /// 存储用的链表    /// </summary>
    PoolingList<Transform> poolingList;
    Transform target;   //最近的目标
    Material material;  //显示用的材质
    Material hookRopeMat;   //绳索的材质
    float particleSize; //粒子大小，读取材质的数据

    bool isInsert;      //是否插入绘制栈中

    /// <summary>    /// 得到最近的目标对象，可能为空    /// </summary>
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
        hookRopeMat = Resources.Load<Material>("EffectMaterial/HookRopeMat");
        if (material == null)
            Debug.LogError("资源加载错误");
        particleSize = material.GetFloat("_ParticleSize");
        GPUDravinDrawStack.Instance.InsertRender(this);
        isInsert = true;
    }

    /// <summary>
    /// 将需要作为钩锁节点的模型位置坐标传入，作为判断位置的根据
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
                //返回false，也就是在里面就退出
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
        if (isLink)
        {
            buffer.SetGlobalVector("_TargetPos", finalPos);
            buffer.SetGlobalVector("_BeginPos", begin.position);
            buffer.DrawProcedural(Matrix4x4.identity, hookRopeMat, 0, MeshTopology.Points, 1);
        }

        ExecuteBuffer(ref buffer, context);
        return;
    }

    Vector3 finalPos;
    Transform begin;
    bool isLink = false;

    /// <summary>    /// 显示连接图片    /// </summary>
    public void LinkHookRope(Vector3 target, Transform begin)
    {
        finalPos = target;
        this.begin = begin;
        isLink = true;
    }

    public void CloseHookRope()
    {
        isLink = false;
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
