using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{

    /// <summary>    /// 粒子的根据数据    /// </summary>
    public struct ParticleNodeData
    {
        public Vector3 beginPos;        //该组粒子运行初始位置
        public Vector3Int initEnum;     //x:初始化的形状,y:是否使用重力，z:图片编号
        public Vector2 sphereData;      //初始化球坐标需要的数据
        public Vector3 cubeRange;       //初始化矩形坐标的范围
        public Vector3 lifeTimeRange;   //生存周期的范围,x:随机释放时间,Y:存活时间,Z:最大生存时间
        public Vector3 noiseData;       //噪声调整速度时需要的数据
        public Vector3Int outEnum;      //确定输出时算法的枚举
        public Vector2 smoothRange;     //粒子的大小范围
    };


    public class ParticleNoiseFactory : GPUDravinBase
    {
        [SerializeField]
        /// <summary>        /// 粒子组的数量        /// </summary>
        int particleGroupCount = 1500;
        /// <summary>        /// 粒子组        /// </summary>
        ParticleNodeData[] particleNodes;
        /// <summary>        /// 粒子组存储位置，每一次更新都只会更新该数据        /// </summary
        ComputeBuffer groupsBuffer;
        /// <summary>        /// 所有粒子的存储位置，只会进行初始化，之后由GPU控制        /// </summary>
        ComputeBuffer particlesBuffer;
        [SerializeField]
        ComputeShader compute;

        private static ParticleNoiseFactory instance;
        public static ParticleNoiseFactory Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject game = new GameObject("ParticleFactory");
                    game.AddComponent<ParticleNoiseFactory>();
                }
                return instance;
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
            DontDestroyOnLoad(gameObject);
            InitializeParticle();
        }
        bool isInsert = false;
        private void Start()
        {
            GPUDravinDrawStack.Instance.InsertRender(this);
            isInsert = true;
        }
        private void OnDestroy()
        {
            if(isInsert)
                GPUDravinDrawStack.Instance.RemoveRender(this);
            groupsBuffer.Release();
            particlesBuffer.Release();
        }

        private void FixedUpdate()
        {
            
        }

        private void InitializeParticle()
        {
            particleNodes = new ParticleNodeData[particleGroupCount];
            NoiseParticleData[] particles = new NoiseParticleData[particleGroupCount * 64];

            //初始化组数据
            for(int i=0; i<particleNodes.Length; i++)
            {
                particleNodes[i] = new ParticleNodeData
                {
                    initEnum = Vector3Int.zero,
                    lifeTimeRange = -Vector3.one
                };
            }
            groupsBuffer?.Release();
            groupsBuffer = new ComputeBuffer(particleGroupCount, Marshal.SizeOf(particleNodes[0]));
            groupsBuffer.SetData(particleNodes);

            for(int i=0; i< particles.Length; i++)
            {
                Vector4 random = new Vector4(Random.value, Random.value, Random.value, 0);
                particles[i] = new NoiseParticleData
                {
                    random = random
                };
            }
            particlesBuffer?.Release();
            particlesBuffer = new ComputeBuffer(particles.Length, Marshal.SizeOf(particles[0]));
            particlesBuffer.SetData(particles);
        }

        public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
        {
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
}