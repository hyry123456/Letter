using UnityEngine;

namespace UI
{
    /// <summary>
    /// 可交互对象的提示UI
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        private static InteractionUI instance;
        public static InteractionUI Instance => instance;

        /// <summary>    /// 受控制的物体，用来关闭以及开启显示     /// </summary>
        [SerializeField]
        GameObject control;
        /// <summary>
        /// 用来判断是否要开启
        /// </summary>
        bool isShow;

        void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Start()
        {
            control.SetActive(false);
            isShow = false;
        }

        public void ShowInteracte()
        {
            isShow = true;
        }

        public void CloseInteracte()
        {
            isShow= false;
        }

        private void FixedUpdate()
        {
            Control.PlayerControl player = Control.PlayerControl.Instance;
            if (player == null || !player.UseControl)
            {
                control.SetActive(false);
                return;
            }

            control.SetActive(isShow);
        }
    }
}