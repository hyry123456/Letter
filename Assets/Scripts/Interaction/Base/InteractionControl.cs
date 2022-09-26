
using UnityEngine;

namespace Interaction
{
    public class InteractionControl : MonoBehaviour
    {
        private static InteractionControl instance;

        public static InteractionControl Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("intreraction");
                    instance = go.AddComponent<InteractionControl>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                }
                return instance;
            }
        }

        private Control.PlayerControl playerControl;
        /// <summary>        /// 交互检测射线的起始点        /// </summary>
        //public Transform beginPos;
        /// <summary>        /// 当前可以触发的交互信息        /// </summary>
        public InteractionBase nowInteractionInfo;
        /// <summary>        /// 是否正在交互中        /// </summary>
        public bool isInteracting;
        /// <summary>        /// 射线检测的距离        /// </summary>
        public float interacteCheckDistance = 3f;

        private InteractionControl() { }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }
        
        /// <summary>        /// 挂到主角上，由主角时时交互        /// </summary>
        private void Start()
        {
            playerControl = gameObject.GetComponent<Control.PlayerControl>();
            isInteracting = false;
        }

        /// <summary>
        /// 检查是否有可以交互的对象
        /// </summary>
        protected void FixedUpdate()
        {
            //交互中就退出
            if (isInteracting) 
            {
                return;
            }
            RaycastHit hit;
            if (Physics.Raycast(playerControl.transform.position, playerControl.GetLookatDir() * 3, out hit, interacteCheckDistance))
            {
                InteractionBase hitInfo = hit.transform.GetComponent<InteractionBase>();
                if (hitInfo != null)
                {
                    nowInteractionInfo = hitInfo;
                    return;
                }
                nowInteractionInfo = null;
            }
        }

        /// <summary>        /// 运行交互事件        /// </summary>
        /// <param name="interactionInfo">发生的交互事件</param>
        public void RunInteraction(InteractionBase interactionInfo)
        {
            isInteracting = true;   //设置正在交互
            if (interactionInfo == null) { 
                Debug.Log("交互对象空了");
            }
            //运行交互行为
            interactionInfo.InteractionBehavior();
        }

        /// <summary>
        /// 有任务系统的UI显示结束后调用，表示交互的UI结束了，准备接下来的操作吧
        /// </summary>
        public void ReRunInteraction()
        {
            RunInteraction(nowInteractionInfo);
        }

        /// <summary>
        /// 运行当前正在交互的交互事件
        /// </summary>
        public void RunInteraction()
        {
            if (nowInteractionInfo == null) return;
            RunInteraction(nowInteractionInfo);
        }

        /// <summary>
        /// 表示停止交互，该系统重新开始工作
        /// </summary>
        public void StopInteraction()
        {
            isInteracting = false;
        }

        /// <summary>        /// 开启交互        /// </summary>
        public void StartInteraction()
        {
            isInteracting = true;
        }

        public bool GetInteraction()
        {
            return isInteracting;
        }

        /// <summary>
        /// 添加交互信息，由于当交互事件不是射线点击时触发时需要添加交互就需要手动添加了
        /// </summary>
        /// <param name="interaction">添加的交互信息</param>
        public void AddInteractionInfo(InteractionBase interaction)
        {
            nowInteractionInfo = interaction;
        }
    }
}