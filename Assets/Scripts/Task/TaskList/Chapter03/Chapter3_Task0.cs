using Interaction;
using UnityEngine;

namespace Task
{
    //第一节，提示场景信息
    public class Chapter3_Task0 : ChapterPart
    {
        GameObject origin;
        Common.NPC_Pooling npc;
        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            if (!isLoaded)
            {
                Common.SustainCoroutine.Instance.AddCoroutine(BeginDialogAndShowNPC);
            }
            else
            {
                Common.SustainCoroutine.Instance.AddCoroutine(AddSkill);
            }
            Common.SustainCoroutine.Instance.AddCoroutine(AddTriger);
        }

        //开始对话并显示NPC
        bool BeginDialogAndShowNPC()
        {
            if(origin == null)
            {
                origin = Resources.Load<GameObject>("Prefab/Character/NPC_Simple");
                return false;
            }
            if(npc == null)
            {
                npc = Common.SceneObjectPool.Instance.GetObject<Common.NPC_Pooling>(
                    "NPC_Simple", origin, new Vector3(137, 21, 293), Quaternion.identity);
                return false;
            }

            UI.SmallDialog.Instance.ShowSmallDialog(
                chapter.GetDiglogText(0), () =>
                {
                    InteracteDelegate interacte = npc.gameObject.AddComponent<InteracteDelegate>();
                    interacte.interDelegate = () =>
                    {
                        UI.BigDialog.Instance.ShowBigdialog(
                            chapter.GetDiglogText(1), () =>
                            {
                                Common.SustainCoroutine.Instance.AddCoroutine(AddSkill);
                            });
                    };
                });
            return true;
        }

        //添加技能
        bool AddSkill()
        {
            Control.PlayerControl.Instance.AddSkill("HookRope");
            Control.PlayerControl.Instance.AddSkill("SingleBullet");
            Control.PlayerControl.Instance.AddSkill("WaveSickle");
            return true;
        }

        //添加到达下一节的碰撞器
        bool AddTriger()
        {
            GameObject tem = new GameObject("TempColler");
            tem.transform.position = new Vector3(-60, 58, 275);
            BoxCollider box = tem.AddComponent<BoxCollider>();
            box.size = new Vector3(1, 15, 10);
            box.isTrigger = true;
            TrigerInteracteDelegate interace = tem.AddComponent<TrigerInteracteDelegate>();
            interace.trigerDelegate = () =>
            {
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo { data = "3_0" });
            };
            return true;
        }
        

        public override void ExitTaskEvent(Chapter chapter)
        {
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            Debug.Log("达到第一节");
            return true;
        }
    }
}