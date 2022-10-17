using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    /// <summary>
    /// 最后一关
    /// </summary>
    public class Chapter5 : AsynChapterBase
    {
        public Chapter5()
        {
            chapterName = "一切的终结";
            chapterDescription = "越过障碍，看一下坤家最后的辉煌";
            taskPartCount = 4;
            chapterID = 5;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/5.task";
            targetPart = targetPart + "Chapter5_Task";
            runtimeScene = "FinalScene";
        }

        public override void CheckAndLoadChapter()
        {
            if (AsynTaskControl.Instance.CheckChapterIsComplete(3))
                AsynTaskControl.Instance.AddChapter(this);
        }

        public override void CompleteChapter(bool isInThisScene)
        {
        }

        public override void ExitChapter()
        {
            Debug.Log("章节6完成");
        }
    }
}