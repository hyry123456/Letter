using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/RecieveLetter/MoveDoor")]
    /// <summary>    /// 移动门口动画    /// </summary>
    public class MoveDoor : ScriptAnimateBase
    {
        private GameObject door;
        private Transform moveBegin;
        private Transform moveEnd;

        public float moveTime = 2;
        private float nowRadio = 0;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            door = Common.SceneObjectMap.Instance.FindControlObject("DoorObj");
            moveBegin = Common.SceneObjectMap.Instance.FindControlObject("DoorBegin").transform;
            moveEnd = Common.SceneObjectMap.Instance.FindControlObject("DoorEnd").transform;

            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(MovesDoor, false);
        }

        bool MovesDoor()
        {
            nowRadio += Time.deltaTime * (1.0f / moveTime);
            if (nowRadio >= 1.0f)
            {
                //nowRadio = 1.0f;
                door.transform.rotation = moveEnd.rotation;
                door.transform.position = moveEnd.position;
                return true;
            }
            door.transform.rotation = Quaternion.Lerp(moveBegin.rotation,
                moveEnd.rotation, nowRadio);
            door.transform.position = Vector3.Lerp(moveBegin.position,
                moveEnd.position, nowRadio);

            return false;
        }


        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if (nowRadio >= 1.0)
                return true;
            return false;
        }
    }
}