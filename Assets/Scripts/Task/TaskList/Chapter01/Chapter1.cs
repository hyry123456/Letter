using UnityEngine;


namespace Task {
    public class Chapter1 : AsynChapterBase
    {
        public Chapter1()
        {
            chapterName = "�ڶ��£�������ʹ";
            chapterTitle = "������Ϣ��ʹ���ش�";
            taskPartCount = 2;      //��ʾ������������
            chapterID = 1;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/1.task";
            runtimeScene = "SampleScene";
            targetPart += "Chapter1_Task";
        }


        public override void CheckAndLoadChapter()
        {
            //��һ��û��ɾͲ�����
            if (!AsynTaskControl.Instance.CheckTaskIsComplete(0))
                return;
            Debug.Log("��ʼ�ڶ���");
            //��������뵽��������У���ʾ������ʼ����
            //����һЩ֧�����񣬿��Բ�Ҫ��������½ڣ�������ĳ�ؽ�����������½�
            AsynTaskControl.Instance.AddChapter(chapterID);     
        }

        public override void CompleteChapter()
        {
            Debug.Log("�ڶ������");
        }

        public override void ExitChapter()
        {
            Debug.Log("�ڶ������");
            //һ����֮����������ֱ�������������һ��
        }
    }
}