
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
        /// <summary>        /// 当前可以触发的交互信息        /// </summary>
        public InteractionBase nowInteractionInfo;
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
        }

        /// <summary> /// 检查是否有可以交互的对象  /// </summary>
        protected void FixedUpdate()
        {
            RaycastHit hit;
            Debug.DrawRay(playerControl.transform.position, playerControl.GetLookatDir() * 3);
            if (Physics.Raycast(playerControl.transform.position, playerControl.GetLookatDir() * 3, out hit, interacteCheckDistance))
            {
                InteractionBase hitInfo = hit.transform.GetComponent<InteractionBase>();
                if (hitInfo != null && !hit.collider.isTrigger)
                {
                    nowInteractionInfo = hitInfo;
                }
                else
                    nowInteractionInfo = null;
            }
            else
                nowInteractionInfo = null;
            if(nowInteractionInfo == null)
                UI.InteractionUI.Instance.CloseInteracte();
            else
                UI.InteractionUI.Instance.ShowInteracte();
        }

        /// <summary>        /// 运行交互事件        /// </summary>
        /// <param name="interactionInfo">发生的交互事件</param>
        public void RunInteraction(InteractionBase interactionInfo)
        {
            if (interactionInfo == null) { 
                Debug.Log("交互对象空了");
            }
            //运行交互行为
            interactionInfo.InteractionBehavior();
        }

        /// <summary>
        /// 运行当前正在交互的交互事件
        /// </summary>
        public void RunInteraction()
        {
            if (nowInteractionInfo == null) return;
            RunInteraction(nowInteractionInfo);
        }
    }
}