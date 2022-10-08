using UnityEngine;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/RecieveLetter/MoveLetter")]
    /// <summary>
    /// 第一个场景播放移动信封的脚本
    /// </summary>
    public class Reci_Lett_MoveLetter : ScriptAnimateBase
    {
        private GameObject letter;
        private Transform moveBegin;
        private Transform moveEnd;
        public float moveTime = 2;
        private float nowRadio = 0;

        private GameObject papre;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            letter = Common.SceneObjectMap.Instance.FindControlObject("PapreObj");
            moveBegin = Common.SceneObjectMap.Instance.FindControlObject("PapreBegin").transform;
            moveEnd = Common.SceneObjectMap.Instance.FindControlObject("PapreEnd").transform;

            papre = Common.SceneObjectMap.Instance.FindControlObject("Canvas");

            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(MoveLetter, false);
        }

        bool MoveLetter()
        {
            nowRadio += Time.deltaTime * (1.0f / moveTime);
            if(nowRadio >= 1.0f)
            {
                //nowRadio = 1.0f;
                letter.transform.rotation = moveEnd.rotation;
                letter.transform.position = moveEnd.position;
                return true;
            }
            letter.transform.rotation = Quaternion.Lerp(moveBegin.rotation,
                moveEnd.rotation, nowRadio);
            letter.transform.position = Vector3.Lerp(moveBegin.position,
                moveEnd.position, nowRadio);

            return false;
        }


        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            //Debug.Log(nowRadio);
            //if (nowRadio > 1.0f) return true;
            //return false;
            if (!papre.activeSelf)
            {
                papre.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                papre.SetActive(false);
                return true;
            }
            return false;
        }
    }
}