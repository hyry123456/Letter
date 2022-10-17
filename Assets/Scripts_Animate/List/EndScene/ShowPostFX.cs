using UnityEngine;

namespace ScriptAnimate
{

    [CreateAssetMenu(menuName = "ScriptAnimate/EndScene/ShowPostFX")]
    public class ShowPostFX : ScriptAnimateBase
    {
        [SerializeField]
        DefferedRender.PostFXSetting fXSetting;
        float nowRadio;
        [SerializeField]
        float showTime = 4;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            nowRadio = 0;
            fXSetting.SetColorFilter(Color.black);
            Common.SustainCoroutine.Instance.AddCoroutine(AddColor);
        }

        bool AddColor()
        {
            nowRadio += Time.deltaTime * (1.0f / showTime);
            if(nowRadio >= 1.0f)
            {
                fXSetting.SetColorFilter(Color.white);
                return true;
            }
            fXSetting.SetColorFilter(Color.Lerp(Color.black, Color.white, nowRadio));
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if (nowRadio >= 1.0f)
                return true;
            return false;
        }
    }
}