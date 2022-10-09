using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SmallDialog : UIDialogBase
    {
        private static SmallDialog instance;
        public static SmallDialog Instance => instance;

        /// <summary>        /// 每一个文字生成的等待时间        /// </summary>
        public float perFontWaitTime = 0.08f;
        private float nowWaitTime = 0;
        /// <summary>        /// 小对话显示用的文本组件        /// </summary>
        Text smallDialog;

        private void Awake()
        {
            smallDialog = GetComponentInChildren<Text>();
            instance = this;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        protected override bool CheckWaitEnd()
        {
            nowWaitTime += Time.deltaTime;
            if(nowWaitTime > perFontWaitTime * smallDialog.text.Length)
            {
                nowWaitTime = 0;
                return true;
            }
            return false;
        }

        protected override Color GetTextColor()
        {
            return smallDialog.color;
        }

        protected override string ReadyOneLineString(string str)
        {
            str = str.Trim();
            return str;
        }

        protected override void SetTextData(string text)
        {
            smallDialog.text = text;
        }

        protected override void CloseUI()
        {
            gameObject.SetActive(false);
            if(endBehavior != null)
            {
                endBehavior();
                endBehavior = null;
            }
            nowWaitTime = 0;
        }

        public void ShowSmallDialog(string strs, Common.INonReturnAndNonParam endBehavior)
        {
            this.endBehavior = endBehavior;
            InsertDialog(strs);
            gameObject.SetActive(true);
            return;
        }
    }
}