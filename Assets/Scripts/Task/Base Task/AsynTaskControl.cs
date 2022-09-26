
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Task
{

    struct TaskInfo
    {
        public string Name;
        /// <summary>        /// ����״̬��0=δ��ʼ��1=��ʼ�С�2=���        /// </summary>
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

        /// <summary>        /// ��������        /// </summary>
        private static string allTaskPath = Application.streamingAssetsPath + "/Task/AllTask.task";
        /// <summary>        /// ��ɵ�����        /// </summary>
        private static string completeTaskPath = Application.streamingAssetsPath + "/Task/CompleteTask.task";
        /// <summary>        /// �����е�����Ĵ洢�ļ�        /// </summary>
        private static string obtainTaskPath = Application.streamingAssetsPath + "/Task/ObtainTask.task";

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

        /// <summary>        /// �����е�����        /// </summary>
        private List<Chapter> exectuteTasks;


        /// <summary>        /// ���������ӳ��������<��ţ�����>        /// </summary>
        private Dictionary<int, TaskInfo> taskMap;

        private void LoadTask()
        {
            LoadAllTask();          //�������е�����
            LoadObtainTask();       //�������г��е�����
            LoadCompleteTask();     //ȷ����ɵ�����
            ReadyTask();            //��ʼִ��������
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
                //���������½ڵ�ӳ���ϵ
                taskMap.Add(chapterTask.chapterID, new TaskInfo { Name = allTasks[i].Trim(), state = 0 });
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
                string chapterPrefix = "FireControl.Task.";
                Assembly assembly = Assembly.GetExecutingAssembly();
                for (int i = 0; i < task.Count; i++)
                {
                    string[] tremps = task[i].Split(' ');
                    int index = int.Parse(tremps[0]);
                    TaskInfo taskInfo = taskMap[index];
                    taskInfo.state = 1;     //������
                    Chapter chapterTask = 
                        (Chapter)assembly.CreateInstance(chapterPrefix + taskInfo.Name);
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
                    string chapterPrefix = "FireControl.Task.";
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    for (int i = 0; i < comTasks.Length; i++)
                    {
                        int value;
                        if (int.TryParse(comTasks[i], out value))
                        {
                            taskMap.TryGetValue(value, out TaskInfo task);
                            task.state = 2;     //��ʾ���
                            taskMap[value] = task;
                            Chapter chapterTask = 
                                (Chapter)assembly.CreateInstance(chapterPrefix + task.Name);
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
        /// ��������Ƿ���ɣ�����������Է�֧Ϊ���ݵģ�����ֻ��Ҫ���ǰ���Ƿ��Ѿ���ɾ͹��ˣ�
        /// �������ֻ�ṩ���ķ���
        /// </summary>
        /// <param name="taskId">����ı�ţ�ע��ñ��ֵҪΨһ</param>
        public bool CheckTaskIsComplete(int taskId)
        {
            return taskMap[taskId].state == 2;
        }

        /// <summary>        /// ������ɵ�ͨ����Ϊ�����������˳���Ȼ�󱣴��ļ�        /// </summary>
        /// <param name="chapter">Ҫ��ɵ�����</param>
        public void CompleteChapter(Chapter chapter)
        {
            exectuteTasks.Remove(chapter);
            //�����˳�����
            chapter.ExitChapter();
            TaskInfo info = taskMap[chapter.chapterID];
            info.state = 2;         //�������
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