
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{


    public class UICommon:MonoBehaviour
    {
        private static UICommon common;
        private UICommon(){ }

        /// <summary>
        /// UICommon对象的初始化，以及获得对象
        /// </summary>
        /// <returns>返回一个唯一对象</returns>
        public static UICommon Instance
        {
            get
            {
                if (common == null)
                {
                    //创健游戏对象
                    GameObject gameObject = new GameObject("UICommon");
                    common = gameObject.AddComponent<UICommon>();
                }
                return common;
            }
        }

        private void Start()
        {
            if(common == null)
                common = this.GetComponent<UICommon>();
        }

        /// <summary>
        /// 获取游戏对象
        /// </summary>
        public GameObject GetGameObject(string name, UIControl uIControl)
        {
            GameObject temp;
            if (uIControl == null)
            {
                Debug.Log("没有UIControl");
                return null;
            }
            uIControl.UIObjectDictionary.TryGetValue(name, out temp);
            return temp;
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="name">要显示的UI的名字</param>
        /// <param name="control">UI管理组件</param>
        public void ShowUI(string name, UIControl control)
        {
            control.UIObjectDictionary[name].SetActive(true);
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="name">要关闭显示的UI的名字</param>
        /// <param name="control">UI管理组件</param>
        public void CloseUI(string name, UIControl control)
        {
            GameObject gameObject;
            if(control.UIObjectDictionary.TryGetValue(name, out gameObject))
                gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置UI显示或者关闭，如果UI是显示状态就关闭，如果是关闭状态就显示
        /// </summary>
        public void SetUICloseOrClick(string name, UIControl control)
        {
            GameObject game = control.UIObjectDictionary[name];
            if(game == null)
            {
                Debug.Log("未找到UI" + name);
                return;
            }
            if (game.activeSelf)
            {
                game.SetActive(false);
            }
            else
                game.SetActive(true);
        }

        /// <summary>
        /// 对于后续生成的UI加入UIControl中管理的方法，防止后续添加的UI不能进行管理
        /// </summary>
        /// <param name="widgrt">UI管理部件</param>
        /// <param name="control">control组件</param>
        /// <param name="isShow">确定该组件是否显示，后续添加的物体不再受初始隐藏影响了，
        /// 所以我直接设置显示以及隐藏</param>
        public void LateUIAddControl(UISceneWidgrt widgrt, UIControl control, bool isShow)
        {
            if (widgrt == null) return;
            ISceneClickHandler sceneClickHandler = transform.GetComponent<ISceneClickHandler>();
            control.UIObjectDictionary.Add(widgrt.name, widgrt.gameObject);
            control.AddSceneClick(sceneClickHandler.ScenePointClick);
            widgrt.gameObject.SetActive(isShow);
        }

        /// <summary>
        /// 对于一些需要进行删除的动态UI进行删除方法
        /// </summary>
        /// <param name="widgrt">UI部件</param>
        /// <param name="control">UI控制组件</param>
        public void DeleteUIInControl(UISceneWidgrt widgrt, UIControl control)
        {
            if (control.UIObjectDictionary.ContainsKey(widgrt.name))
            {
                control.UIObjectDictionary.Remove(widgrt.name);
                ISceneClickHandler pointClick = widgrt.GetComponent<ISceneClickHandler>();
                control.DeleteSceneClick(pointClick.ScenePointClick);
            }
        }

    }
}