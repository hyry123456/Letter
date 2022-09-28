using UnityEngine;

namespace Task {
    public class Chapter2 : AsynChapterBase
    {
        public Chapter2()
        {
            chapterName = "第三章，敌人测试";
            taskPartCount = 2;      //表示有两个子任务
            chapterID = 2;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/3.task";
            runtimeScene = "SampleScene";
            targetPart += "Chapter2_Task";
        }

        public override void CheckAndLoadChapter()
        {
            AsynTaskControl.Instance.AddChapter(chapterID);
        }

        public override void CompleteChapter()
        {
        }

        public override void ExitChapter()
        {
        }
    }
}