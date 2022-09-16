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
            useDynamicBatching,      //��̬������
            useGPUInstancing,        //GPUʵ����
            useSRPBatcher;          //SRP������
        public bool maskLight;      //�Ƿ����ֵƹ�

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
    };

        /// <summary>	/// ��Ӱ���ò���	/// </summary>
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