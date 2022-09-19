using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{

    /// <summary>
    /// UI框架使用的基类，所有的侦测实现类都要继承该类
    /// </summary>
    public class UIUseBase : MonoBehaviour
    {
        /// <summary>        /// 侦测组件        /// </summary>
        protected UISceneWidgrt widgrt;
        /// <summary>        /// UI的管理组件        /// </summary>
        protected UIControl control;

        /// <summary>
        /// 默认初始化方法，加载侦测组件以及UI的管理组件,同时停止本UI显示
        /// </summary>
        protected virtual void Awake()
        {
            //加载组件的标准操作
            widgrt = GetComponent<UISceneWidgrt>();
            if (widgrt == null)
            {
                widgrt = this.gameObject.AddComponent<UISceneWidgrt>();
            }
            LoadControl();
        }

        /// <summary>
        /// 加载UIControl的方法，提取出来是因为有一些物体并没有一开始就设置在正确位置
        /// 所以需要后面调用
        /// </summary>
        public void LoadControl()
        {
            Transform parent = transform.parent;
            Transform preParent = transform;
            while (parent != null)
            {
                preParent = parent;
                parent = parent.parent;
            }
            control = preParent.GetComponent<UIControl>();
        }

        /// <summary>
        /// 显示自身案例, 可以在Init中直接添加该事件，防止添加了侦测组件后导致的UI消失
        /// </summary>
        protected void ShowSelf()
        {
            UICommon.Instance.ShowUI(this.name, control);
        }


    }
}