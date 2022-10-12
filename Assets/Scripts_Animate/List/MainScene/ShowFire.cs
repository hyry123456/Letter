using UnityEngine;

namespace ScriptAnimate
{

    [CreateAssetMenu(menuName = "ScriptAnimate/MainScene/ShowFire")]
    public class ShowFire : ScriptAnimateBase
    {

        DefferedRender.ParticleDrawData drawData = new DefferedRender.ParticleDrawData
        {
            beginSpeed = Vector3.up * 3,
            speedMode = DefferedRender.SpeedMode.JustBeginSpeed,
            useGravity = false,
            followSpeed = false,
            radius = 2,
            radian = 6.18f,
            lifeTime = 6,
            showTime = 5,
            frequency = 5,
            octave = 2,
            intensity = 1,
            sizeRange = new Vector2(1, 2),
            colorIndex = DefferedRender.ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = DefferedRender.SizeCurveMode.SmallToBig_Subken,
            textureIndex = 1,
            groupCount = 1,
        };

        [SerializeField]
        float nowRadio;
        public float stateTime = 2f;
        public float moveHeight = 15f;
        //用来判断是否结束
        bool isEnd;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            this.animateControl = animateControl;
            nowRadio = 0;
            isEnd = false;
            Common.SustainCoroutine.Instance.AddCoroutine(ShowFiresSphere);
        }

        ScriptAnimateControl animateControl;

        //一开始显示出火球
        bool ShowFiresSphere()
        {
            drawData.beginPos = animateControl.transform.position;
            nowRadio += Time.deltaTime * (1.0f / stateTime);
            if(nowRadio >= 1.0f)
            {
                nowRadio = 0;
                drawData.beginSpeed = Vector3.up;
                drawData.speedMode = DefferedRender.SpeedMode.PositionInside;
                Common.SustainCoroutine.Instance.AddCoroutine(MoveFiresSphere);
                return true;
            }
            DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
            return false;
        }

        bool MoveFiresSphere()
        {
            nowRadio += Time.deltaTime * (1.0f / stateTime);
            drawData.beginPos = Vector3.Lerp(animateControl.transform.position,
                Vector3.up * moveHeight + animateControl.transform.position, nowRadio);
            if(nowRadio >= 1.0f)
            {
                //渲染一个爆炸效果
                drawData.beginSpeed = Vector3.up * 30; drawData.intensity = 30;
                drawData.sizeRange = new Vector2(0.2f, 0.5f);
                drawData.speedMode = DefferedRender.SpeedMode.PositionOutside;
                drawData.groupCount = 50;
                drawData.textureIndex = 0;
                drawData.lifeTime = 5;
                drawData.showTime = 4;
                DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
                //设置回去
                drawData.sizeRange = new Vector2(1, 2); drawData.intensity = 1;
                drawData.beginSpeed = Vector3.up;
                drawData.groupCount = 1; drawData.textureIndex = 1;
                drawData.lifeTime = 6; drawData.showTime = 5;
                drawData.colorIndex = DefferedRender.ColorIndexMode.HighlightRedToBlue;
                nowRadio = 0;
                Common.SustainCoroutine.Instance.AddCoroutine(DeclineFiresSphere);
                return true;
            }
            return false;
        }

        bool DeclineFiresSphere()
        {
            nowRadio += Time.deltaTime * (1.0f / stateTime);
            drawData.beginPos = Vector3.Lerp(Vector3.up * moveHeight + animateControl.transform.position,
                animateControl.transform.position, nowRadio);
            if(nowRadio >= 1.0f)
            {
                isEnd = true;
                return true;
            }
            DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            return isEnd;
        }
    }
}