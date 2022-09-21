using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{
    public class ParticleNoise : GPUDravinBase
    {
        public ComputeShader computeShader;
        private ComputeBuffer particleBuffer;
        private ComputeBuffer initializeBuffer;
        public NoiseData[] noiseDatas;

        //初始化
        [SerializeField]
        private int particleCount = 1000;   //每组粒子数量
        [SerializeField]
        private int particleOutCount = 100; //每秒输出数量
        public bool runInUpdate = true;
        public bool isUse = true;

        public int ParticleOutCount
        {
            get { return particleOutCount; }
        }

        public Material copyMat;
        private Material mat;
        public Material ShowMat
        {
            get
            {
                if (mat == null || mat.shader != particleShader)
                {
                    mat = new Material(particleShader);
                    mat.CopyPropertiesFromMaterial(copyMat);
                    mat.SetTexture(mainTexId, mainTexture);
                }
                return mat;
            }
        }

        //输出、着色
        [SerializeField]
        private int rowCount = 1;
        [SerializeField]
        private int colCount = 1;
        [SerializeField]
        Shader particleShader;
        [SerializeField]
        private AnimationCurve sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField, Range(0.1f, 10f)]
        private float particleSize = 1;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient colorWithLive;
        [SerializeField]
        private Texture mainTexture;

        private int kernel_Updata;
        private int kernel_FixUpdata;
        private bool isInsert;
        public uint arrive;
        private float arriveF = 0;
        public ParticleInitializeData[] init;

        private int
            timeId = Shader.PropertyToID("_Time"),
            uvCountId = Shader.PropertyToID("_UVCount"),
            rowCountId = Shader.PropertyToID("_RowCount"),
            colCountId = Shader.PropertyToID("_ColCount"),
            colorsId = Shader.PropertyToID("_Colors"),
            alphasId = Shader.PropertyToID("_Alphas"),
            particleCountId = Shader.PropertyToID("_ParticleCount"),
            sizesId = Shader.PropertyToID("_Sizes"),
            particleSizeId = Shader.PropertyToID("_ParticleSize"),
            mainTexId = Shader.PropertyToID("_MainTex"), 
            texAspectRatioId = Shader.PropertyToID("_TexAspectRatio"), 
            particleBufferId = Shader.PropertyToID("_ParticleNoiseBuffer"),
            initBufferId = Shader.PropertyToID("_InitializeBuffer");

        Vector3 GetSphereBeginPos(Vector4 random, float arc, float radius, Transform matrix4X4)
        {
            float u = Mathf.Lerp(0, arc, random.x);
            float v = Mathf.Lerp(0, arc, random.y);
            Vector3 pos = new Vector3(radius * Mathf.Cos(u),
                radius * Mathf.Sin(u) * Mathf.Cos(v), radius * Mathf.Sin(u) * Mathf.Sin(v));
            return matrix4X4.TransformPoint(pos);
        }

        Vector3 GetCubeBeginPos(Vector4 random, Vector3 cubeRange, Transform matrix4X4)
        {
            Vector3 begin = -cubeRange / 2.0f;
            Vector3 end = cubeRange / 2.0f;
            Vector3 pos = new Vector3(
                Mathf.Lerp(begin.x, end.x, random.x),
                Mathf.Lerp(begin.y, end.y, random.y),
                Mathf.Lerp(begin.z, end.z, random.z)
                );
            return matrix4X4.TransformPoint(pos) ;
        }

        private void Awake()
        {
            if (computeShader == null || ShowMat == null) return;
            kernel_Updata = computeShader.FindKernel("Noise_PerFrame");
            kernel_FixUpdata = computeShader.FindKernel("Noise_PerFixFrame");
        }

        private void OnEnable()
        {
            GPUDravinDrawStack.Instance.InsertRender(this);
            isInsert = true;
            arrive = 0;

            ReadyBuffer();
            SetUnUpdateData();
        }

        private void OnDisable()
        {
            if (isInsert)
            {
                GPUDravinDrawStack.Instance.RemoveRender(this);
                isInsert = false;
            }
            particleBuffer?.Release();
            initializeBuffer?.Release();
        }

        private void Update()
        {
            if (isUse)
            {
                if (runInUpdate)
                {
                    arriveF += Time.deltaTime;
                    uint add = (uint)(arriveF / (1.0f / particleOutCount));
                    if (add > 0)
                    {
                        arrive += add;
                        arriveF = 0;
                        arrive %= 1000000007;
                    }
                }

                UpdateInitial();

                SetUpdateData();
                computeShader.Dispatch(kernel_Updata, noiseDatas.Length, (particleCount / 64) + 1, 1);
            }
        }

        private void FixedUpdate()
        {
            if (isUse)
            {
                SetFixUpdateData();
                computeShader.Dispatch(kernel_FixUpdata, noiseDatas.Length, (particleCount / 64) + 1, 1);
            }
        }

        private void OnValidate()
        {
            mat = null;

            if (isInsert)
            {
                SetUnUpdateData();
                ReadyInitialParticle();
            }
        }

        private void ReadyBuffer()
        {
            ReadyPerParticle();
            ReadyInitialParticle();
        }

        /// <summary>        /// 初始化每一个粒子的数据        /// </summary>
        private void ReadyPerParticle()
        {
            particleBuffer?.Release();
            particleBuffer = new ComputeBuffer(particleCount * noiseDatas.Length, 
                sizeof(float) * (4 + 2 + 3 + 4 + 1 + 4 + 1 + 3 + 1));
            NoiseParticleData[] noiseParticleData = new NoiseParticleData[particleCount * noiseDatas.Length];
            for(int j=0; j< noiseDatas.Length; j++)
            {
                NoiseData noiseData = noiseDatas[j];
                for (int i = 0; i < particleCount; i++)
                {
                    Vector4 random = new Vector4(Random.value, Random.value, Random.value, 0);
                    Vector3 pos = Vector3.zero;
                    switch (noiseData.shapeMode)
                    {
                        case InitialShapeMode.Sphere:
                            pos = GetSphereBeginPos(random, noiseData.arc, 
                                noiseData.radius, noiseData.position); break;
                        case InitialShapeMode.Cube:
                            pos = GetCubeBeginPos(random, 
                                noiseData.cubeRange, noiseData.position); break;
                    }

                    Vector3 speed = new Vector3(
                        Mathf.Lerp(noiseData.velocityBegin.x, noiseData.velocityEnd.x, random.y),
                        Mathf.Lerp(noiseData.velocityBegin.y, noiseData.velocityEnd.y, random.z),
                        Mathf.Lerp(noiseData.velocityBegin.z, noiseData.velocityEnd.z, random.x)
                    );
                    noiseParticleData[j * particleCount + i] = new NoiseParticleData
                    {
                        random = random,
                        index = new Vector2Int(i , -1),
                        worldPos = pos,
                        liveTime = Mathf.Lerp(noiseData.lifeTime.x, noiseData.lifeTime.y, Random.value),
                        nowSpeed = speed,
                    };
                }
            }

            particleBuffer.SetData(noiseParticleData);
            arrive = 0;
        }

        /// <summary>        /// 加载每一组粒子的初始化数据        /// </summary>
        private void ReadyInitialParticle()
        {
            initializeBuffer?.Dispose();
            initializeBuffer = new ComputeBuffer(noiseDatas.Length, 
                sizeof(float) * (44));
            init = new ParticleInitializeData[noiseDatas.Length];
            for(int i=0; i< noiseDatas.Length; i++)
            {
                NoiseData noiseData = noiseDatas[i];
                Vector3Int initEnum = Vector3Int.zero;
                initEnum.x = (int)noiseData.shapeMode;
                Vector2 sphere = new Vector2(noiseData.arc, noiseData.radius);
                Vector3 cube = noiseData.cubeRange;
                Matrix4x4 matrix4X4 = noiseData.position.localToWorldMatrix;
                Vector3 noise = 
                    new Vector3(noiseData.octave, noiseData.frequency, noiseData.intensity);
                Vector3Int outEnum = Vector3Int.zero;
                outEnum.x = (noiseData.isSizeBySpeed) ? (int)noiseData.sizeBySpeedMode : 0;

                init[i] = new ParticleInitializeData
                {
                    beginPos = noiseData.position.position,
                    velocityBeg = noiseData.velocityBegin,
                    velocityEnd = noiseData.velocityEnd,
                    InitEnum = initEnum,
                    sphereData = sphere,
                    cubeRange = cube,
                    transfer_M = matrix4X4,
                    lifeTimeRange = noiseData.lifeTime,
                    noiseData = noise,
                    outEnum = outEnum,
                    smoothRange = noiseData.smoothRange,
                    arriveIndex = 0,
                };
            }

            initializeBuffer.SetData(init);
        }

        private void UpdateInitial()
        {
            if (initializeBuffer == null || init == null) return;
            if (runInUpdate)
            {
                for (int i = 0; i < init.Length; i++)
                {
                    init[i].arriveIndex = arrive;
                }
            }
            initializeBuffer.SetData(init);
        }

        private void SetUnUpdateData()
        {
            computeShader.SetInts(uvCountId, new int[2] { rowCount, colCount });
            computeShader.SetInt(particleCountId, particleCount);

            Vector4[] sizes = new Vector4[sizeCurve.length];
            for (int i = 0; i < sizeCurve.length; i++)
            {
                sizes[i] = new Vector4(sizeCurve.keys[i].time, sizeCurve.keys[i].value,
                    sizeCurve.keys[i].inTangent, sizeCurve.keys[i].outTangent);
            }
            computeShader.SetVectorArray(sizesId, sizes);

            GradientAlphaKey[] gradientAlphas = colorWithLive.alphaKeys;
            Vector4[] alphas = new Vector4[gradientAlphas.Length];
            for (int i = 0; i < gradientAlphas.Length; i++)
            {
                alphas[i] = new Vector4(gradientAlphas[i].alpha,
                    gradientAlphas[i].time);
            }
            computeShader.SetVectorArray(alphasId, alphas);

            GradientColorKey[] gradientColorKeys = colorWithLive.colorKeys;
            Vector4[] colors = new Vector4[gradientColorKeys.Length];
            for (int i = 0; i < gradientColorKeys.Length; i++)
            {
                colors[i] = gradientColorKeys[i].color;
                colors[i].w = gradientColorKeys[i].time;
            }
            computeShader.SetVectorArray(colorsId, colors);

            ShowMat.SetTexture(mainTexId, mainTexture);
            ShowMat.SetInt(rowCountId, rowCount);
            ShowMat.SetInt(colCountId, colCount);
            float ratio = (mainTexture == null)? 1 : (float)mainTexture.width / mainTexture.height;
            ShowMat.SetFloat(texAspectRatioId, ratio);
            ShowMat.SetFloat(particleSizeId, particleSize);
        }

        private void SetUpdateData()
        {
            computeShader.SetBuffer(kernel_Updata, particleBufferId, particleBuffer);
            computeShader.SetBuffer(kernel_Updata, initBufferId, initializeBuffer);

            //设置时间
            computeShader.SetVector(timeId, new Vector4(Time.time, Time.deltaTime, Time.fixedDeltaTime));

            if (runInUpdate)
            {
                for (int i = 0; i < init.Length; i++)
                {
                    init[i].arriveIndex = arrive;
                    if (noiseDatas[i].isPhysical)
                    {
                        init[i].beginPos = noiseDatas[i].position.position;
                        init[i].transfer_M = noiseDatas[i].position.localToWorldMatrix;
                    }
                }
                initializeBuffer.SetData(init);
            }
        }

        private void SetFixUpdateData()
        {
            computeShader.SetBuffer(kernel_FixUpdata, particleBufferId, particleBuffer);
            computeShader.SetBuffer(kernel_FixUpdata, initBufferId, initializeBuffer);
            ShowMat.SetTexture(mainTexId, mainTexture);

            //设置时间
            computeShader.SetVector(timeId, new Vector4(Time.time, Time.deltaTime, Time.fixedDeltaTime));
        }

        public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
        {
            if (isUse)
            {
                ShowMat.SetBuffer(particleBufferId, particleBuffer);
                buffer.DrawProcedural(Matrix4x4.identity, ShowMat, 0, MeshTopology.Points,
                    1, particleCount * noiseDatas.Length);
                ExecuteBuffer(ref buffer, context);
            }
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
        }
    }
}