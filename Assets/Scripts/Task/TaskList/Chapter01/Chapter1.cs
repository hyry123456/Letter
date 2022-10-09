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
            chapterTitle = "半夜惨叫，心脏骤停，为何认定他杀";
            taskPartCount = 2;
            chapterID = 1;

            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/1.task";
            targetPart = targetPart + "Chapter1_Task";
            runtimeScene = "MainScene";
        }

        public override void CheckAndLoadChapter()
        {
            if (AsynTaskControl.Instance.CheckTaskIsComplete(0))
            {
                AsynTaskControl.Instance.AddChapter(this);
            }
            return;
        }

        public override void CompleteChapter()
        {
            Debug.Log("第二章完成");
            return;
        }

        public override void ExitChapter()
        {
            return;
        }
    }
}