using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// UI显示用的按钮，该按钮需要一开始就显示，其是否显示是有其父级物体决定的
    /// 在按钮显示后需要独立于设置页面
    /// </summary>
    public class UIChangeButton : UIUseBase
    {
        private int index;
        protected override void Awake()
        {
            base.Awake();
            control.init += ShowSelf;
            widgrt.pointerClick += ChangeShowUI;
        }

        private void ChangeShowUI(PointerEventData eventData)
        {
            UISettings.Instance.ChangeShowUI(index);
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }
    }
}