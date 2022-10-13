using Task;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI的单个章节的显示按键，当点击该按键后会进行
    /// </summary>
    public class UITaskButton : Common.ObjectPoolBase, IPointerClickHandler
    {
        private UITaskSettings settings;
        [SerializeField]
        private Text chapterName, partName;

        private string chapterDescription, partDescription;
        public string ChapterDescription => chapterDescription;
        public string PartDescription => partDescription;

        /// <summary>
        /// 点击事件，当点击后切换任务
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            settings.ChangeChapter(this);
        }

        protected override void OnEnable()
        {
        }

        /// <summary>   /// 确定改按钮对应的章节     /// </summary>
        /// <param name="index"></param>
        public void SetChapterIndex(int index, UITaskSettings settings)
        {
            this.settings = settings;
            Chapter chapter = AsynTaskControl.Instance.GetExecuteChapter()[index];
            chapterDescription = chapter.chapterDescription;
            partDescription = chapter.GetPartDescribe();

            chapterName.text = chapter.chapterName;
            partName.text = chapter.GetPartName();
        }


    }
}