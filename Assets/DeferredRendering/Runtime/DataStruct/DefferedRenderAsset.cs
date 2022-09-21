using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefferedRender
{
    [System.Serializable]
    public struct RenderSetting
    {
        public bool allowHDR;
        public bool 
            useDynamicBatching,      //动态批处理
            useGPUInstancing,        //GPU实例化
            useSRPBatcher;          //SRP批处理
        public bool maskLight;      //是否遮罩灯光
        [Range(0.25f, 1f)]
        public float renderScale;          //渲染缩放

        [RenderingLayerMaskField]
        public int renderingLayerMask;

        public Shader cameraShader;

        public ClusterLightSetting clusterLightSetting;
    }

    /// <summary>
    /// Deffer Render Data Asset, Defind and input require data
    /// </summary>
    [CreateAssetMenu(menuName = "Rendering/Deffer Render Pipeline")]
    public class DefferedRenderAsset : RenderPipelineAsset
    {
        [SerializeField]
        RenderSetting renderSetting = new RenderSetting
        {
            allowHDR = false,
            useDynamicBatching = true,
            useGPUInstancing = true,
            useSRPBatcher = true,
            renderingLayerMask = -1,
            clusterLightSetting = new ClusterLightSetting
            {
                clusterCount = new Vector3Int(16, 16, 36),
                isUse = false,
            },
            renderScale = 1f,
        };

        /// <summary>	/// 阴影设置参数	/// </summary>
        [SerializeField]
        ShadowSetting shadows = default;

        [SerializeField]
        PostFXSetting postFXSetting = null;

        protected override RenderPipeline CreatePipeline()
        {
            return new DefferRenderPipeline(
                renderSetting, shadows, postFXSetting
            );
        }
    }
}