
using System.Collections.Generic;
using Interaction;

namespace Task
{
    /// <summary>
    /// 章节类，也就是任务系统的每一系列任务的主类
    /// </summary>
    public abstract class Chapter
    {
        /// <summary>        /// 章节名称，方式时需要用到        /// </summary>
        public string chapterName;
        /// <summary>        /// 章节标题，这个才是游戏中用于显示的任务名称        /// </summary>
        public string chapterTitle;
        /// <summary>        /// 子章节数量        /// </summary>
        public int taskPartCount;
        /// <summary>        /// 当前子章节        /// </summary>
        protected ChapterPart part;
        /// <summary>        /// 章节编号        /// </summary>
        public int chapterID;
        /// <summary>        /// 当前完成的子章节编号        /// </summary>
        public int nowCompletePartId;
        /// <summary>        /// 如果任务需要文本文件，用该路径存储文件        /// </summary>
        public string chapterSavePath;
        /// <summary>        /// 文本读取后的存储位置        /// </summary>
        private List<string> readData;

        /// <summary>
        /// 检查小节完成情况，用于任务的时时检查，判断是否可以进入下一个任务状态
        /// </summary>
        /// <param name="info">交互信息</param>
        public abstract void CheckTask(InteracteInfo info);
        /// <summary>
        /// 检查该章节是否可以启动，可以时自己调用加载方法，
        /// 该方法只有在未完成且未获取时才会调用
        /// </summary>
        public abstract void CheckAndLoadChapter();

        /// <summary>
        /// 改变任务小节时调用
        /// </summary>
        public abstract void ChangeTask();

        /// <summary>
        /// 当章节开启时调用的方法，也就是章节的准备方法，当该任务可以触发时调用
        /// </summary>
        public abstract void BeginChapter();

        /// <summary>        /// 设置任务小章节的同时加载任务章节        /// </summary>
        public abstract void SetNowTaskPart(int nowPart);

        /// <summary>
        /// 退出章节时间，如果需要进行一系列的退出行为，
        /// 可以将方法查到协程控制类上，而不是自己调用协程
        /// </summary>
        public abstract void ExitChapter();

        /// <summary>
        /// 当章节完成时进行的方法，比如加载、删除场景之类的，
        /// 目前先放在这里先，之后换在其他位置
        /// </summary>
        public abstract void CompleteChapter();

        /// <summary>        /// 获得子章节名称        /// </summary>
        public virtual string GetPartName()
        {
            return part.partName;
        }

        /// <summary>        /// 获得子章节描述        /// </summary>
        public virtual string GetPartDescribe()
        {
            return part.partDescribe;
        }

        /// <summary>
        /// 加载对话，根据文本路径读取内容，存储到数组中，
        /// 并且文本仅读取一次
        /// </summary>
        /// <param name="part">读取第几部分</param>
        /// <returns>该部分的文本</returns>
        public string GetDiglogText(int part)
        {
            if(readData == null)
            {
                readData = Common.FileReadAndWrite.ReadFileByAngleBrackets(chapterSavePath);
            }
            return readData[part];
        }
    }
}