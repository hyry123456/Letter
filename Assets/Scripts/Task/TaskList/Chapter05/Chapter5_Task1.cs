using Interaction;
using UnityEngine;
using Common;

namespace Task
{

    public class Chapter5_Task1 : ChapterPart
    {
        GameObject childe;
        NPC_Pooling childe_Pooling;

        public Chapter5_Task1()
        {
            partName = "触及更远";
        }

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            if(!isLoaded)
                SustainCoroutine.Instance.AddCoroutine(ShowDialog);
            else
                SustainCoroutine.Instance.AddCoroutine(TransferPlayer);
            //添加结束交互
            SustainCoroutine.Instance.AddCoroutine(AddEndInteracte);
        }

        bool TransferPlayer()
        {
            //改变位置
            Control.PlayerControl.Instance.transform.position = new Vector3(-53, -8, 24);
            return true;
        }

        /// <summary>   /// 添加少爷对话   /// </summary>
        bool ShowDialog()
        {
            if(childe == null)
            {
                childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            childe_Pooling = SceneObjectPool.Instance.GetObject<NPC_Pooling>(
               "Childe", childe, new Vector3(-53, -9.5f, 29), Quaternion.Euler(new Vector3(0, -180, 0)));
            InteracteDelegate interacte = childe_Pooling.gameObject.AddComponent<InteracteDelegate>();
            interacte.interDelegate = () =>
            {
                UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(2), null);
            };
            return true;
        }

        /// <summary>        /// 添加结束任务触发        /// </summary>
        bool AddEndInteracte()
        {
            GameObject temCollen = new GameObject("TempCollider");
            BoxCollider collider = temCollen.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(16, 10, 15);
            temCollen.transform.position = new Vector3(-206, 25, 32.6f);
            TrigerInteracteDelegate trigerInteracte = temCollen.AddComponent<TrigerInteracteDelegate>();
            trigerInteracte.trigerDelegate = () =>
            {
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo());
            };

            return true;
        }


        public override void ExitTaskEvent(Chapter chapter)
        {
            if(childe_Pooling == null)
                return;
            childe_Pooling.CloseObject();
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            //触发了就直接退出
            return true;
        }
    }
}