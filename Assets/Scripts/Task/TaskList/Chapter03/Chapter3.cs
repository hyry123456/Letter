using UnityEngine;

namespace Task
{
    /// <summary>/// 第四章，也就是遗迹场景的关卡，用来提示下关卡信息/// </summary>
    public class Chapter3 : AsynChapterBase
    {
        public Chapter3()
        {
            chapterName = "探索遗迹";
            chapterDescription = "破败的遗迹，幻梦般的场景，鲲家的秘密";
            taskPartCount = 3;
            chapterID = 3;
            chapterSavePath = Application.streamingAssetsPath +
                "/Task/Chapter/3.task";
            targetPart = targetPart + "Chapter3_Task";
            runtimeScene = "AmongRemain";
        }

        public override void CheckAndLoadChapter()
        {
            if (AsynTaskControl.Instance.CheckChapterIsComplete(2))
            {
                AsynTaskControl.Instance.AddChapter(this);
            }
        }

        public override void CompleteChapter(bool isInThisScene)
        {
            Common.SustainCoroutine.Instance.AddCoroutine(AddSkill);
        }

        bool AddSkill()
        {
            if (Control.PlayerControl.Instance == null) return true;
            Control.PlayerControl.Instance.AddSkill("HookRope");
            Control.PlayerControl.Instance.AddSkill("SingleBullet");
            Control.PlayerControl.Instance.AddSkill("WaveSickle");
            Control.PlayerControl.Instance.AddSkill("DetectiveView");
            return true;
        }

        public override void ExitChapter()
        {
        }
    }
}