using Interaction;
using UnityEngine;
using Common;

namespace Task
{

    public class Chapter4_Task0 : ChapterPart
    {
        GameObject childe;
        NPC_Pooling child_Pooling;


        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            if (!isLoaded)
                SustainCoroutine.Instance.AddCoroutine(CreateChildAndDialog);
            else
                SustainCoroutine.Instance.AddCoroutine(CreateChilde);
            SustainCoroutine.Instance.AddCoroutine(AddNPC);
        }

        //创建少爷并且添加一下少爷的对话，这个是第一次加载时进行的
        bool CreateChildAndDialog()
        {
            if (childe == null)
            {
                childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            child_Pooling = SceneObjectPool.Instance.GetObject<NPC_Pooling>(
                "Childe", childe, new Vector3(155, 7.6f, 110), new Vector3(156, 7.6f, 110));
            InteracteDelegate interacte = child_Pooling.gameObject.AddComponent<InteracteDelegate>();
            interacte.interDelegate = () =>
            {
                UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(0),
                    //改变交互
                    () =>
                    {
                        GameObject.Destroy(interacte);      //清除当前交互
                        //换一个交互
                        interacte = child_Pooling.gameObject.AddComponent<InteracteDelegate>();
                        interacte.interDelegate = () =>
                        {
                            AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                                new InteracteInfo { data = "4_0" });
                        };


                    });
            };
            return true;
        }

        //创建少爷，在非第一次时进行
        bool CreateChilde()
        {
            if (childe == null)
            {
                childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            child_Pooling = SceneObjectPool.Instance.GetObject<NPC_Pooling>(
                "Childe", childe, new Vector3(155, 7.6f, 110), new Vector3(156, 7.6f, 110));
            InteracteDelegate interacte = child_Pooling.gameObject.AddComponent<InteracteDelegate>();
            interacte.interDelegate = () =>
            {
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo { data = "4_0" });
            };
            //移动主角
            Control.PlayerControl.Instance.transform.position = new Vector3(210, 1, 108);

            return true;
        }

        //创建交互
        bool AddNPC()
        {
            //添加获取信件交互
            GameObject addLetter = SceneObjectMap.Instance.FindControlObject("Chapter4_AddLetter");
            addLetter.AddComponent<InteracteDelegate>().interDelegate = () =>
            {
                Package.PackageSimple.Instance.AddItem<Package.SkillLetter>("SkillLetter");
                UI.SmallDialog.Instance.ShowSmallDialog(chapter.GetDiglogText(1), null);
            };

            //添加试炼之地前面的NPC
            GameObject oldMan = SceneObjectMap.Instance.FindControlObject("OldMan");
            oldMan.AddComponent<InteracteDelegate>().interDelegate = () =>
            {
                UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(4), null);
            };
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            //关闭少爷
            child_Pooling?.CloseObject();
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            //一开始的交互
            if(info.data == "4_0")
            {
                if (Package.PackageSimple.Instance.CheckItemByName("祝福试炼地的信件"))
                {
                    UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(3),
                        () =>
                        {
                            AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                                new InteracteInfo { data = "4_1" });
                        });
                }
                else
                {
                    UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(2), null);
                }
                return false;
            }
            if (info.data == "4_1")
                return true;
            return false;
        }
    }
}