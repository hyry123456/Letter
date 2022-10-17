using Interaction;
using UnityEngine;

namespace Task
{
    //第三节，加载下对话
    public class Chapter3_Task2 : ChapterPart
    {
        GameObject origin;
        Common.NPC_Pooling npc;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            Common.SustainCoroutine.Instance.AddCoroutine(ShowChildAndDialog);
        }


        bool ShowChildAndDialog()
        {
            if(origin == null)
            {
                origin = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            Control.PlayerControl.Instance.transform.position = new Vector3(-211, 23, 277);

            npc = Common.SceneObjectPool.Instance.GetObject<Common.NPC_Pooling>(
                "Childe", origin, new Vector3(-270, 1.5f, 313),
                Quaternion.Euler(new Vector3(0, 180, 0)));
            InteracteDelegate interacte = npc.gameObject.AddComponent<InteracteDelegate>();
            interacte.interDelegate = () =>
            {
                UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(3),
                    () =>
                    {
                        AsynTaskControl.Instance.CheckChapter(chapter.chapterID, new InteracteInfo());
                        Control.SceneChangeControl.Instance.ChangeScene("FinalScene");
                    });
            };
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}