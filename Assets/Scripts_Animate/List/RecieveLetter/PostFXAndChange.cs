using UnityEngine;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/RecieveLetter/PostFXAndChange")]
    /// <summary>    /// 进行后处理后切换场景    /// </summary>
    public class PostFXAndChange : ScriptAnimateBase
    {
        [SerializeField]
        DefferedRender.PostFXSetting fXSetting;
        float nowRadio;
        
        public float time = 5;
        public string TargetSceneName;
        private float begin;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            fXSetting.BeginFog();
            nowRadio = 1;
            begin = fXSetting.Fog.fogMaxDepth;
            Common.SustainCoroutine.Instance.AddCoroutine(FogAndBlack);
        }

        bool FogAndBlack()
        {
            nowRadio -= Time.deltaTime * (1.0f / time);
            if(nowRadio <= 0)
            {
                nowRadio = 0;
                fXSetting.SetMaxDepthFog(Mathf.Lerp(0, begin, nowRadio));
                fXSetting.SetColorFilter(Color.black);
                return true;
            }
            fXSetting.SetMaxDepthFog(Mathf.Lerp(0, begin, nowRadio));
            fXSetting.SetColorFilter(Color.Lerp(Color.black,  Color.white, nowRadio));
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if(nowRadio <= 0)
            {
                fXSetting.SetMaxDepthFog(begin);
                fXSetting.SetColorFilter(Color.white);
                Debug.Log("ChangeScene " + TargetSceneName);
                Control.SceneChangeControl.Instance.ChangeScene(TargetSceneName);

                return true;
            }
            return false;
        }
    }
}