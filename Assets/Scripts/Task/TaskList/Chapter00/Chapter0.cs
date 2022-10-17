using UnityEngine;


namespace Task {
    public class Chapter0 : AsynChapterBase
    {
        public Chapter0()
        {
            chapterName = "疑云漫漫";
            chapterDescription = "多年不见的老爷，神秘的信件，再入坤家谜团";
            taskPartCount = 1;      //表示有1个子任务
            chapterID = 0;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/0.task";
            runtimeScene = "MainScene";
            targetPart += "Chapter0_Task";
        }


        public override void CheckAndLoadChapter()
        {
            //第一章一旦调用，就是刚开始游戏
            Debug.Log("开始第一章");
            //将任务加入到任务管理中，表示该任务开始运行
            //对于一些支线任务，可以不要立刻添加章节，而是在某地交互后再添加章节
            AsynTaskControl.Instance.AddChapter(chapterID);     
        }

        public override void CompleteChapter(bool isInThisScene)
        {
        }

        public override void ExitChapter()
        {
            Debug.Log("第一章完成");
            //一般有之后的任务可以直接在这里插入下一章
            AsynTaskControl.Instance.AddChapter(1);
        }
    }
}