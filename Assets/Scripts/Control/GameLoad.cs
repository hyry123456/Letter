using UnityEngine;

namespace Control
{
    /// <summary>    /// 加载类，用来调用全部的加载方法    /// </summary>
    public class GameLoad : MonoBehaviour
    {
        private static GameLoad instance;
        public static GameLoad Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject gameObject = new GameObject("GameLoad");
                    gameObject.AddComponent<GameLoad>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            Common.SustainCoroutine sustain = Common.SustainCoroutine.Instance; //加载协程
            //Common.SceneObjectMap objectMap = Common.SceneObjectMap.Instance;

            Application.targetFrameRate = -1;

        }


    }
}