
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Task
{
    public enum TaskMode
    {
        /// <summary>   /// ����δ��ʼ   /// </summary>
        NotStart = 1,
        /// <summary>   /// ����ʼ��   /// </summary>
        Start = 2,
        /// <summary>   /// �������     /// </summary>
        Finish = 4,
    }

    struct TaskInfo
    {
        public string Name;
        public TaskMode state;
        /// <summary>        /// �Ƿ����ڵ�ǰ�������еĳ���        /// </summary>
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

        /// <summary>        /// ��������        /// </summary>
        private static string allTaskPath = Application.streamingAssetsPath + "/Task/AllTask.task";
        /// <summary>        /// ��ɵ�����        /// </summary>
        private static string completeTaskPath = Application.streamingAssetsPath + "/Task/CompleteTask.task";
        /// <summary>        /// �����е�����Ĵ洢�ļ�        /// </summary>
        private static string obtainTaskPath = Application.streamingAssetsPath + "/Task/ObtainTask.task";
        /// <summary>        /// ���з�������������ǰ׺        /// </summary>
        const string chapterPrefix = "Task.";

        /// <summary>        /// �����е�����        /// </summary>
        private List<Chapter> exectuteTasks;

        /// <summary>        /// ���������ӳ��������<��ţ�����>        /// </summary>
        private Dictionary<int, TaskInfo> taskMap;
        /// <summary>
        /// ��ǰ���еĳ������ƣ���Ϊ�����������߳��м��صģ������Ҫ��Э���н������ƻ�ȡ
        /// </summary>
        string nowSceneName;

        /// <summary>        /// ���ؿ�ʼ״̬��������������        /// </summary>
        public static void ClearData()
        {
            Common.FileReadAndWrite.WriteFile(completeTaskPath, "");
            Common.FileReadAndWrite.WriteFile(obtainTaskPath, "");
        }

        private AsynTaskControl()
        {
            //���̼߳���
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
                Thread.Sleep(TimeSpan.FromSeconds(1));  //���߸��̣߳��ȴ��������ƻ�ȡ
            }

            LoadAllTask();          //�������е�����
            LoadObtainTask();       //�������г��е�����
            LoadCompleteTask();     //ȷ����ɵ�����
            ReadyTask();            //��ʼִ��������
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        /// <summary>        /// ͨ�����䴴���½ڶ���        /// </summary>
        /// <param name="chapterName">�½�����</param>
        private Chapter GetChapter(string chapterName)
        {
            return (Chapter)assembly.CreateInstance(chapterName);
        }

        /// <summary>        /// �������������ӳ���ϵ��        /// </summary>
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
                //���������½ڵ�ӳ���ϵ���½ڱ�ţ��½���Ϣ
                taskMap.Add(chapterTask.chapterID, new TaskInfo {
                    Name = allTasks[i].Trim(), 
                    state = 0,
                    //�жϸ������Ƿ�Ҫ�ڸó�������
                    isInRuntimeScene = (nowSceneName == chapterTask.runtimeScene), 
                });
            }
        }

        /// <summary>
        /// ���ػ�ȡ�˵�������ļ�
        /// ����洢��ʽ��<ChapterId nowPartIndex>���½ڱ��+��ǰ������ı��
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
                    //�Ǳ�����������ֱ������
                    if (!taskInfo.isInRuntimeScene) continue;

                    taskInfo.state = TaskMode.Start;     //������
                    Chapter chapterTask = GetChapter(chapterPrefix + taskInfo.Name);
                    taskMap[index] = taskInfo;

                    //���뵽�������е�����������
                    exectuteTasks.Add(chapterTask);
                    //����ͬʱ����
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

        /// <summary>        /// ������������˵�����        /// </summary>
        private void LoadCompleteTask()
        {
            string completeTask = Common.FileReadAndWrite.DirectReadFile(completeTaskPath);
            //��ֵ��ɵ������б�
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
                            task.state ^= TaskMode.Finish;     //��ʾ���

                            //�������ڱ���������ʶΪ��ɺ�����
                            if (!task.isInRuntimeScene) continue;
                            taskMap[value] = task;

                            Chapter chapterTask = GetChapter(chapterPrefix + task.Name);
                            chapterTask.CompleteChapter();      //����������ɵķ���
                        }
                    }
                }
            }
        }

        /// <summary>        /// ��������        /// </summary>
        private void ReadyTask()
        {
            if (taskMap == null) return;
            foreach(TaskInfo info in taskMap.Values)
            {
                //δ��ʼ������͵��ü�鷽�������Ƿ�Ҫ��ʼ������
                if (info.state != 0 || !info.isInRuntimeScene)
                    continue;
                Chapter chapterTask = GetChapter(chapterPrefix + info.Name);
                chapterTask.CheckAndLoadChapter();
            }
        }

        /// <summary>
        /// ��������Ƿ���ɣ�����������Է�֧Ϊ���ݵģ�����ֻ��Ҫ���ǰ���Ƿ��Ѿ���ɾ͹��ˣ�
        /// �������ֻ�ṩ���ķ���
        /// </summary>
        /// <param name="taskId">����ı�ţ�ע��ñ��ֵҪΨһ</param>
        public bool CheckTaskIsComplete(int taskId)
        {
            return (taskMap[taskId].state & TaskMode.Finish) != 0;
        }

        /// <summary>        /// ������ɵ�ͨ����Ϊ�����������˳���Ȼ�󱣴��ļ�        /// </summary>
        /// <param name="chapter">Ҫ��ɵ�����</param>
        public void CompleteChapter(Chapter chapter)
        {
            exectuteTasks.Remove(chapter);
            //�����˳�����
            chapter.ExitChapter();
            TaskInfo info = taskMap[chapter.chapterID];
            info.state = TaskMode.Finish;         //�������
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

        /// <summary>        /// ����Ŀǰ���е�����        /// </summary>
        public void SaveObtainChapter()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("");

            for (int i = 0; i < exectuteTasks.Count; i++)
            {
                if (exectuteTasks[i].taskPartCount <= exectuteTasks[i].nowCompletePartId)
                    Debug.LogError("����");
                stringBuilder.Append("<" 
                    + exectuteTasks[i].chapterID.ToString() + ' ' 
                    + exectuteTasks[i].nowCompletePartId.ToString() + ">");
            }
            //Debug.Log("Save = " + stringBuilder.ToString());
            Common.FileReadAndWrite.WriteFile(obtainTaskPath, stringBuilder.ToString());
            stringBuilder.Clear();
        }

        /// <summary>        /// ��ȡĿǰ�������е���������        /// </summary>
        public List<Chapter> GetExecuteChapter()
        {
            return exectuteTasks;
        }

        /// <summary>
        /// ���¼���һ�����񣬿����л���������ã������ж���������е�����
        /// </summary>
        public void ReLoadTask()
        {
            AsyncLoad.Instance.AddAction(LoadTask);
        }

        /// <summary>        /// ��ȡ�����������е�����        /// </summary>
        public Chapter GetExecuteChapterByIndex(int index)
        {
            return exectuteTasks[index];
        }

        /// <summary>        /// �������ĵ��ú���        /// </summary>
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
        /// ����½ں��������ڸ�����������͵Ľ������͵���
        /// </summary>
        /// <param name="chapter">�½�����</param>
        /// <returns>�Ƿ���ӳɹ�</returns>
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
                        Debug.Log("�����ظ�����");
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

        /// <summary>        /// ׼���½ڣ�������Ϊ�½ڵ���������        /// </summary>
        /// <param name="chapterId">�½ڱ��</param>
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