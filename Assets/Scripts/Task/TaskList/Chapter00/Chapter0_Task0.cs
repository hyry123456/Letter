using Interaction;
using UnityEngine;

namespace Task
{
    public class Chapter0_Task0 : ChapterPart
    {

        GameObject obj;
        Chapter belongChapter;
        DefferedRender.ParticleDrawData particleDrawData;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            belongChapter = chapter;
            Debug.Log("进入了第一章");
            Common.SustainCoroutine.Instance.AddCoroutine(FindObject);
        }

        bool needBeacon;

        public bool FindObject()
        {
            //使用游戏对象Map映射表来查找对象
            obj = Common.SceneObjectMap.Instance.FindControlObject("交互对象");
            InteracteDelegate @delegate = obj.AddComponent<InteracteDelegate>();
            @delegate.interactionID = 0;
            @delegate.nonReturnAndNonParam = ()=>{
                needBeacon = false;
                AsynTaskControl.Instance.CheckChapter(belongChapter.chapterID, new InteracteInfo
                {
                    data = "0_0"
                });
                GameObject.Destroy(@delegate);
                InteractionControl.Instance.StopInteraction();      //停止交互，释放交互对象
            };
            needBeacon = true;      //持续释放信标

            particleDrawData = new DefferedRender.ParticleDrawData
            {
                beginPos = obj.transform.position,
                beginSpeed = Vector3.up * 10,
                speedMode = DefferedRender.SpeedMode.JustBeginSpeed,
                useGravity = false,
                followSpeed = true,
                lifeTime = 10,
                showTime = 8,
                frequency = 1f,
                octave = 1,
                intensity = 0.1f,
                sizeRange = Vector2.up ,
                colorIndex = (int)DefferedRender.ColorIndexMode.HighlightToAlpha,
                sizeIndex = (int)DefferedRender.SizeCurveMode.SmallToBig_Epirelief,
                textureIndex = 0,
                groupCount = 1,
            };
            Common.SustainCoroutine.Instance.AddCoroutine(CircleRelaseBeacon);
            return true;
        }

        /// <summary>        /// 循环释放信标，直到任务结束        /// </summary>
        public bool CircleRelaseBeacon()
        {
            //只要交互不触发，没有将needBeacon设置为false，就会不断的释放
            if (needBeacon)
            {
                DefferedRender.ParticleNoiseFactory.Instance.DrawPos(particleDrawData);
                return false;
            }
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("退出第一章第一小节");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if (info.data == "0_0")
                return true;
            else return false;
        }
    }
}