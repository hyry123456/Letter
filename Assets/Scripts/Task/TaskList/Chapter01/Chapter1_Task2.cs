using Interaction;
using UnityEngine;

namespace Task
{

    public class Chapter1_Task2 : ChapterPart
    {
        GameObject childe;  //少爷
        Common.NPC_Pooling childePooling;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            if (isLoaded)   //非直接加载，为第二次加载，初始化技能
            {
                Common.SustainCoroutine.Instance.AddCoroutine(AddPlayerSkill);
            }
            Common.SustainCoroutine.Instance.AddCoroutine(AddNPCs);
        }

        //给主角添加技能
        bool AddPlayerSkill()
        {
            Control.PlayerControl.Instance.AddSkill("SingleBullet");
            Control.PlayerControl.Instance.AddSkill("DetectiveView");
            Control.PlayerControl.Instance.AddSkill("WaveSickle");
            Control.PlayerControl.Instance.transform.position = new Vector3(163, 1, 152);
            return true;
        }

        bool AddNPCs()
        {
            if(childe == null)
            {
                childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            childePooling = (Common.NPC_Pooling)Common.SceneObjectPool.Instance.GetObject(
                "Childe", childe, new Vector3(155, 7.6f, 110),
                        new Vector3(156, 7.6f, 110));
            InteracteDelegate interacte = childePooling.gameObject.AddComponent<InteracteDelegate>();
            interacte.interDelegate = () =>
            {
                UI.BigDialog.Instance.ShowBigdialog(
                    chapter.GetDiglogText(4), () =>
                    {
                        AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                            new InteracteInfo
                            {
                                data = "1_2"
                            });
                        interacte = childePooling.GetComponent<InteracteDelegate>();
                        //删除掉该交互，因为该对话结束了
                        GameObject.Destroy(interacte);
                    });
            };
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            childePooling.CloseObject();
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}