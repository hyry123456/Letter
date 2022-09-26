
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
        /// <summary>        
        /// 如果任务需要文本文件，用该路径存储文件，整个章节的文本都加到其中，方便一同读取
        /// </summary>
        public string chapterSavePath;
        /// <summary>        /// 文本读取后的存储位置        /// </summary>
        private List<string> readData;
        /// <summary>        /// 存在的场景，用来判断该任务是否运行在该场景        /// </summary>
        public string runtimeScene;

        /// <summary>
        /// 检查小节完成情况，用于任务的时时检查，判断是否可以进入下一个任务状态
        /// </summary>
        /// <param name="info">交互信息</param>
        public abstract void CheckTask(InteracteInfo info);
        /// <summary>
        /// 检查该章节是否可以启动，可以时调用加载方法，
        /// 在加载时会在未启用时调用，否则可能在某个任务完成后判断是否满足启用要求
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
        /// 当章节完成了，在重新开始游戏时会运行该方法，
        /// 用来加载、删除场景之类的，之后有更好的加载方式可以不用该方法
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