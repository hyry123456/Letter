using UnityEngine;


namespace Task {
    public class Chapter0 : AsynChapterBase
    {
        public Chapter0()
        {
            chapterName = "��һ�£�������ʹ";
            chapterTitle = "������Ϣ��ʹ���ش�";
            taskPartCount = 2;      //��ʾ������������
            chapterID = 0;
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/0.task";
            runtimeScene = "SampleScene";
            targetPart += "Chapter0_Task";
        }


        public override void CheckAndLoadChapter()
        {
            //��һ��һ�����ã����Ǹտ�ʼ��Ϸ
            Debug.Log("��ʼ��һ��");
            //��������뵽��������У���ʾ������ʼ����
            //����һЩ֧�����񣬿��Բ�Ҫ��������½ڣ�������ĳ�ؽ�����������½�
            AsynTaskControl.Instance.AddChapter(chapterID);     
        }

        public override void CompleteChapter()
        {
            Debug.Log("��һ���Ѿ������");
        }

        public override void ExitChapter()
        {
            Debug.Log("��һ�����");
            //һ����֮����������ֱ�������������һ��
            AsynTaskControl.Instance.AddChapter(1);
        }
    }
}