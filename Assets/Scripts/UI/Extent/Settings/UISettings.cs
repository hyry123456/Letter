using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 用来显示游戏设置的UI管理类
    /// </summary>
    public class UISettings : MonoBehaviour
    {
        private static UISettings instance;
        public static UISettings Instance => instance;

        [SerializeField]
        private GameObject[] controls;
        [SerializeField]
        private UIChangeButton[] buttons;
        /// <summary>   /// 显示用的父级，确定整体的显示以及关闭    /// </summary>
        [SerializeField]
        private GameObject controlsFather;
        /// <summary>    /// 当前显示中的UI    /// </summary>
        private int nowIndex;


        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            if(controls == null || controls.Length == 0)
                return;
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].SetActive(false);
                buttons[i].SetIndex(i);
            }

            controlsFather.SetActive(false);

        }

        private void OnDestroy()
        {
            instance = null;
        }


        /// <summary> /// 显示或者关闭UI设置 /// </summary>
        public void ShowOrCloseUISettings()
        {
            if(nowIndex < 0)
                ShowUISettings();
            else
                CloseUISettings();
        }

        /// <summary>  /// 关闭显示的UI     /// </summary>
        private void CloseUISettings()
        {
            controls[nowIndex].SetActive(false);
            controlsFather.SetActive(false);
            nowIndex = -1;
        }

        /// <summary>  /// 第一次显示UI，打开父级以及第一个界面的UI   /// </summary>
        public void ShowUISettings()
        {
            controlsFather.SetActive(true);
            controls[0].SetActive(true);
            nowIndex = 0;
            Image image = buttons[0].gameObject.GetComponent<Image>();
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }

        /// <summary>    /// 切换当前显示中的UI  /// </summary>
        public void ChangeShowUI(int index)
        {
            if (index == nowIndex) return;
            controls[nowIndex].SetActive(false);
            Image image = buttons[nowIndex].gameObject.GetComponent<Image>();
            Color color = image.color; color.a = 0.5f; image.color = color;

            controls[index].SetActive(true);
            image = buttons[index].gameObject.GetComponent<Image>();
            color = image.color; color.a = 1f; image.color = color;
            nowIndex = index;
        }
    }
}