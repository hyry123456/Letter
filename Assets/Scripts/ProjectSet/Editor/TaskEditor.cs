using UnityEditor;

namespace Task
{
    public class TaskEditor : Editor
    {
        /// <summary>        /// 清除所有保存的任务数据        /// </summary>
        [MenuItem("MyProjectSetting/Task/Clear")]
        public static void Clear()
        {
            AsynTaskControl.ClearData();
        }

    }
}