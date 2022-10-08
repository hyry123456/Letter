using Interaction;
using UnityEngine;

namespace Task
{
    public class Chapter0_Task0 : ChapterPart
    {
        Common.NPC_Pooling npc;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            if (!isLoaded)
            {
                Common.SustainCoroutine.Instance.AddCoroutine(ShowDialog);
            }
            Common.SustainCoroutine.Instance.AddCoroutine(BeginNPCDialog);
        }

        /// <summary>     /// ��ʾ�Ի�����ʾ��ҽ����ķ�ʽ     /// </summary>
        bool ShowDialog()
        {
            //������ʾ�¶Ի�������Ҫʲô�������
            UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(0), null);
            return true;
        }

        bool BeginNPCDialog()
        {
            GameObject NPC = Resources.Load<GameObject>("Prefab/Character/NPC_Simple");
            npc = (Common.NPC_Pooling)Common.SceneObjectPool.Instance.GetObject(
                "NPC_Simple", NPC, new Vector3(280, 0.5f, 180), Quaternion.identity);
            InteracteDelegate interacte = npc.gameObject.AddComponent<InteracteDelegate>();
            interacte.nonReturnAndNonParam = () =>
            {
                //���ȿ�ʼ�Ի���˵һ�·���������
                UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(1),
                    () =>
                    {
                        //�Ի���������������
                        AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                            new InteracteInfo
                            {
                                data = "0_0"
                            });
                    });

            };

            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            npc.CloseObject();
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if (info.data == "0_0")
                return true;
            else return false;
        }
    }
}