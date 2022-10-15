using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    /// <summary>
    /// 支线剧情，解锁主角的2连跳技能
    /// </summary>
    public class Chapter4 : AsynChapterBase
    {
        public Chapter4()
        {
            chapterName = "坤家的馈赠";
            chapterDescription = "坤家因此的秘技，到底有什么样的能力";
            taskPartCount = 2;
            chapterID = 4;
            chapterSavePath = Application.streamingAssetsPath + "/" + "Task/Chapter/4.task";
            targetPart = targetPart + "Chapter4_Task";
            runtimeScene = "MainScene";
        }

        public override void CheckAndLoadChapter()
        {
            //当第1章结束，提示前往祭坛前启动该支线任务
            if (AsynTaskControl.Instance.CheckChapterIsComplete(1))
                AsynTaskControl.Instance.AddChapter(this);
        }

        public override void CompleteChapter(bool isInThisScene)
        {
            //添加2段跳技能
            Common.SustainCoroutine.Instance.AddCoroutine(AddSkill);
        }

        bool AddSkill()
        {
            Control.PlayerControl.Instance.Motor.maxAirJumps = 1;
            return true;
        }


        public override void ExitChapter()
        {
            Debug.Log("第四章完成");
        }
    }
}