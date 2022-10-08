
using UnityEngine;

namespace ScriptAnimate
{
    [System.Serializable]
    public abstract class ScriptAnimateBase : ScriptableObject
    {
        /// <summary>
        /// 动画播放结束时进行的行为，用来判断是否需要切换到下一行为
        /// </summary>
        /// <param name="animateControl">控制器对象，用来获取游戏对象</param>
        /// <returns>是否退出，是就会进行下一行为，否则会在下一帧再调用一次</returns>
        public abstract bool EndAnimate(ScriptAnimateControl animateControl);

        /// <summary>      /// 动画播放开始时执行的行为，用来进行行为的初始化      /// </summary>
        /// <param name="animateControl">控制器对象</param>
        public abstract void BeginAnimate(ScriptAnimateControl animateControl);
    }
}