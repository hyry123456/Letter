using Interaction;
using UnityEngine;

namespace Task
{
    //章节2第一章，也就是启动一下传送祭坛传送而已
    public class Chapter2_Task0 : ChapterPart
    {
        public Chapter2_Task0()
        {
            partDescription = "前往祭坛";
        }

        float moveTime = 3f;
        float nowRadio;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            Debug.Log("开始第三章第一节");
            this.chapter = chapter;
            Common.SustainCoroutine.Instance.AddCoroutine(ShowHint);
            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(CreateAlterTrigger);
        }

        //显示提示
        bool ShowHint()
        {
            UI.SmallDialog.Instance.ShowSmallDialog(
                chapter.GetDiglogText(0), null);
            //添加物品
            Package.PackageSimple.Instance.AddItem<Package.Letter>("Letter");
            return true;
        }

        //在祭坛处添加一个触发器
        bool CreateAlterTrigger()
        {
            GameObject game = new GameObject("TempTrigger");
            game.transform.position = new Vector3(24, 6, 275);
            SphereCollider sphere = game.AddComponent<SphereCollider>();
            sphere.radius = 30; sphere.isTrigger = true;
            TrigerInteracteDelegate triger = game.AddComponent<TrigerInteracteDelegate>();
            triger.trigerDelegate = () =>
            {
                GameObject animate = Common.SceneObjectMap.Instance.FindControlObject("AlterAnimate");
                ScriptAnimate.ScriptAnimateControl animateControl = 
                    animate.GetComponent<ScriptAnimate.ScriptAnimateControl>();
                animateControl.BeginUse();
                Common.SustainCoroutine.Instance.AddCoroutine(MoveCameraToTarget);
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                   new InteracteInfo
                   {
                       data = "2_0"
                   });
            };
            return true;
        }

        Vector3 target = new Vector3(46, 0.5f, 275);
        Quaternion targetQuater = Quaternion.Euler(new Vector3(-35, -90, 0));

        bool MoveCameraToTarget()
        {
            nowRadio += Time.deltaTime * (1.0f / moveTime);
            Transform camTrans = Camera.main.transform;
            if(nowRadio >= 1.0f)
            {
                camTrans.position = target;
                camTrans.rotation = targetQuater;
                return true;
            }
            camTrans.position = Vector3.Lerp(camTrans.position, target, nowRadio);
            camTrans.rotation = Quaternion.Lerp(camTrans.rotation, targetQuater, nowRadio);
            return false;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("任务结束");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}