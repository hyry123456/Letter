using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{
    public enum TextureMode
    {
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024
    }

    enum Pass
    {
        Copy = 0,
    }

    [System.Serializable]
    public struct Triangle
    {
        public Vector3 position0;
        public Vector3 position1;
        public Vector3 position2;

        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
    };


    [ExecuteInEditMode]
    public class TerrainDraw : GPUDravinBase
    {
        private bool isInsert;

        public Mesh planeMesh;

        public Shader showShader;
        public Material showMat;
        public Material ShowMat
        {
            get
            {
                if (showShader == null) return null;
                if (showMat == null || showMat.shader == showShader)
                    showMat = new Material(showShader);
                return showMat;
            }
        }

        private ComputeBuffer specularBuffer;
        private ComputeBuffer terrainDataBuffer;


        [Range(2, 50)]
        public int tessCount = 2;
        [Range(1, 60)]
        public int tessDegree = 1;
        [Min(0.0f)]
        public float tessMinDistance = 10;
        [Min(0.0f)]
        public float tessMaxDistance = 100;
        [Min(0.0f)]
        public float terrainSize = 1000;

        #region VisualTexSetting

        public Texture2D normalTex;
        public RenderTexture heightmapTex;
        public Terrain terrain;

        public TextureMode visualTexMode = TextureMode._128;
        private Texture2DArray visualTex_Diffuse;
        private Texture2DArray visualTex_Normal;
        private Texture2DArray visualTex_Mask;
        private Texture2DArray visualTex_Splat;

        public bool isDebug = false;

        #endregion

        private void OnEnable()
        {
            if(!gameObject.activeSelf || !this.enabled) return;
            GPUDravinDrawStack.Instance.InsertRender(this);
            isInsert = true;
            //terrain = GetComponent<Terrain>();
            //ReadyBuffer();
            CreateMesh();
            GetVisualTexture();
            SetNormal();
            ReadyBuffer();
        }

        private void OnValidate()
        {

            //terrain = GetComponent<Terrain>();
            CreateMesh();
            ReadyBuffer();
            GetVisualTexture();
        }

        private void OnDisable()
        {
            if (normalTex != null)
                DestroyImmediate(normalTex);

            if (isInsert)
            {
                GPUDravinDrawStack.Instance.RemoveRender(this);
                isInsert = false;
            }
            //clipResultBuffer?.Dispose();
            //argsBuffer?.Dispose();
            specularBuffer?.Dispose();
            terrainDataBuffer?.Dispose();
        }

        private void OnDestroy()
        {
            if (normalTex != null)
                DestroyImmediate(normalTex);

            if (isInsert)
            {
                GPUDravinDrawStack.Instance.RemoveRender(this);
                isInsert = false;
            }

            specularBuffer?.Dispose();
            terrainDataBuffer?.Dispose();

            //createMeshBuffer?.Dispose();
            //clipResultBuffer?.Dispose();
            //argsBuffer?.Dispose();
        }

        private void SetNormal()
        {
            if (normalTex != null) DestroyImmediate(normalTex);
            normalTex = new Texture2D(heightmapTex.width, heightmapTex.height, TextureFormat.RGBA32, -1, true);
            var colors = new Color[heightmapTex.width * heightmapTex.width];
            int index = 0;
            for (int i = 0; i < heightmapTex.width; i++)
                for (int j = 0; j < heightmapTex.height; j++)
                {
                    var normal = terrain.terrainData.GetInterpolatedNormal(
                        (float)j / heightmapTex.width, (float)i / heightmapTex.height);
                    colors[index++] = new Color(normal.x * 0.5f + 0.5f, normal.y * 0.5f + 0.5f, normal.z * 0.5f + 0.5f);
                }
            normalTex.SetPixels(colors);
            normalTex.Apply();
        }

        private void ReadyBuffer()
        {
            specularBuffer?.Release();
            terrainDataBuffer?.Release();
            int length = terrain.terrainData.terrainLayers.Length;
            Color[] speculars = new Color[length];
            Vector3[] datas = new Vector3[length];
            for(int i=0; i< length; i++)
            {
                speculars[i] = terrain.terrainData.terrainLayers[i].specular;
                datas[i] = new Vector3(terrain.terrainData.terrainLayers[i].normalScale,
                    terrain.terrainData.terrainLayers[i].metallic,
                    terrain.terrainData.terrainLayers[i].smoothness);
            }
            specularBuffer = new ComputeBuffer(length, sizeof(float) * 4);
            specularBuffer.SetData(speculars);
            terrainDataBuffer = new ComputeBuffer(length, sizeof(float) * 3);
            terrainDataBuffer.SetData(datas);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if(meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = planeMesh;
            }
            if(meshFilter.sharedMesh != planeMesh)
                meshFilter.sharedMesh = planeMesh;
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if(renderer == null)
            {
                renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = ShowMat;
            }
            if(renderer.sharedMaterial != ShowMat)
                renderer.sharedMaterial = ShowMat;
        }

        private void GetVisualTexture()
        {
            heightmapTex = terrain.terrainData.heightmapTexture;
            Texture2D[] textures_Diffuse = new Texture2D[terrain.terrainData.terrainLayers.Length];
            for (int i = 0; i < textures_Diffuse.Length; i++)
            {
                textures_Diffuse[i] = terrain.terrainData.terrainLayers[i].diffuseTexture;
            }
            visualTex_Diffuse = TextureArrayCreate.CreateTextureArrayBySet(textures_Diffuse);

            Texture2D[] textures_Normal = new Texture2D[terrain.terrainData.terrainLayers.Length];
            for (int i = 0; i < textures_Diffuse.Length; i++)
            {
                Texture2D temp = terrain.terrainData.terrainLayers[i].normalMapTexture;
                if (temp == null) temp = TextureArrayCreate.CreateTexture2D(
                    terrain.terrainData.terrainLayers[i].diffuseTexture.height, Color.white);
                textures_Normal[i] = temp;
            }
            visualTex_Normal = TextureArrayCreate.CreateTextureArrayBySet(textures_Normal);

            Texture2D[] textures_Mask = new Texture2D[terrain.terrainData.terrainLayers.Length];
            for (int i = 0; i < textures_Diffuse.Length; i++)
            {
                Texture2D temp = terrain.terrainData.terrainLayers[i].maskMapTexture;
                if (temp == null) temp = TextureArrayCreate.CreateTexture2D(
                    terrain.terrainData.terrainLayers[i].diffuseTexture.height, Color.white);
                textures_Mask[i] = temp;
            }
            visualTex_Mask = TextureArrayCreate.CreateTextureArrayBySet(textures_Mask);

            Texture2D[] textures_Spilt = new Texture2D[terrain.terrainData.alphamapTextures.Length];
            for (int i = 0; i < textures_Spilt.Length; i++)
            {
                textures_Spilt[i] = terrain.terrainData.alphamapTextures[i];
            }
            visualTex_Splat = TextureArrayCreate.CreateTextureArrayBySet(textures_Spilt);

        }

        private void CreateMesh()
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            Vector3 begin = transform.position;
            for (int i = 0; i < this.tessCount; i++)
            {
                for (int j = 0; j < this.tessCount; j++)
                {
                    verts.Add(begin + new Vector3(i / (tessCount - 1.0f) * terrainSize,
                        0, j / (tessCount - 1.0f) * terrainSize));
                    uvs.Add(new Vector2(i / (tessCount - 1.0f), j / (tessCount - 1.0f)));
                    if (i == 0 || j == 0)
                        continue;
                    tris.Add(tessCount * i + j);
                    tris.Add(tessCount * i + j - 1);
                    tris.Add(tessCount * (i - 1) + j - 1);
                    tris.Add(tessCount * (i - 1) + j - 1);
                    tris.Add(tessCount * (i - 1) + j);
                    tris.Add(tessCount * i + j);
                }
            }


            if (planeMesh)
                DestroyImmediate(planeMesh);
            planeMesh = new Mesh();
            planeMesh.vertices = verts.ToArray();
            planeMesh.uv = uvs.ToArray();
            planeMesh.triangles = tris.ToArray();
            planeMesh.RecalculateNormals();
            Bounds bounds = new Bounds(new Vector3(terrainSize / 2, 0, terrainSize / 2), Vector3.one * terrainSize);
            planeMesh.bounds = bounds;
        }


        public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
        {

        }

        public override void DrawByProjectMatrix(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Matrix4x4 projectMatrix)
        {
            Material material = ShowMat;
            if (material == null || planeMesh == null) return;

            //buffer.DrawMesh(planeMesh, Matrix4x4.identity, material, 0, 1);
            //ExecuteBuffer(ref buffer, context);
            return;
        }

        public override void DrawOtherSSS(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
        {
            return;
        }

        public override void DrawPreSSS(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
        {
            if (isDebug)
            {
                //GetVisualTexture();
                Texture2D[] textures_Spilt = new Texture2D[terrain.terrainData.alphamapTextures.Length];
                for (int i = 0; i < textures_Spilt.Length; i++)
                {
                    textures_Spilt[i] = terrain.terrainData.alphamapTextures[i];
                }
                visualTex_Splat = TextureArrayCreate.CreateTextureArrayBySet(textures_Spilt);
            }

            Material material = ShowMat;
            if (material == null || planeMesh == null) return;


            //buffer.DrawMesh(planeMesh, Matrix4x4.identity, material, 0, 0);
            //ExecuteBuffer(ref buffer, context);
        }

        public override void SetUp(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
        {
            buffer.SetGlobalFloat("_TessDegree", tessDegree);
            buffer.SetGlobalTexture("_HeightTex", heightmapTex);
            buffer.SetGlobalFloat("_Height",
                terrain.terrainData.heightmapScale.y * 2);
            buffer.SetGlobalFloat("_TessDistanceMax", tessMaxDistance);
            buffer.SetGlobalFloat("_TessDistanceMin", tessMinDistance);
            buffer.SetGlobalTexture("_NormalTex", normalTex);
            buffer.SetGlobalTexture("_VTexture_Diffuse", visualTex_Diffuse);
            buffer.SetGlobalTexture("_VTexture_Normal", visualTex_Normal);
            buffer.SetGlobalTexture("_VTexture_Mask", visualTex_Mask);
            buffer.SetGlobalTexture("_VTexture_Spilt", visualTex_Splat);
            buffer.SetGlobalInt("_TextureCount", terrain.terrainData.terrainLayers.Length);
            buffer.SetGlobalBuffer("_SpecularBuffer", specularBuffer);
            buffer.SetGlobalBuffer("_TerrainDataBuffer", terrainDataBuffer);

            //Vector4[] clipPlane = GetFrustumPlane(Camera.main);
            //buffer.SetGlobalVectorArray("_ClipPlane", clipPlane);

            ExecuteBuffer(ref buffer, context);

            return;
        }

    }
}