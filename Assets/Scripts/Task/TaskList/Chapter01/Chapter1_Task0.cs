using Interaction;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    /// <summary>
    /// ���븮�еĵ�һ�����񣬴�������NPC����ֵһЩ�Ի�����ʾһЩ�����������飬
    /// Ȼ�󴴽�һ����ү���������Ҫ�Ի���
    /// </summary>
    public class Chapter1_Task0 : ChapterPart
    {
        //��ʼ��NPC�ı�ţ���Ҫһ��ȫ�����س���
        int index;
        GameObject npc; //��ͨNPC
        GameObject childe;  //��ү
        Common.NPC_Pooling childeNPC;
        List<Common.NPC_Pooling> npcs;  //����NPC

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            Debug.Log("1");
            this.chapter = chapter;
            Common.SustainCoroutine.Instance.AddCoroutine(CreateNPC);
        }

        /// <summary> /// �������е�NPC/// </summary>
        bool CreateNPC()
        {
            InteracteDelegate interacte;
            //Ϊ��ʱ�������飬�洢ȫ����������npc
            if(npcs == null)
                npcs = new List<Common.NPC_Pooling>();
            switch (index)
            {
                case 0: //��ʼ����ү
                    if(childe == null)
                    {
                        childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                        return false;
                    }
                    childeNPC = (Common.NPC_Pooling)
                        Common.SceneObjectPool.Instance.GetObject("Childe", childe, new Vector3(155, 7.6f, 110),
                        new Vector3(156, 7.6f, 110));
                    interacte = childeNPC.gameObject.AddComponent<InteracteDelegate>();
                    interacte.interDelegate = () =>
                    {
                        UI.BigDialog.Instance.ShowBigdialog(
                            chapter.GetDiglogText(0), () =>
                            {
                                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                                    new InteracteInfo
                                    {
                                        data = "1_0"
                                    });
                            });
                    };
                    index++;
                    return false;
                case 1:             //������Ů1
                    if(npc == null)
                    {
                        npc = Resources.Load<GameObject>("Prefab/Character/NPC_Simple");
                        return false;
                    }
                    npcs.Add((Common.NPC_Pooling)
                        Common.SceneObjectPool.Instance.GetObject("NPC_Simple", npc, new Vector3(155, 0.5f, 152),
                        Quaternion.identity));
                    index++;
                    return false;
                case 2:             //������Ů2
                    npcs.Add((Common.NPC_Pooling)
                        Common.SceneObjectPool.Instance.GetObject("NPC_Simple", npc, new Vector3(155, 0.5f, 153),
                        Quaternion.identity));
                    Debug.Log(npcs.Count);
                    index++;
                    return false;
                case 3:             //������Ů1��2�ĶԻ�
                    GameObject triger = new GameObject("TempTriger");   //����һ��������
                    SphereCollider sphereCollider = triger.AddComponent<SphereCollider>();
                    triger.transform.position = new Vector3(155, 0.5f, 152.5f);
                    sphereCollider.radius = 10; sphereCollider.isTrigger = true;
                    TrigerInteracteDelegate trigeInter = triger.AddComponent<TrigerInteracteDelegate>();
                    trigeInter.trigerDelegate = () =>
                    {
                        UI.SmallDialog.Instance.ShowSmallDialog(chapter.GetDiglogText(1),
                            () =>
                            {
                                interacte = npcs[0].gameObject.AddComponent<InteracteDelegate>();
                                interacte.interDelegate = () =>
                                {
                                    UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(2), null);
                                };
                            });
                    };
                    index++;
                    return false;

            }
            return true;
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