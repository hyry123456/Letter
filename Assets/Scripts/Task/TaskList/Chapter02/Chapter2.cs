using UnityEngine;

namespace Task
{
    /// <summary>
    /// �����£����ǰ����һ�������Ļ��ᣬҲ����ֻ��һ������
    /// </summary>
    public class Chapter2 : AsynChapterBase
    {

        public Chapter2()
        {
            chapterName = "������ʵ�����кβ�";
            chapterTitle = "���ؼ�̳����֮�ż���������ʲô����";
            taskPartCount = 1;
            chapterID = 2;

            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/2.task";
            targetPart = targetPart + "Chapter2_Task";
            runtimeScene = "MainScene";
        }

        public override void CheckAndLoadChapter()
        {
            if (AsynTaskControl.Instance.CheckChapterIsComplete(1))
                AsynTaskControl.Instance.AddChapter(this);
            return;
        }

        public override void CompleteChapter()
        {
            Debug.Log("�����½���");
        }

        public override void ExitChapter()
        {
            return;
        }
    }
}