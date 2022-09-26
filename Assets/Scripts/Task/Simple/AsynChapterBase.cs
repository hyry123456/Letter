using System.Reflection;

namespace Task
{
    /// <summary>
    /// ���߳���������స����Ҳ�������̳е���
    /// </summary>
    public abstract class AsynChapterBase : Chapter
    {
        protected string targetPart = "FireControl.Task.";

        //��ʼ������
        //public AsynChapterBase()
        //{
        //    chapterName = "AsynChapterBase";
        //    chapterTitle = "һ�������½�";
        //    taskPartSize = 2;
        //    chapterID = 0;
        //    //�����ļ��ñ������
        //    chapterSavePath = Application.streamingAssetsPath + "/" + "Task/Chapter/0.task";
        //    targetPart = targetPart +"��ֵ���½�����";
        //}


        public override void ChangeTask()
        {
            nowCompletePartId++;
            part.ExitTaskEvent(this);       //�˳���ǰ����
            if (nowCompletePartId == taskPartCount)     //�½����
            {
                AsynTaskControl.Instance.CompleteChapter(this);
                return;
            }
            //δ��ɾ��������½�
            string targetPartStr = targetPart + (nowCompletePartId).ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, false);
        }

        public override void CheckTask(Interaction.InteracteInfo info)
        {
            if (part.IsCompleteTask(this, info))
            {
                ChangeTask();
            }
        }

        /// <summary>        /// ��ʼ�½�ǰ�ȳ�ʼ��        /// </summary>
        public override void BeginChapter()
        {
            string targetPartStr = targetPart + '0';
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, false);
            nowCompletePartId = 0;
        }


        public override void SetNowTaskPart(int nowPart)
        {
            nowCompletePartId = nowPart;
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPart + nowPart.ToString());
            part.EnterTaskEvent(this, true);
        }

    }
}