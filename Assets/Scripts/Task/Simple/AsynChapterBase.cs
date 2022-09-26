using System.Reflection;

namespace Task
{
    /// <summary>
    /// 多线程任务加载类案例，也是用来继承的类
    /// </summary>
    public abstract class AsynChapterBase : Chapter
    {
        protected string targetPart = "Task.";

        //初始化案例
        //public AsynChapterBase()
        //{
        //    chapterName = "AsynChapterBase";
        //    chapterTitle = "一个测试章节";
        //    taskPartSize = 2;
        //    chapterID = 0;
        //    //任务文件用编号命名
        //    chapterSavePath = Application.streamingAssetsPath + "/" + "Task/Chapter/0.task";
        //    targetPart = targetPart +"赋值子章节名称";
        //    runtimeScene = "simpleScene";
        //}


        public override void ChangeTask()
        {
            nowCompletePartId++;
            part.ExitTaskEvent(this);       //退出当前任务

            if (nowCompletePartId == taskPartCount)     //章节完成
            {
                //保存完成的任务
                AsynTaskControl.Instance.CompleteChapter(this);
                //更新获取中的任务
                AsynTaskControl.Instance.SaveObtainChapter();
                return;
            }
            //未完成就搜索子章节
            string targetPartStr = targetPart + nowCompletePartId.ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, false);

            //更新获取中的任务
            AsynTaskControl.Instance.SaveObtainChapter();

        }

        public override void CheckTask(Interaction.InteracteInfo info)
        {
            if (part.IsCompleteTask(this, info))
            {
                ChangeTask();
            }
        }

        /// <summary>        /// 开始章节前先初始化        /// </summary>
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