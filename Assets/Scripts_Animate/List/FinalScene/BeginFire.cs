using UnityEngine;
using DefferedRender;
using Common;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/FinalScene/BeginFire")]
    public class BeginFire : ScriptAnimateBase
    {
        GameObject childe;
        ParticleDrawData drawData = new ParticleDrawData
        {
            beginSpeed = Vector3.up * 10,
            useGravity = false,
            followSpeed = false,
            cubeOffset = Vector3.one,
            radian = 6.18f,
            radius = 10f,
            lifeTime = 2,
            showTime = 2f,
            frequency = 1,
            octave = 8,
            intensity = 10,
            sizeRange = new Vector2(1, 5),
            colorIndex = ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = SizeCurveMode.SmallToBig_Epirelief,
            textureIndex = 1,
        };

        Vector3 targetPos, beginPos;
        float nowRadio = 0;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            SustainCoroutine.Instance.AddCoroutine(Begin);
        }
        bool Begin()
        {
            childe = SceneObjectMap.Instance.FindControlObject("AnimateChild");
            //获得主角，播放一次爆炸燃烧
            drawData.beginPos = childe.transform.position;
            drawData.endPos = drawData.beginPos + Vector3.up * 3;
            drawData.speedMode = SpeedMode.PositionOutside;
            drawData.groupCount = 15;
            ParticleNoiseFactory.Instance.DrawCube(drawData);

            drawData.groupCount = 1;
            drawData.speedMode = SpeedMode.JustBeginSpeed;
            targetPos = SceneObjectMap.Instance.FindControlObject("ChildeTargetPos").transform.position;
            beginPos = childe.transform.position;

            nowRadio = 0;
            SustainCoroutine.Instance.AddCoroutine(SustainFire);
            return true;
        }

        //持续燃烧少爷，且移动少爷
        bool SustainFire()
        {
            nowRadio += Time.deltaTime;
            if(nowRadio >= 1.0f)
            {
                drawData.speedMode = SpeedMode.PositionOutside;
                drawData.groupCount = 15;
                ParticleNoiseFactory.Instance.DrawShape(drawData);
                SceneObjectMap.Instance.FindControlObject("ChildeTargetPos").SetActive(false);
                return true;
            }
            childe.transform.position = Vector3.Lerp(beginPos, targetPos, nowRadio);
            drawData.beginPos = childe.transform.position;
            drawData.endPos = drawData.beginPos + Vector3.up * 3;
            ParticleNoiseFactory.Instance.DrawCube(drawData);
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