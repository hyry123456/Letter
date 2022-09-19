
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// UI 侦测组件，用来侦测物体以及进行UI的实时运作，表示这个是一个会变化的UI
    /// </summary>
    public class UISceneWidgrt : MonoBehaviour, IPointerClickHandler, IDragHandler, 
        IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISceneClickHandler, IPointerUpHandler
    {
        public delegate void PointerClick(PointerEventData eventData);
        /// <summary>        /// 光标点击事件        /// </summary>
        public PointerClick pointerClick;
        /// <summary>
        /// 光标点击事件的调用，请不要随便修改该函数，添加点击事件直接在光标点击事件中添加即可
        /// </summary>
        /// <param name="eventData">点击数据</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (pointerClick != null)
                pointerClick(eventData);
        }

        public delegate void Drag(PointerEventData eventData);
        /// <summary>        /// 拖拽事件        /// </summary>
        public Drag drag;
        /// <summary>       
        /// /// 拖拽事件调用，请不要随便调用该函数，事件添加在drag中添加即可   
        /// /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (drag != null)
                drag(eventData);
        }

        public delegate void PointerDown(PointerEventData eventData);
        /// <summary>        /// 鼠标按下事件，在鼠标按下时进行        /// </summary>
        public PointerDown pointerDown;
        /// <summary>        
        /// /// 鼠标按下事件调用，请在pointerDown中添加事件        
        /// /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (pointerDown != null)
                pointerDown(eventData);
        }

        /// <summary>        /// 鼠标松开事件定义        /// </summary>
        public delegate void PointerUp(PointerEventData eventData);
        /// <summary>        /// 鼠标松开事件        /// </summary>
        public PointerUp pointerUp;
        public void OnPointerUp(PointerEventData eventData)
        {
            if(pointerUp != null)
            {
                pointerUp(eventData);
            }
        }

        public delegate void PointerEnter(PointerEventData eventData);
        /// <summary>        /// 鼠标进入事件，当鼠标进入时调用        /// </summary>
        public PointerEnter pointerEnter;
        /// <summary>        
        /// /// 鼠标进入事件的调用，请在pointerEnter中添加事件       
        /// /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (pointerEnter != null)
                pointerEnter(eventData);
        }

        /// <summary>        /// 鼠标退出事件        /// </summary>
        public delegate void PointerExit(PointerEventData eventData);
        public PointerExit pointerExit;
        /// <summary>        /// 鼠标退出事件调用函数，请勿调用        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (pointerExit != null)
            {
                pointerExit(eventData);
            }
        }

        public delegate void SceneClick(PointerEventData eventData);
        public SceneClick sceneClick;
        public void ScenePointClick(PointerEventData eventData)
        {
            if (sceneClick != null)
                sceneClick(eventData);
        }

        //private void Awake()
        //{
        //    gameObject.SetActive(false);
        //}

    }
}