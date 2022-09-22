using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{

    /// <summary>    /// ���ӵĸ�������    /// </summary>
    public struct ParticleNodeData
    {
        public Vector3 beginPos;        //�����������г�ʼλ��
        public Vector3Int initEnum;     //x:��ʼ������״,y:�Ƿ�ʹ��������z:ͼƬ���
        public Vector2 sphereData;      //��ʼ����������Ҫ������
        public Vector3 cubeRange;       //��ʼ����������ķ�Χ
        public Vector3 lifeTimeRange;   //�������ڵķ�Χ,x:����ͷ�ʱ��,Y:���ʱ��,Z:�������ʱ��
        public Vector3 noiseData;       //���������ٶ�ʱ��Ҫ������
        public Vector3Int outEnum;      //ȷ�����ʱ�㷨��ö��
        public Vector2 smoothRange;     //���ӵĴ�С��Χ
    };


    public class ParticleNoiseFactory : GPUDravinBase
    {
        [SerializeField]
        /// <summary>        /// �����������        /// </summary>
        int particleGroupCount = 1500;
        /// <summary>        /// ������        /// </summary>
        ParticleNodeData[] particleNodes;
        /// <summary>        /// ������洢λ�ã�ÿһ�θ��¶�ֻ����¸�����        /// </summary
        ComputeBuffer groupsBuffer;
        /// <summary>        /// �������ӵĴ洢λ�ã�ֻ����г�ʼ����֮����GPU����        /// </summary>
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

            //��ʼ��������
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