using Interaction;
using UnityEngine;
using Common;

namespace Task
{
    /// <summary>
    /// 试炼正式章节，不要设置的太难，不然很难搞
    /// </summary>
    public class Chapter4_Task1 : ChapterPart
    {

        GameObject childe;
        NPC_Pooling child_Pooling;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            SustainCoroutine.Instance.AddCoroutine(CreateChildAndSetDialog);
            if (isLoaded)
                SustainCoroutine.Instance.AddCoroutine(MovePlayer);
            SustainCoroutine.Instance.AddCoroutine(AddSkillInteratce);
        }

        /// <summary>
        /// 一开始创建少爷，进行对话交互设置，因为之前已经提示过了，这里就不提示了
        /// </summary>
        bool CreateChildAndSetDialog()
        {
            if (childe == null)
            {
                childe = Resources.Load<GameObject>("Prefab/Character/Childe");
                return false;
            }
            child_Pooling = SceneObjectPool.Instance.GetObject<NPC_Pooling>(
                "Childe", childe, new Vector3(155, 7.6f, 110), new Vector3(156, 7.6f, 110));
            InteracteDelegate interacte = child_Pooling.gameObject.AddComponent<InteracteDelegate>();
            //主角交互
            interacte.interDelegate = () =>
            {
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo { data = "4_1" });
            };
            
            return true;
        }

        bool AddSkillInteratce()
        {
            GameObject add = SceneObjectMap.Instance.FindControlObject("Chapter4_AddSkill");
            add.gameObject.AddComponent<InteracteDelegate>().interDelegate = () =>
            {
                Package.PackageSimple.Instance.AddItem<Package.Benediction>("Benediction");
                UI.SmallDialog.Instance.ShowSmallDialog(chapter.GetDiglogText(7), null);
            };
            return true;
        }

        //移动主角，不用每一次重新跑
        bool MovePlayer()
        {
            Control.PlayerControl.Instance.transform.position = new Vector3(-3, 1, 183);
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("支线完成");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if(info.data == "4_1") {
                if (Package.PackageSimple.Instance.CheckItemByName("坤家的祝福"))
                {
                    UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(6), () =>
                    {
                        //开启2段跳
                        Control.PlayerControl.Instance.Motor.maxAirJumps = 1;
                    });
                    return true;
                }
                else
                {
                    UI.BigDialog.Instance.ShowBigdialog(chapter.GetDiglogText(5), null);
                    return false;
                }
            }
            return false;
        }
    }
}