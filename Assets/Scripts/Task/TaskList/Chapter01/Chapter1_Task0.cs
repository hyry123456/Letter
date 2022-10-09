using Interaction;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    /// <summary>
    /// 进入府中的第一个任务，创建几个NPC，赋值一些对话，提示一些昨晚发生的事情，
    /// 然后创建一个少爷，这个是主要对话者
    /// </summary>
    public class Chapter1_Task0 : ChapterPart
    {
        //初始化NPC的编号，不要一次全部加载出来
        int index;
        GameObject npc; //普通NPC
        GameObject childe;  //少爷
        Common.NPC_Pooling childeNPC;
        List<Common.NPC_Pooling> npcs;  //其他NPC

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            Debug.Log("1");
            this.chapter = chapter;
            Common.SustainCoroutine.Instance.AddCoroutine(CreateNPC);
        }

        /// <summary> /// 创建所有的NPC/// </summary>
        bool CreateNPC()
        {
            InteracteDelegate interacte;
            //为空时创建数组，存储全部创建过的npc
            if(npcs == null)
                npcs = new List<Common.NPC_Pooling>();
            switch (index)
            {
                case 0: //初始化少爷
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
                                interacte = childeNPC.GetComponent<InteracteDelegate>();
                                //删除掉该交互，因为该对话结束了
                                GameObject.Destroy(interacte);
                            });
                    };
                    index++;
                    return false;
                case 1:             //加载侍女1
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
                case 2:             //加载侍女2
                    npcs.Add((Common.NPC_Pooling)
                        Common.SceneObjectPool.Instance.GetObject("NPC_Simple", npc, new Vector3(155, 0.5f, 153),
                        Quaternion.identity));
                    Debug.Log(npcs.Count);
                    index++;
                    return false;
                case 3:             //加载侍女1与2的对话
                    GameObject triger = new GameObject("TempTriger");   //创建一个触发器
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
            ///清除掉这些这些NPC
            for(int i=0; i<npcs.Count; i++)
            {
                npcs[i].CloseObject();
            }
            npcs.Clear(); npcs = null;
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}