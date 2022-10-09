using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    public class Chapter1 : AsynChapterBase
    {
        public Chapter1()
        {
            chapterName = "Ϊ��������Ϊ��";
            chapterTitle = "��ҹ�ҽУ�������ͣ��Ϊ���϶���ɱ";
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
            Debug.Log("�ڶ������");
            return;
        }

        public override void ExitChapter()
        {
            return;
        }
    }
}