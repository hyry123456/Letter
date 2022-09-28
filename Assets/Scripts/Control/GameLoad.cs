using UnityEngine;

namespace Control
{
    /// <summary>    
    /// 加载类，用来调用全部的加载方法，为了保证加载，每一个类场景都放一个，
    /// 这个类只是调用类，具体是否重复加载是看具体类实现的
    /// </summary>
    public class GameLoad : MonoBehaviour
    {
        private void Awake()
        {
            Common.SustainCoroutine sustain = Common.SustainCoroutine.Instance; //加载协程
            SceneChangeControl changeControl = SceneChangeControl.Instance;
            Task.AsynTaskControl.Instance.ReLoadTask();
            Application.targetFrameRate = -1;
        }
    }
}