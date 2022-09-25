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
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            Common.SustainCoroutine sustain = Common.SustainCoroutine.Instance; //����Э��
            //Common.SceneObjectMap objectMap = Common.SceneObjectMap.Instance;
<<<<<<< HEAD
            Application.targetFrameRate = -1;
            //Application.targetFrameRate = 60;
=======
            //Application.targetFrameRate = -1;
            Application.targetFrameRate = 60;
            //Debug.Log("60");
>>>>>>> 2d659d9c2c73c5dfcfcdc7aea19b3b69c273ac38
        }


    }
}