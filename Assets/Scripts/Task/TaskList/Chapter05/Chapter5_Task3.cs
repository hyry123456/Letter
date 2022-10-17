using Interaction;
using UnityEngine;
using Common;

namespace Task
{

    public class Chapter5_Task3 : ChapterPart
    {
        GameObject childe;

        Vector3 end = new Vector3(-398, 74, 28), begin = new Vector3(-366, 75, 28);
        float radio, transferTime = 5;
        Vector3 childeFirstBegin = new Vector3(-403, 72.25f, 42),
            childeFirstEnd = new Vector3(-525, 96, 25);


        public Chapter5_Task3()
        {
            partName = "更快，更高，更远";
        }

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            SustainCoroutine.Instance.AddCoroutine(ShowBeginDialog);
        }

        //刚开始的对话
        bool ShowBeginDialog()
        {
            UI.SmallDialog.Instance.ShowSmallDialog(chapter.GetDiglogText(4), null);
            Control.PlayerControl.Instance.StopControl();
            radio = 0;
            Control.PlayerControl.Instance.transform.position = begin;
            SustainCoroutine.Instance.AddCoroutine(TransferPlayer);
            return true;
        }

        //传送主角
        bool TransferPlayer()
        {
            radio += Time.deltaTime * (1.0f / transferTime);
            if(radio >= 1.0f)
            {
                radio = 0.0f;
                childe = SceneObjectMap.Instance.FindControlObject("AnimateChild");
                //文本
                UI.SmallDialog.Instance.ShowSmallDialog(chapter.GetDiglogText(5), null);
                //移动少爷，开始少爷动画
                SustainCoroutine.Instance.AddCoroutine(MoveChilde);
                return true;
            }
            Vector3 pos = Vector3.Lerp(begin, end, radio);
            pos.y = Control.PlayerControl.Instance.transform.position.y;
            Control.PlayerControl.Instance.transform.position = pos;
            return false;
        }

        //移动少爷，开始少爷动画
        bool MoveChilde()
        {
            radio += Time.deltaTime * (1.0f / transferTime);
            if(radio >= 1.0f)
            {
                radio = 0;
                //播放少爷的动画，不再受任务控制
                ScriptAnimate.ScriptAnimateControl animateControl =
                    childe.GetComponent<ScriptAnimate.ScriptAnimateControl>();
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo());
                animateControl.BeginUse();
                return true;
            }
            childe.transform.position = Vector3.Lerp(childeFirstBegin, childeFirstEnd, radio);
            return false;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            //触发了就直接退出
            return true;
        }
    }
}