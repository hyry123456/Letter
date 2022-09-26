
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Task
{
    public enum TaskMode
    {
        /// <summary>   /// 任务未开始   /// </summary>
        NotStart = 1,
        /// <summary>   /// 任务开始中   /// </summary>
        Start = 2,
        /// <summary>   /// 任务完成     /// </summary>
        Finish = 4,
    }

    struct TaskInfo
    {
        public string Name;
        public TaskMode state;
        /// <summary>        /// 是否属于当前正在运行的场景        /// </summary>
        public bool isInRuntimeScene;
    }
    public class AsynTaskControl
    {
        private static AsynTaskControl instance;
        public static AsynTaskControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AsynTaskControl();
                }
                return instance;
            }
        }

        /// <summary>        /// 所有任务        /// </summary>
        private static string allTaskPath = Application.streamingAssetsPath + "/Task/AllTask.task";
        /// <summary>        /// 完成的任务        /// </summary>
        private static string completeTaskPath = Application.streamingAssetsPath + "/Task/CompleteTask.task";
        /// <summary>        /// 进行中的任务的存储文件        /// </summary>
        private static string obtainTaskPath = Application.streamingAssetsPath + "/Task/ObtainTask.task";
        /// <summary>        /// 进行反射查找类的名称前缀        /// </summary>
        const string chapterPrefix = "Task.";

        /// <summary>        /// 进行中的任务        /// </summary>
        private List<Chapter> exectuteTasks;

        /// <summary>        /// 所有任务的映射容器，<编号，名称>        /// </summary>
        private Dictionary<int, TaskInfo> taskMap;
        /// <summary>
        /// 当前运行的场景名称，因为任务是在子线程中加载的，因此需要在协程中进行名称获取
        /// </summary>
        string nowSceneName;

        /// <summary>        /// 返回开始状态，重置所有任务        /// </summary>
        public static void ClearData()
        {
            Common.FileReadAndWrite.WriteFile(completeTaskPath, "");
            Common.FileReadAndWrite.WriteFile(obtainTaskPath, "");
        }

        private AsynTaskControl()
        {
            //多线程加载
            AsyncLoad.Instance.AddAction(LoadTask);
        }

        private bool FindTaskName()
        {
            nowSceneName = Control.SceneChangeControl.Instance.GetRuntimeSceneName();
            return true;
        }

        private void LoadTask()
        {
            nowSceneName = null;
            Common.SustainCoroutine.Instance.AddCoroutine(FindTaskName);
            while(nowSceneName == null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));  //休眠该线程，等待场景名称获取
            }

            LoadAllTask();          //加载所有的任务
            LoadObtainTask();       //加载所有持有的任务
            LoadCompleteTask();     //确定完成的任务
            ReadyTask();            //开始执行任务检测
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        /// <summary>        /// 通过反射创建章节对象        /// </summary>
        /// <param name="chapterName">章节名称</param>
        private Chapter GetChapter(string chapterName)
        {
            return (Chapter)assembly.CreateInstance(chapterName);
        }

        /// <summary>        /// 生成所有任务的映射关系表        /// </summary>
        private void LoadAllTask()
        {
            string allTaskStr = Common.FileReadAndWrite.DirectReadFile(allTaskPath);
            string[] allTasks = null;
            if (allTaskStr != null && !allTaskStr.Equals(""))
            {
                allTasks = allTaskStr.Split('\n');
            }
            else
                return;
            taskMap = new Dictionary<int, TaskInfo>(allTasks.Length);

            for (int i = 0; i < allTasks.Length; i++)
            {
                if (allTasks[i] == null || allTasks[i].Length == 0)
                    continue;
                string target = chapterPrefix + allTasks[i].Trim();
                Chapter chapterTask = GetChapter(target);

                if (chapterTask == null)
                {
                    Debug.Log(target);
                }
                //生成所有章节的映射关系，章节编号：章节信息
                taskMap.Add(chapterTask.chapterID, new TaskInfo {
                    Name = allTasks[i].Trim(), 
                    state = 0,
                    //判断该任务是否要在该场景运行
                    isInRuntimeScene = (nowSceneName == chapterTask.runtimeScene), 
                });
            }
        }

        /// <summary>
        /// 加载获取了的任务的文件
        /// 任务存储格式：<ChapterId nowPartIndex>，章节编号+当前子任务的编号
        /// </summary>
        private void LoadObtainTask()
        {
            exectuteTasks = new List<Chapter>();
            List<string> task = Common.FileReadAndWrite.ReadFileByAngleBrackets(obtainTaskPath);
            if (task != null && task.Count > 0)
            {
                for (int i = 0; i < task.Count; i++)
                {
                    string[] tremps = task[i].Split(' ');
                    int index = int.Parse(tremps[0]);
                    TaskInfo taskInfo = taskMap[index];
                    //非本场景的任务，直接跳过
                    if (!taskInfo.isInRuntimeScene) continue;

                    taskInfo.state = TaskMode.Start;     //运行中
                    Chapter chapterTask = GetChapter(chapterPrefix + taskInfo.Name);
                    taskMap[index] = taskInfo;

                    //插入到正在运行的任务数组中
                    exectuteTasks.Add(chapterTask);
                    //设置同时启动
                    chapterTask.SetNowTaskPart(int.Parse(tremps[1]));
                }
                task.Clear();
            }
            else
            {
                if (task != null)
                    task.Clear();
            }

        }

        /// <summary>        /// 加载所有完成了的任务        /// </summary>
        private void LoadCompleteTask()
        {
            string completeTask = Common.FileReadAndWrite.DirectReadFile(completeTaskPath);
            //赋值完成的任务列表
            if (completeTask != null && !completeTask.Equals(""))
            {
                string[] comTasks = completeTask.Split('\n');
                if (comTasks != null && comTasks.Length > 0)
                {
                    for (int i = 0; i < comTasks.Length; i++)
                    {
                        int value;
                        if (int.TryParse(comTasks[i], out value))
                        {
                            TaskInfo task = taskMap[value];
                            task.state ^= TaskMode.Finish;     //表示完成

                            //非运行在本场景，标识为完成后跳过
                            if (!task.isInRuntimeScene) continue;
                            taskMap[value] = task;

                            Chapter chapterTask = GetChapter(chapterPrefix + task.Name);
                            chapterTask.CompleteChapter();      //调用任务完成的方法
                        }
                    }
                }
            }
        }

        /// <summary>        /// 运行任务        /// </summary>
        private void ReadyTask()
        {
            if (taskMap == null) return;
            foreach(TaskInfo info in taskMap.Values)
            {
                //未开始的任务就调用检查方法看看是否要开始该任务
                if (info.state != 0 || !info.isInRuntimeScene)
                    continue;
                Chapter chapterTask = GetChapter(chapterPrefix + info.Name);
                chapterTask.CheckAndLoadChapter();
            }
        }

        /// <summary>
        /// 检查任务是否完成，任务加载是以分支为根据的，所以只需要检查前面是否已经完成就够了，
        /// 因此这里只提供检查的方法
        /// </summary>
        /// <param name="taskId">任务的编号，注意该编号值要唯一</param>
        public bool CheckTaskIsComplete(int taskId)
        {
            return (taskMap[taskId].state & TaskMode.Finish) != 0;
        }

        /// <summary>        /// 任务完成的通用行为，将该任务退出，然后保存文件        /// </summary>
        /// <param name="chapter">要完成的任务</param>
        public void CompleteChapter(Chapter chapter)
        {
            exectuteTasks.Remove(chapter);
            //调用退出函数
            chapter.ExitChapter();
            TaskInfo info = taskMap[chapter.chapterID];
            info.state = TaskMode.Finish;         //完成任务
            taskMap[chapter.chapterID] = info;

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");

            foreach (KeyValuePair<int, TaskInfo> taskInfo in taskMap)
            {
                if(taskInfo.Value.state == TaskMode.Finish)
                {
                    stringBuilder.Append(taskInfo.Key.ToString() + '\n');
                }
            }
            Common.FileReadAndWrite.WriteFile(completeTaskPath, stringBuilder.ToString());
            stringBuilder.Clear();
        }

        /// <summary>        /// 保存目前运行的任务        /// </summary>
        public void SaveObtainChapter()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");

            for (int i = 0; i < exectuteTasks.Count; i++)
            {
                if (exectuteTasks[i].taskPartCount <= exectuteTasks[i].nowCompletePartId)
                    Debug.LogError("超了");
                stringBuilder.Append("<" 
                    + exectuteTasks[i].chapterID.ToString() + ' ' 
                    + exectuteTasks[i].nowCompletePartId.ToString() + ">");
            }
            //Debug.Log("Save = " + stringBuilder.ToString());
            Common.FileReadAndWrite.WriteFile(obtainTaskPath, stringBuilder.ToString());
            stringBuilder.Clear();
        }

        /// <summary>        /// 获取目前正在运行的所有任务        /// </summary>
        public List<Chapter> GetExecuteChapter()
        {
            return exectuteTasks;
        }

        /// <summary>
        /// 重新加载一次任务，可在切换场景后调用，用来判断这个场景中的任务
        /// </summary>
        public void ReLoadTask()
        {
            AsyncLoad.Instance.AddAction(LoadTask);
        }

        /// <summary>        /// 获取单个真正运行的任务        /// </summary>
        public Chapter GetExecuteChapterByIndex(int index)
        {
            return exectuteTasks[index];
        }

        /// <summary>        /// 检查任务的调用函数        /// </summary>
        public void CheckChapter(int chaptherId, Interaction.InteracteInfo data)
        {
            for (int i = 0; i < exectuteTasks.Count; i++)
            {
                if (exectuteTasks[i].chapterID == chaptherId)
                {
                    exectuteTasks[i].CheckTask(data);
                    return;
                }
            }
        }


        /// <summary>
        /// 添加章节函数，用于给添加任务类型的交互类型调用
        /// </summary>
        /// <param name="chapter">章节名称</param>
        /// <returns>是否添加成功</returns>
        public bool AddChapter(Chapter chapter)
        {
            if (exectuteTasks == null)
            {
                exectuteTasks = new List<Chapter> { chapter };
                chapter.BeginChapter();
                SaveObtainChapter();
                return true;
            }
            else
            {
                for (int i = 0; i < exectuteTasks.Count; i++)
                {
                    if (exectuteTasks[i].chapterID == chapter.chapterID)
                    {
                        Debug.Log("出现重复任务");
                        return false;
                    }
                }
                exectuteTasks.Add(chapter);
                chapter.BeginChapter();
                SaveObtainChapter();
                return true;
            }
        }

        public bool AddChapter(int chapterId)
        {
            TaskInfo info = taskMap[chapterId];
            if (info.state != 0)
                return false;
            info.state = TaskMode.Start;
            Chapter chapter = GetChapter(chapterPrefix + info.Name);

            if (exectuteTasks == null)
            {
                exectuteTasks = new List<Chapter> { chapter };
                chapter.BeginChapter();
                SaveObtainChapter();
                return true;
            }
            else
            {
                exectuteTasks.Add(chapter);
                chapter.BeginChapter();
                SaveObtainChapter();
                return true;
            }

        }

        /// <summary>        /// 准备章节，可以作为章节的启动方法        /// </summary>
        /// <param name="chapterId">章节编号</param>
        public void ReadyChapter(int chapterId)
        {
            TaskInfo taskInfo = taskMap[chapterId];
            if(taskInfo.state == TaskMode.NotStart)
            {
                Chapter chapter = GetChapter(chapterPrefix + taskInfo.Name);
                chapter.CheckAndLoadChapter();
            }
        }



    }
}