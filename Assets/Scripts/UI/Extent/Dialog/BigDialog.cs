using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI的大对话类，在对话框中显示对话
    /// </summary>
    public class BigDialog : UIDialogBase
    {
        private static BigDialog instance;
        public static BigDialog Instance
        {
            get { return instance; }
        }

        /// <summary>        /// 对话的文本组件        /// </summary>
        public Text diglogText;
        /// <summary>        /// 名字的文本文件        /// </summary>
        public Text nameText;

        protected void Awake()
        {
            instance = this;
            gameObject.SetActive(false);
        }

        /// <summary>        /// 场景销毁时删除该实例        /// </summary>
        private void OnDestroy()
        {
            instance = null;
        }

        protected override void Update()
        {
            base.Update();
            //进行跳过
            if(sb != null)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    diglogText.text = sb.ToString();
                    sb = null;
                }
            }

        }

        protected override bool CheckWaitEnd()
        {
            if(Input.GetKeyDown(KeyCode.Return))
                return true;
            return false;
        }


        protected override Color GetTextColor()
        {
            return diglogText.color;
        }


        protected override string ReadyOneLineString(string str)
        {
            //使用|作为内容以及名称的区分值
            string[] strs = str.Split('|');
            nameText.text = strs[0];
            return strs[1];
        }

        protected override void SetTextData(string text)
        {
            diglogText.text = text;
        }

        protected override void CloseUI()
        {
            gameObject.SetActive(false);
            if (endBehavior != null)
                endBehavior();
            endBehavior = null;
            Control.PlayerControl.Instance.BeginControl();
        }

        public void ShowBigdialog(string strs, Common.INonReturnAndNonParam endBehavior)
        {
            this.endBehavior = endBehavior;
            gameObject.SetActive(true);
            Control.PlayerControl.Instance.StopControl();
            InsertDialog(strs);
        }
    }
}