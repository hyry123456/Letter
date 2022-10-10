using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptAnimate
{

    [CreateAssetMenu(menuName = "ScriptAnimate/MainScene/MoveLetterAndFire")]
    public class MoveLetterAndFire : ScriptAnimateBase
    {
        public float moveTime = 2;
        GameObject letter;
        float nowRadio = 0;
        Transform begin;
        Transform end;
        Material material;


        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            letter = Resources.Load<GameObject>("Prefab/Effect/Letter");
            letter = GameObject.Instantiate(letter);


            if (material == null)
                material = letter.GetComponent<MeshRenderer>().material;
            material.SetFloat("_BlendBegin", 0f);

            GameObject game = Common.SceneObjectMap.Instance.FindControlObject("LetterMove");
            begin = game.transform.Find("MoveBeg");
            end = game.transform.Find("MoveEnd");

            letter.transform.position = begin.position;
            letter.transform.rotation = begin.rotation;

            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(MoveLetter);
        }


        //ÒÆ¶¯Ö½ÕÅ
        bool MoveLetter()
        {
            nowRadio += Time.deltaTime * (1.0f / moveTime);
            if(nowRadio >= 1.0f)
            {
                nowRadio = 1.0f;
                letter.transform.rotation = end.rotation;
                letter.transform.position = end.position;
                Common.SustainCoroutine.Instance.AddCoroutine(FireLetter);
                return true;
            }
            letter.transform.rotation = Quaternion.Lerp(begin.rotation, end.rotation, nowRadio);
            letter.transform.position = Vector3.Lerp(begin.position, end.position, nowRadio);
            return false;
        }

        DefferedRender.ParticleDrawData drawData = new DefferedRender.ParticleDrawData
        {
            beginSpeed = Vector3.up,
            speedMode = DefferedRender.SpeedMode.VerticalVelocityInside,
            useGravity = true,
            followSpeed = false,
            cubeOffset = new Vector3(0.1f, 0.1f, 2f),
            lifeTime = 2,
            showTime = 2,
            frequency = 5,
            octave = 2,
            intensity = 1,
            sizeRange = new Vector2(0.1f, 0.4f),
            colorIndex = (int)DefferedRender.ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = (int)DefferedRender.SizeCurveMode.SmallToBig_Subken,
            textureIndex = 1,
            groupCount = 1,
        };



        //È¼ÉÕÖ½ÕÅ
        bool FireLetter()
        {
            nowRadio += Time.deltaTime * (1.0f / moveTime);

            if(nowRadio >= 2.0f)
            {
                material.SetFloat("_BlendBegin", 1.0f);
                return true;
            }
            float parRadio = (nowRadio - 1.35f) / 0.35f;
            if(parRadio > 0f && parRadio < 1f)
            {
                Vector3 letterPos = letter.transform.position;
                letterPos = Vector3.Lerp(letterPos - Vector3.up * 1f,
                    letterPos + Vector3.up * 1.6f, parRadio);
                drawData.beginPos = letterPos - Vector3.forward * 1.2f;
                drawData.endPos = letterPos + Vector3.forward * 1.2f;
                DefferedRender.ParticleNoiseFactory.Instance.DrawCube(drawData);
            }

            material.SetFloat("_BlendBegin", nowRadio - 1.0f);
            return false;
        }




        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if (nowRadio >= 2.0)
                return true;
            return false;
        }
    }
}