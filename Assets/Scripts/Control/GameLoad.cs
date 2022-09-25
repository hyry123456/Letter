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
<<<<<<< HEAD
            Application.targetFrameRate = -1;
=======
            //Common.SceneObjectMap objectMap = Common.SceneObjectMap.Instance;

            Application.targetFrameRate = -1;

>>>>>>> 3709854a057de584b36aecb80a2d677bf9d33d80
        }


    }
}