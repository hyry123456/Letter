
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Task
{

    struct TaskInfo
    {
        public string Name;
        /// <summary>        /// 任务状态，0=未开始、1=开始中、2=完成        /// </summary>
        public int state;
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

        /// <summary>        /// 进行中的任务        /// </summary>
        private List<Chapter> exectuteTasks;


        /// <summary>        /// 所有任务的映射容器，<编号，名称>        /// </summary>
        private Dictionary<int, TaskInfo> taskMap;

        private void LoadTask()
        {
            LoadAllTask();          //加载所有的任务
            LoadObtainTask();       //加载所有持有的任务
            LoadCompleteTask();     //确定完成的任务
            ReadyTask();            //开始执行任务检测
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

            string chapterPrefix = "FireControl.Task.";
            Assembly assembly = Assembly.GetExecutingAssembly();
            for (int i = 0; i < allTasks.Length; i++)
            {
                if (allTasks[i] == null || allTasks[i].Length == 0)
                    continue;
                string target = chapterPrefix + allTasks[i].Trim();
                Chapter chapterTask = (Chapter)assembly.CreateInstance(target);
                if (chapterTask == null)
                {
                    Debug.Log(target);
                }
                //生成所有章节的映射关系
                taskMap.Add(chapterTask.chapterID, new TaskInfo { Name = allTasks[i].Trim(), state = 0 });
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
                string chapterPrefix = "FireControl.Task.";
                Assembly assembly = Assembly.GetExecutingAssembly();
                for (int i = 0; i < task.Count; i++)
                {
                    string[] tremps = task[i].Split(' ');
                    int index = int.Parse(tremps[0]);
                    TaskInfo taskInfo = taskMap[index];
                    taskInfo.state = 1;     //运行中
                    Chapter chapterTask = 
                        (Chapter)assembly.CreateInstance(chapterPrefix + taskInfo.Name);
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
                    string chapterPrefix = "FireControl.Task.";
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    for (int i = 0; i < comTasks.Length; i++)
                    {
                        int value;
                        if (int.TryParse(comTasks[i], out value))
                        {
                            taskMap.TryGetValue(value, out TaskInfo task);
                            task.state = 2;     //表示完成
                            taskMap[value] = task;
                            Chapter chapterTask = 
                                (Chapter)assembly.CreateInstance(chapterPrefix + task.Name);
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
            string chapterPrefix = "FireControl.Task.";
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach(TaskInfo info in taskMap.Values)
            {
                if (info.state != 0)
                    continue;
                Chapter chapterTask = (Chapter)assembly.CreateInstance(chapterPrefix + info.Name);
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
            return taskMap[taskId].state == 2;
        }

        /// <summary>        /// 任务完成的通用行为，将该任务退出，然后保存文件        /// </summary>
        /// <param name="chapter">要完成的任务</param>
        public void CompleteChapter(Chapter chapter)
        {
            exectuteTasks.Remove(chapter);
            //调用退出函数
            chapter.ExitChapter();
            TaskInfo info = taskMap[chapter.chapterID];
            info.state = 2;         //完成任务
            taskMap[chapter.chapterID] = info;

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");

            foreach (KeyValuePair<int, TaskInfo> taskInfo in taskMap)
            {
                if(taskInfo.Value.state == 2)
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
            info.state = 1;
            string chapterPrefix = "FireControl.Task.";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Chapter chapter = (Chapter)assembly.CreateInstance(chapterPrefix + info.Name);

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



    }
}