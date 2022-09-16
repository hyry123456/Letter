using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>    /// 场景的组件管理类，用来用名称找到物体    /// </summary>
    public class SceneObjectMap 
    {
        private static SceneObjectMap instance;
        public static SceneObjectMap Instance
        {
            get
            {
                if(instance == null)
                    instance = new SceneObjectMap();
                return instance;
            }
        }
        private SceneObjectMap()
        {
            LoadAllObject();
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