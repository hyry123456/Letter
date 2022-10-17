using UnityEngine.EventSystems;

namespace UI
{

    public class GameBeginByTask : UIUseBase
    {
        protected override void Awake()
        {
            base.Awake();
            control.init += ShowSelf;
            widgrt.pointerClick += ChangeScene;
        }

        private void ChangeScene(PointerEventData eventData)
        {
            //没有完成第0章
            if (!Task.AsynTaskControl.Instance.CheckChapterIsComplete(0))
            {
                Control.SceneChangeControl.Instance.ChangeScene("ReceiveLetter");
            }
            //到达最后一关
            else if (Task.AsynTaskControl.Instance.CheckChapterIsComplete(3))
            {
                Control.SceneChangeControl.Instance.ChangeScene("FinalScene");
            }
            else
            {
                Control.SceneChangeControl.Instance.ChangeScene("MainScene");
            }
        }
    }
}