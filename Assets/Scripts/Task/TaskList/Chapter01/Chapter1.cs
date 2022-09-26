using UnityEngine;


namespace Task {
    public class Chapter1 : AsynChapterBase
    {
        public Chapter1()
        {
            chapterName = "第二章，新人信使";
            chapterTitle = "大清信息，使命必达";
            taskPartCount = 2;      //表示有两个子任务
            chapterID = 1;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/1.task";
            runtimeScene = "SampleScene";
            targetPart += "Chapter1_Task";
        }


        public override void CheckAndLoadChapter()
        {
            //第一章没完成就不进入
            if (!AsynTaskControl.Instance.CheckTaskIsComplete(0))
                return;
            Debug.Log("开始第二章");
            //将任务加入到任务管理中，表示该任务开始运行
            //对于一些支线任务，可以不要立刻添加章节，而是在某地交互后再添加章节
            AsynTaskControl.Instance.AddChapter(chapterID);     
        }

        public override void CompleteChapter()
        {
            Debug.Log("第二章完成");
        }

        public override void ExitChapter()
        {
            Debug.Log("第二章完成");
            //一般有之后的任务可以直接在这里插入下一章
        }
    }
}