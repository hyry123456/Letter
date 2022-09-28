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
            Debug.Log("�����˵�һ��");
            Common.SustainCoroutine.Instance.AddCoroutine(FindObject);
        }

        bool needBeacon;

        public bool FindObject()
        {
            //ʹ����Ϸ����Mapӳ��������Ҷ���
            obj = Common.SceneObjectMap.Instance.FindControlObject("��������");
            InteracteDelegate @delegate = obj.AddComponent<InteracteDelegate>();
            @delegate.interactionID = 0;
            @delegate.nonReturnAndNonParam = ()=>{
                needBeacon = false;
                AsynTaskControl.Instance.CheckChapter(belongChapter.chapterID, new InteracteInfo
                {
                    data = "0_0"
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
                sizeRange = Vector2.up ,
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
            //ֻҪ������������û�н�needBeacon����Ϊfalse���ͻ᲻�ϵ��ͷ�
            if (needBeacon)
            {
                DefferedRender.ParticleNoiseFactory.Instance.DrawPos(particleDrawData);
                return false;
            }
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("�˳���һ�µ�һС��");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if (info.data == "0_0")
                return true;
            else return false;
        }
    }
}