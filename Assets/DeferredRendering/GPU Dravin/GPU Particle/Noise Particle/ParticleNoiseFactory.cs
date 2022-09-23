using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{
    [System.Serializable]
    /// <summary>    /// 粒子的根据数据    /// </summary>
    public struct ParticleNodeData
    {
        public Vector3 beginPos;        //该组粒子运行初始位置
        public Vector3 beginSpeed;      //初始速度
        public Vector3Int initEnum;     //x:初始化的形状,y:是否使用重力，z:图片编号
        public Vector2 sphereData;      //初始化球坐标需要的数据, x=角度, y=半径
        public Vector3 cubeRange;       //初始化矩形坐标的范围
        public Vector3 lifeTimeRange;   //生存周期的范围,x:随机释放时间,Y:存活时间,Z:最大生存到的时间
        public Vector3 noiseData;       //噪声调整速度时需要的数据
        public Vector3Int outEnum;      //确定输出时算法的枚举,x:followSpeed?
        public Vector2 smoothRange;     //粒子的大小范围
        public Vector2Int uvCount;        //x:row，y:column,
        public Vector2Int drawData;     //x:颜色条编号,y是大小的编号
    };

    /// <summary>    /// 当个图片中的图集数量    /// </summary>
    [System.Serializable]
    public struct TextureUVCount
    {
        public int rowCount;
        public int columnCount;
    }

    /// <summary>
    /// 渲染粒子时的根据数据，设置一个结构体方便确定形参类型，不然数据太多了,不好检查
    /// </summary>
    public struct ParticleDrawData
    {
        /// <summary>        /// 初始位置，会在该位置及其周围生成粒子        /// </summary>
        public Vector3 beginPos;
        /// <summary>        /// 初始速度，用来进行默认速度初始化        /// </summary>
        public Vector3 beginSpeed;
        /// <summary>        /// 是否对粒子使用重力        /// </summary>
        public bool useGravity;
        /// <summary>        /// 粒子渲染是否要跟随速度，而不是朝向摄像机        /// </summary>
        public bool followSpeed;
        /// <summary>        /// radius是角度，radian是弧度(0-3.14)，用来控制这个球渲染的范围        /// </summary>
        public float radius, radian;
        /// <summary>        /// 矩形生成粒子时确定这个矩形的大小，分别表示xyz的偏移值        /// </summary>
        public Vector3 cubeOffset;
        /// <summary>        /// 这个粒子的最长生存周期        /// </summary>
        public float liftTime;
        /// <summary>        /// 单个粒子的显示时间，注意，显示时间请不要超过生存时间        /// </summary>
        public float showTime;
        /// <summary>        /// 噪声采样的频率        /// </summary>
        public float frequency;
        /// <summary>        /// 噪声采样的循环次数，次数越多越混乱，更有噪声的感觉，不要超过8次        /// </summary>
        public int octave;
        /// <summary>        /// 噪声的强度，越强粒子移动变化越快        /// </summary>
        public float intensity;
        /// <summary>        /// 粒子的大小方位，size曲线的结果会映射到该数据中        /// </summary>
        public Vector2 sizeRange;
        /// <summary>        /// 颜色编号，用来确定粒子的颜色以及透明度        /// </summary>
        public int colorIndex;
        /// <summary>        /// 选择的大小曲线编号        /// </summary>
        public int sizeIndex;
        /// <summary>        /// 选择的图片编号        /// </summary>
        public int textureIndex;
        /// <summary>        /// 粒子组数量，也就是要生成的粒子数量，一组有64个粒子        /// </summary>
        public int groupCount;
    }

    /// <summary>    /// 颜色条目的可选模式    /// </summary>
    public enum ColorIndexMode
    {
        /// <summary>   /// 全白，且透明到透明，中间一部分为白色    /// </summary>
        AlphaToAlpha = 0,
        /// <summary>        /// 透明到透明，中间是明亮的黄光        /// </summary>
        HighlightAlphaToAlpha = 1,
        /// <summary>        /// 强光，前面不透明，后面透明        /// </summary>
        HighlightToAlpha = 2,
    }

    public enum SizeCurveMode
    {
        /// <summary>        /// 从小到大        /// </summary>
        SmallToBig = 0, 
        /// <summary>        /// 从小到大再到小，类似正态分布曲线        /// </summary>
        Small_Hight_Small = 1,
    }

    public class ParticleNoiseFactory : GPUDravinBase
    {
        /// <summary>        /// 粒子组的数量        /// </summary>
        const int particleGroupCount = 3600;
        /// <summary>        /// 粒子组        /// </summary>
        ParticleNodeData[] particleNodes;
        /// <summary>        /// 粒子组存储位置，每一次更新都只会更新该数据        /// </summary
        ComputeBuffer groupsBuffer;
        /// <summary>        /// 所有粒子的存储位置，只会进行初始化，之后由GPU控制        /// </summary>
        ComputeBuffer particlesBuffer;
        [SerializeField]
        ComputeShader compute;
        //[SerializeField, GradientUsage(true)]
        //Gradient[] gradients = new Gradient[6];
        //颜色数组，默认6个，且是固定的
        [SerializeField]
        Gradient[] gradients;
        //大小数组，默认6个
        [SerializeField]
        AnimationCurve[] curves;
        [SerializeField]
        TextureUVCount[] uvCounts;

        [SerializeField]
        Material material;      //渲染用的材质
        /// <summary>        /// 当前循环到的组数，用来控制影响的组数        /// </summary>
        public int index = 0;

        [SerializeField]
        /// <summary>        /// 渲染时用到的图集        /// </summary>
        Texture2DArray textureArray;

        int colorsId = Shader.PropertyToID("_GradientColor"),
            alphasId = Shader.PropertyToID("_GradientAlpha"),
            sizesId = Shader.PropertyToID("_GradientSizes"),
            particlesBufferId = Shader.PropertyToID("_ParticlesBuffer"),
            groupsBufferId = Shader.PropertyToID("_GroupNodeBuffer"),
            timeId = Shader.PropertyToID("_Time");

        int kernel_PerFrame, kernel_PerFix;

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
            ParticleFactoryMenu factoryMenu = 
                Resources.Load<ParticleFactoryMenu>("ParticleFactory/ParticleFactoryMenu");
            compute = factoryMenu.compue;
            material = factoryMenu.material;
            textureArray = factoryMenu.textureArray;
            uvCounts = factoryMenu.uvCounts;

            kernel_PerFrame = compute.FindKernel("Particles_PerFrame");
            kernel_PerFix = compute.FindKernel("Particles_PerFixFrame");
            InitializeMode();       //先初始化所有加载模式
            InitializeParticle();   //再初始化粒子，以及传递数据到GPU
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

        private void Update()
        {
            compute.SetVector(timeId, new Vector4(Time.time, Time.deltaTime, Time.fixedDeltaTime));
            compute.SetBuffer(kernel_PerFrame, particlesBufferId, particlesBuffer);
            compute.SetBuffer(kernel_PerFrame, groupsBufferId, groupsBuffer);
            compute.Dispatch(kernel_PerFrame, particleGroupCount, 1, 1);
        }

        private void FixedUpdate()
        {
            compute.SetVector(timeId, new Vector4(Time.time, Time.deltaTime, Time.fixedDeltaTime));
            compute.SetBuffer(kernel_PerFix, particlesBufferId, particlesBuffer);
            compute.SetBuffer(kernel_PerFix, groupsBufferId, groupsBuffer);
            compute.Dispatch(kernel_PerFix, particleGroupCount, 1, 1);
        }

        /// <summary>        /// 初始化所有的条目，也就是颜色和大小        /// </summary>
        private void InitializeMode()
        {
            curves = new AnimationCurve[2];
            Keyframe keyframe = new Keyframe();
            //第一个，逐渐变大
            keyframe.time = 0; keyframe.value = 0; keyframe.inTangent = 2; keyframe.outTangent = 2;
            curves[0] = new AnimationCurve();
            curves[0].AddKey(keyframe);
            keyframe.time = 1; keyframe.value = 1; keyframe.inTangent = 0; keyframe.outTangent = 0;
            curves[0].AddKey(keyframe);
            //第二个，正态分布
            curves[1] = new AnimationCurve();
            keyframe.time = 0; keyframe.value = 0; keyframe.inTangent = 5; keyframe.outTangent = 5;
            curves[1].AddKey(keyframe);
            keyframe.time = 0.5f; keyframe.value = 1f; keyframe.inTangent = 0; keyframe.outTangent = 0;
            curves[1].AddKey(keyframe);
            keyframe.time = 1; keyframe.value = 0; keyframe.inTangent = -5; keyframe.outTangent = -5;
            curves[1].AddKey(keyframe);

            //添加颜色
            gradients = new Gradient[3];
            //添加第一个
            gradients[0] = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(); colorKeys[0].color = Color.white; colorKeys[0].time = 0;
            colorKeys[1] = new GradientColorKey(); colorKeys[1].color = Color.white; colorKeys[0].time = 1;
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];
            alphaKeys[0] = new GradientAlphaKey(); alphaKeys[0].alpha = 0; alphaKeys[0].time = 0;
            alphaKeys[1] = new GradientAlphaKey(); alphaKeys[1].alpha = 1; alphaKeys[1].time = 0.05f;
            alphaKeys[2] = new GradientAlphaKey(); alphaKeys[2].alpha = 1; alphaKeys[2].time = 0.95f;
            alphaKeys[3] = new GradientAlphaKey(); alphaKeys[3].alpha = 0; alphaKeys[3].time = 0f;
            gradients[0].SetKeys(colorKeys, alphaKeys);

            //添加第二个
            gradients[1] = new Gradient();
            colorKeys = new GradientColorKey[5]; 
            colorKeys[0] = new GradientColorKey(); colorKeys[0].color = new Color(4.0f, 0.6f, 0); 
            colorKeys[0].time = 0;
            colorKeys[1] = new GradientColorKey(); colorKeys[1].color = new Color(32.0f, 2.133f, 0);
            colorKeys[1].time = 0.18f;
            colorKeys[2] = new GradientColorKey(); colorKeys[2].color = new Color(29, 8f, 0);
            colorKeys[2].time = 0.5f;
            colorKeys[3] = new GradientColorKey(); colorKeys[3].color = new Color(25f, 4f, 0f);
            colorKeys[3].time = 0.8f;
            colorKeys[4] = new GradientColorKey(); colorKeys[4].color = new Color(20f, 2f, 0f);
            colorKeys[4].time = 1f;
            gradients[1].SetKeys(colorKeys, alphaKeys);     //透明度不变

            //添加第三个
            gradients[2] = new Gradient();
            alphaKeys = new GradientAlphaKey[3];
            alphaKeys[0] = new GradientAlphaKey(); alphaKeys[0].alpha = 1; alphaKeys[0].time = 0;
            alphaKeys[1] = new GradientAlphaKey(); alphaKeys[1].alpha = 1; alphaKeys[1].time = 0.95f;
            alphaKeys[2] = new GradientAlphaKey(); alphaKeys[2].alpha = 0; alphaKeys[2].time = 1f;
            gradients[2].SetKeys(colorKeys, alphaKeys);     //颜色值不变
        }

        /// <summary>        /// 初始化所有粒子        /// </summary>
        private void InitializeParticle()
        {
            particleNodes = new ParticleNodeData[particleGroupCount];
            //初始化组数据
            for (int i=0; i<particleNodes.Length; i++)
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

            NoiseParticleData[] noiseParticles = new NoiseParticleData[particleGroupCount * 64];
            particlesBuffer?.Release();
            particlesBuffer = new ComputeBuffer(particleGroupCount * 64, Marshal.SizeOf(noiseParticles[0]));
            for(int i=0; i< noiseParticles.Length; i++)
            {
                Vector4 random = new Vector4(Random.value, Random.value, Random.value, 0);
                noiseParticles[i] = new NoiseParticleData
                {
                    random = random,
                    index = Vector2Int.zero
                };
            }
            particlesBuffer.SetData(noiseParticles);

            //加载全部颜色
            Vector4[] colors = new Vector4[36];
            for(int i=0; i<6 && i < gradients.Length; i++)
            {
                GradientColorKey[] gradientColorKeys = gradients[i].colorKeys;
                for(int j=0; j< gradientColorKeys.Length && j < 6; j++)
                {
                    colors[i * 6 + j] = gradientColorKeys[j].color;
                    colors[i * 6 + j].w = gradientColorKeys[j].time;
                }
            }
            compute.SetVectorArray(colorsId, colors);
            //加载全部的透明度
            Vector4[] alphas = new Vector4[36];
            for (int i = 0; i < 6 && i < gradients.Length; i++)
            {
                GradientAlphaKey[] gradientAlphaKeys = gradients[i].alphaKeys;
                for (int j = 0; j < gradientAlphaKeys.Length && j < 6; j++)
                {
                    alphas[i * 6 + j] = new Vector4(gradientAlphaKeys[j].alpha,
                        gradientAlphaKeys[j].time);
                }
            }
            compute.SetVectorArray(alphasId, alphas);

            //加载全部的大小
            Vector4[] sizes = new Vector4[36];
            for (int i = 0; i < 6 && i < curves.Length; i++)
            {
                AnimationCurve curve = curves[i];
                for (int j = 0; j < curve.keys.Length && j < 6; j++)
                {
                    sizes[i * 6 + j] = new Vector4(curve.keys[j].time, curve.keys[j].value,
                        curve.keys[j].inTangent, curve.keys[j].outTangent);
                }
            }
            compute.SetVectorArray(sizesId, sizes);

        }

        /// <summary>        /// 渲染一组球形粒子在指定位置        /// </summary>
        public void DrawShape(ParticleDrawData drawData)
        {
            for(int i=0; i< drawData.groupCount; i++)
            {
                if (particleNodes[index].lifeTimeRange.z > Time.time)   //没有粒子可以释放
                    return;
                particleNodes[index].initEnum = new Vector3Int(1, drawData.useGravity ? 1 : 0, drawData.textureIndex);
                SetGroupData(index, drawData);
                index++; index %= particleGroupCount;
            }


        }

        /// <summary>        /// 在一个矩形中渲染粒子        /// </summary>
        /// <param name="cubeOffset">粒子对于Y和X的偏移值</param>
        public void DrawCube(ParticleDrawData drawData)
        {
            for (int i = 0; i < drawData.groupCount; i++)
            {
                if (particleNodes[index].lifeTimeRange.z > Time.time)   //没有粒子可以释放
                    return;
                //设置为cube模式
                particleNodes[index].initEnum = new Vector3Int(2, drawData.useGravity ? 1 : 0, drawData.textureIndex);
                SetGroupData(index, drawData);
                index++; index %= particleGroupCount;
            }
        }

        /// <summary>        /// 在点上渲染粒子        /// </summary>
        public void DrawPos(ParticleDrawData drawData)
        {
            for (int i = 0; i < drawData.groupCount; i++)
            {
                if (particleNodes[index].lifeTimeRange.z > Time.time)   //没有粒子可以释放
                    return;
                particleNodes[index].initEnum = new Vector3Int(0, 
                    drawData.useGravity ? 1 : 0, drawData.textureIndex);
                SetGroupData(index, drawData);
                index++; index %= particleGroupCount;
            }
        }


        /// <summary>
        /// 设置统一的粒子组数据，不包含初始化等设置, 要在前面先处理先，之后可能有其他内容
        /// </summary>
        /// <param name="index">设置的编号</param>
        /// <param name="drawData">设置的根据数据</param>
        private void SetGroupData(int index, ParticleDrawData drawData)
        {
            particleNodes[index].beginPos = drawData.beginPos;
            particleNodes[index].beginSpeed = drawData.beginSpeed;
            particleNodes[index].cubeRange = drawData.cubeOffset;
            particleNodes[index].sphereData = new Vector2(drawData.radian, drawData.radius);
            particleNodes[index].lifeTimeRange = new Vector3(drawData.liftTime
                - drawData.showTime, drawData.showTime, Time.time + drawData.liftTime);
            particleNodes[index].noiseData = new Vector3(drawData.frequency, drawData.octave, drawData.intensity);
            particleNodes[index].smoothRange = drawData.sizeRange;
            particleNodes[index].uvCount = new Vector2Int(uvCounts[drawData.textureIndex].rowCount,
                uvCounts[drawData.textureIndex].columnCount);
            particleNodes[index].drawData = new Vector2Int(drawData.colorIndex, drawData.sizeIndex);
            particleNodes[index].outEnum.x = drawData.followSpeed ? 1 : 0;
            groupsBuffer.SetData(particleNodes, index, index, 1);
        }


        public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
        {

            //material.SetBuffer(particlesBufferId, particlesBuffer);
            buffer.SetGlobalBuffer(particlesBufferId, particlesBuffer);
            buffer.SetGlobalBuffer(groupsBufferId, groupsBuffer);
            buffer.SetGlobalTexture("_Textures", textureArray);
            buffer.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Points,
                1, particlesBuffer.count);
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
}