using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    public class Chapter1 : AsynChapterBase
    {
        public Chapter1()
        {
            chapterName = "为何死，何为死";
            chapterDescription = "半夜惨叫，心脏骤停，为何认定他杀";
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

        public override void CompleteChapter()
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
            AsynTaskControl.Instance.AddChapter(2);
            return;
        }
    }
}