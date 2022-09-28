using Interaction;
using UnityEngine;


namespace Task
{
    public class Chapter0_Task1 : ChapterPart
    {
        DefferedRender.ParticleDrawData particleDrawData;
        GameObject obj;
        Chapter belongChapter;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            belongChapter = chapter;
            Common.SustainCoroutine.Instance.AddCoroutine(FindObject);
            Debug.Log("�����һ�µڶ���");
        }

        bool needBeacon;

        public bool FindObject()
        {
            obj = Common.SceneObjectMap.Instance.FindControlObject("��������");
            obj.transform.position = new Vector3(255, 27, 50);
            InteracteDelegate @delegate = obj.AddComponent<InteracteDelegate>();
            @delegate.interactionID = 1;
            @delegate.nonReturnAndNonParam = () => {
                needBeacon = false;
                AsynTaskControl.Instance.CheckChapter(belongChapter.chapterID, new InteracteInfo
                {
                    data = "0_1"
                });
                GameObject.Destroy(@delegate);
                InteractionControl.Instance.StopInteraction();      //ֹͣ�������ͷŽ�������
            };
            needBeacon = true;      //�����ͷ��ű�

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
                sizeRange = Vector2.up,
                colorIndex = (int)DefferedRender.ColorIndexMode.HighlightToAlpha,
                sizeIndex = (int)DefferedRender.SizeCurveMode.SmallToBig_Epirelief,
                textureIndex = 0,
                groupCount = 1,
            };
            Common.SustainCoroutine.Instance.AddCoroutine(CircleRelaseBeacon);
            return true;
        }

        /// <summary>        /// ѭ���ͷ��űֱ꣬���������        /// </summary>
        public bool CircleRelaseBeacon()
        {
            if (needBeacon)
            {
                DefferedRender.ParticleNoiseFactory.Instance.DrawPos(particleDrawData);
                return false;
            }
            return true;
        }


        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("��һ�µڶ�С�����");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if (info.data == "0_1")
                return true;
            return false;
        }
    }
}