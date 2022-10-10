using System.Collections;
using UnityEngine;

namespace ScriptAnimate
{
    /// <summary>
    /// 脚本动画控制类，用来给一个物体添加脚本动画，
    /// 动画是通过ScriptObject提供的
    /// </summary>
    public class ScriptAnimateControl : MonoBehaviour
    {
        /// <summary>       /// 要移动到的全部点        /// </summary>
        public Transform[] points;
        /// <summary>       /// 要移动到的点需要时间    /// </summary>
        public float[] times;
        [SerializeField]
        /// <summary>     /// 所有需要执行的行为对象，可以赋值为空，表示无行为     /// </summary>
        public ScriptAnimateBase[] scripts;
        [SerializeField]
        private int nowIndex;
        private float nowRadio;
        [SerializeField]
        bool beginUse = true;

        private void Start()
        {
            nowIndex = 1; nowRadio = 0;
            if (!beginUse) return;
            scripts[0]?.BeginAnimate(this);
        }




        private void Update()
        {
            if (!beginUse) return;
            if (points == null || nowIndex >= points.Length) return;
            
            nowRadio += Time.deltaTime * (1.0f / times[0]);
            if(nowRadio > 1.0f)
            {
                if (scripts[nowIndex - 1] == null)  //为空直接播放下一个
                {
                    nowIndex++;
                    nowRadio = 0;
                    scripts[nowIndex - 1]?.BeginAnimate(this);
                    return;
                }
                else
                {
                    //可以结束就退出，否则继续
                    if (scripts[nowIndex - 1].EndAnimate(this))
                    {
                        nowIndex++;
                        nowRadio = 0;
                        scripts[nowIndex - 1]?.BeginAnimate(this);
                        return;
                    }
                    return;
                }

            }
            nowRadio = Mathf.Clamp01(nowRadio);
            transform.rotation = Quaternion.Lerp(points[nowIndex - 1].rotation,
                points[nowIndex].rotation, nowRadio);
            transform.position = Vector3.Lerp(points[nowIndex - 1].position,
                points[nowIndex].position, nowRadio);
        }

        /// <summary>        /// 绘制一下流程，方便提示        /// </summary>
        private void OnDrawGizmos()
        {
            if (points == null) return;

            for(int i=1; i <points.Length; i++)
            {
                Gizmos.color = (i % 2 == 1) ? Color.white : Color.green;
                Vector3 begin = points[i - 1] == null ? Vector3.zero : points[i - 1].position;
                Vector3 end = points[i] == null ? Vector3.zero : points[i].position;
                Gizmos.DrawLine(begin, end);
            }
        }

        public void BeginUse()
        {
            beginUse = true;
            scripts[0]?.BeginAnimate(this);
        }

        private void OnValidate()
        {
            if (beginUse)
            {
                BeginUse();
            }
        }
    }
}