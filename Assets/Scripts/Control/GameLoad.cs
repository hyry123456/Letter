using UnityEngine;

namespace Common
{
    /// <summary>    
    /// 加载类，用来调用全部的加载方法，为了保证加载，每一个类场景都放一个，
    /// 这个类只是调用类，具体是否重复加载是看具体类实现的
    /// </summary>
    public class GameLoad : MonoBehaviour
    {
        private static GameLoad instance;
        public static GameLoad Instance => instance;
        private string sceneName;
        public string SceneName => sceneName;

        private void Awake()
        {
            instance = this;
            sceneName = Control.SceneChangeControl.Instance.GetRuntimeSceneName();

            SustainCoroutine sustain = SustainCoroutine.Instance; //加载协程
            Task.AsynTaskControl.Instance.ReLoadTask();
            Application.targetFrameRate = -1;
            SceneObjectMap objectMap = SceneObjectMap.Instance;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}