using Interaction;
using UnityEngine;

namespace Task
{
    public class Chapter1_Task0 : ChapterPart
    {
        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            Debug.Log("��ʼ�ڶ���");
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return false;
        }
    }
}