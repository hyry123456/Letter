using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

namespace UI
{
    /// <summary> /// UI的任务内容显示 /// </summary>
    public class UITaskSettings : MonoBehaviour
    {

        /// <summary>    /// 当前已经拥有的所有按钮    /// </summary>
        private PoolingList<UITaskButton> buttons;
        [SerializeField]
        Text chapterDescription,     //章节描述
            partDescription;        //小节描述

        /// <summary>  /// 任务显示中，每一个章节按钮的预制件   /// </summary>
        public GameObject origin;

        /// <summary>   /// 单个章节的大小，用来动态生成时赋值给Context    /// </summary>
        public int perNodeSize = 30;
        /// <summary>   /// 需要设置大小的Context对象   /// </summary>
        public RectTransform context;


        private void Start()
        {
            buttons = new PoolingList<UITaskButton>();
        }

        private void OnEnable()
        {
            SustainCoroutine.Instance.AddCoroutine(LaterEnable, true);
        }

        /// <summary>
        /// 延迟初始化，因为UI的加载优先级高，导致其他不能正常加载就开始调用了，
        /// 所以需要延迟初始化
        /// </summary>
        bool LaterEnable()
        {
            if (!gameObject.activeSelf) return true;

            //生成处全部的按钮
            List<Task.Chapter> chapters = Task.AsynTaskControl.Instance.GetExecuteChapter();
            if (chapters == null || chapters.Count == 0) return true;
            context.sizeDelta = new Vector2(0, chapters.Count * perNodeSize);


            //在对象池中取出n个物体
            for (int i = 0; i < chapters.Count; i++)
            {
                UITaskButton button = SceneObjectPool.Instance.GetObject<UITaskButton>(
                    "TaskButton", origin, Vector3.zero, Quaternion.identity);
                button.transform.parent = context;
                button.transform.localScale = Vector3.one;
                button.SetChapterIndex(i, this);
                buttons.Add(button);
            }
            ChangeChapter(buttons.GetValue(0));
            return true;
        }

        private void OnDisable()
        {
            //清空全部按钮
            while(buttons.Count > 0)
            {
                UITaskButton button = buttons.GetValue(0);
                button.CloseObject();
                buttons.Remove(0);
            }
            chapterDescription.text = "";
            partDescription.text = "";
        }

        public void ChangeChapter(UITaskButton taskButton)
        {
            chapterDescription.text = taskButton.ChapterDescription;
            partDescription.text = taskButton.PartDescription;
        }
        
    }
}
