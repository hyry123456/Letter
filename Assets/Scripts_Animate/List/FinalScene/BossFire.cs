using UnityEngine;
using Common;
using DefferedRender;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/FinalScene/BossFire")]
    public class BossFire : ScriptAnimateBase
    {
        BoxCollider begin;
        BoxCollider end;
        float nowRadio, fireTime = 10;
        bool canEnd = false;

        ParticleDrawData drawData = new ParticleDrawData
        {
            beginSpeed = Vector3.up * 10,
            speedMode = SpeedMode.PositionOutside,
            useGravity = false,
            followSpeed = false,
            lifeTime = 5,
            showTime = 5f,
            frequency = 1,
            octave = 8,
            intensity = 50,
            sizeRange = new Vector2(3, 8),
            colorIndex = ColorIndexMode.HighlightRedToBlue,
            sizeIndex = SizeCurveMode.SmallToBig_Epirelief,
            textureIndex = 1,
            groupCount = 15,
        };

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            canEnd = false;
            SustainCoroutine.Instance.AddCoroutine(GetBeginAndEnd);
        }
        bool GetBeginAndEnd()
        {
            begin = SceneObjectMap.Instance.FindControlObject("FireBeginPos").GetComponent<BoxCollider>();
            end = SceneObjectMap.Instance.FindControlObject("FireEndPos").GetComponent<BoxCollider>();
            nowRadio = 0;
            SustainCoroutine.Instance.AddCoroutine(FireBegin);
            return true;
        }

        bool FireBegin()
        {
            nowRadio += Time.deltaTime * (1.0f / fireTime);
            if(nowRadio >= 1.0f)
            {
                nowRadio = 0; canEnd = true;
                drawData.beginPos = end.transform.position;
                drawData.endPos = drawData.beginPos;
                drawData.cubeOffset = end.size;
                SustainCoroutine.Instance.AddCoroutine(SustainFire);
                return true;
            }
            drawData.beginPos = Vector3.Lerp(begin.transform.position,
                end.transform.position, nowRadio);
            drawData.endPos = drawData.beginPos + Vector3.one;
            drawData.cubeOffset = Vector3.Lerp(begin.size, end.size, nowRadio);
            ParticleNoiseFactory.Instance.DrawCube(drawData);
            return false;
        }

        bool SustainFire()
        {
            nowRadio += Time.deltaTime * (1.0f / 10f);
            if(nowRadio >= 1.0f)
            {
                return true;
            }
            ParticleNoiseFactory.Instance.DrawCube(drawData);
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if (canEnd)
                return true;
            return false;
        }
    }
}