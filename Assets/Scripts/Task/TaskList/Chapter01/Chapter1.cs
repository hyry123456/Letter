using UnityEngine;

namespace Task
{
    public class Chapter1 : AsynChapterBase
    {
        public Chapter1()
        {
            chapterName = "疑云漫漫";
            chapterDescription = "再入坤家庄";
            taskPartCount = 3;
            chapterID = 1;

            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/1.task";
            targetPart = targetPart + "Chapter1_Task";
            runtimeScene = "MainScene";
        }

        public override void CheckAndLoadChapter()
        {
            if (AsynTaskControl.Instance.CheckChapterIsComplete(0))
            {
                AsynTaskControl.Instance.AddChapter(this);
            }
            return;
        }

        public override void CompleteChapter(bool isInThisScene)
        {
            Debug.Log("第二章完成");
            Common.SustainCoroutine.Instance.AddCoroutine(AddSkill);
            return;
        }
        //添加技能
        bool AddSkill()
        {
            Control.PlayerControl.Instance.AddSkill("SingleBullet");
            Control.PlayerControl.Instance.AddSkill("DetectiveView");
            Control.PlayerControl.Instance.AddSkill("WaveSickle");

            return true;
        }

        public override void ExitChapter()
        {
            AsynTaskControl.Instance.AddChapter(2);     //主线任务
            AsynTaskControl.Instance.AddChapter(4);     //支线任务
            return;
        }
    }
}