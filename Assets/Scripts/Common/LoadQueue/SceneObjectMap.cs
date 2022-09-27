using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>    
    /// 场景的组件管理类，用来用名称找到物体
    /// </summary>
    public class SceneObjectMap : MonoBehaviour
    {
        private static SceneObjectMap instance;
        public static SceneObjectMap Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject go = new GameObject("SceneObjectMap");
                    instance = go.AddComponent<SceneObjectMap>();
                }
                return instance;
            }
        }
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            LoadAllObject();
        }

        private void OnDestroy()
        {
            instance = null;
            objectMap.Clear();
        }

        const string controlName = "ControlObject";
        Dictionary<string, GameObject> objectMap;

        void LoadAllObject()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(controlName);
            objectMap = new Dictionary<string, GameObject>(objects.Length);
            for(int i=0; i<objects.Length; i++)
            {
                objectMap.Add(objects[i].name, objects[i]);
            }
        }

        public GameObject FindControlObject(string name)
        {
            GameObject obj = null;
            if (objectMap.TryGetValue(name, out obj)) { }
            return obj;
        }

        public void ReleaseObject()
        {
            objectMap.Clear();
        }
    }
}