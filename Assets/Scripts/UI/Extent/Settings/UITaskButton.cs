using Task;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI�ĵ����½ڵ���ʾ������������ð���������
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
        /// ����¼�����������л�����
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            settings.ChangeChapter(this);
        }

        protected override void OnEnable()
        {
        }

        /// <summary>   /// ȷ���İ�ť��Ӧ���½�     /// </summary>
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