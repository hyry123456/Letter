using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// UI场景，用来控制整一系列的UI，比如一整个画布,因此通常一张画布只有一个UIScene，
    /// 子类要获得其对应画布只需要查找父类组件即可
    /// </summary>
    public class UIControl : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>        /// UI的游戏对象字典        /// </summary>
        public Dictionary<string, GameObject> UIObjectDictionary;
        public delegate void Init();
        /// <summary>        /// 初始化函数，防止一些初始化不及时，这个初始化是在字典加载后调用的        /// </summary>
        public Init init;

        public delegate void SceneClick(PointerEventData eventData);
        private SceneClick sceneClick;


        /// <summary>    /// 初始化空间，还没到插入对象的时候    /// </summary>
        private void Awake()
        {
            UIObjectDictionary = new Dictionary<string, GameObject>();
        }

        private IEnumerator Start()
        {
            yield return null;
            ReadyAllUIWidget(this.transform);
            if(init != null)
            {
                init();
            }
            init = null;
        }

        /// <summary>
        /// 查找所有的侦测部件，会搜索所有的子组件,同时对于会变化的组件，
        /// 会进行取消显示，需要显示时在外部调用显示函数即可
        /// </summary>
        private void ReadyAllUIWidget(Transform transform)
        {
            UISceneWidgrt uiSceneWidgrt;
            uiSceneWidgrt = transform.GetComponent<UISceneWidgrt>();
            ISceneClickHandler pointClick = transform.GetComponent<ISceneClickHandler>();
            if(uiSceneWidgrt != null)
            {
                UIObjectDictionary.Add(transform.name, transform.gameObject);
                transform.gameObject.SetActive(false);
                //添加整个画布的点击事件
                sceneClick += pointClick.ScenePointClick;
            }
            int count = transform.childCount;
            for(int i=0; i<count; i++)
            {
                ReadyAllUIWidget(transform.GetChild(i));
            }
        }

        /// <summary>
        /// 画布受到点击时调用
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (sceneClick != null)
                sceneClick(eventData);
        }

        /// <summary>
        /// 添加场景点击时间，该函数用来给后来添加的组件调用场景点击事件
        /// </summary>
        /// <param name="scene"></param>
       public void AddSceneClick(SceneClick scene)
       {
            sceneClick += scene;
       }

        /// <summary>
        /// 用于给将UI移除时调用的方法，用来移除点击事件
        /// </summary>
        /// <param name="scene"></param>
        public void DeleteSceneClick(SceneClick scene)
        {
            sceneClick -= scene;
        }

    }
}