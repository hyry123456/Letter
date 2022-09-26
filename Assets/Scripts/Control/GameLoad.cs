using UnityEngine;

namespace Control
{
    /// <summary>    /// �����࣬��������ȫ���ļ��ط���    /// </summary>
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
            if (instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            Common.SustainCoroutine sustain = Common.SustainCoroutine.Instance; //����Э��
            SceneChangeControl changeControl = SceneChangeControl.Instance;
            Task.AsynTaskControl.Instance.ReLoadTask();

            Application.targetFrameRate = -1;

        }
    }
}