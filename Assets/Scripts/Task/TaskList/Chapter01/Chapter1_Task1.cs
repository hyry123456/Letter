using Interaction;
using System.Reflection;
using UnityEngine;

namespace Task
{

    public class Chapter1_Task1 : ChapterPart
    {
        DefferedRender.PostFXSetting fXSetting;
        Assembly assembly;

        int index,      //�����ж��Ƿ���ʹ�ü���
            enemyIndex, //����ȷ�����ɵĵ���λ��
            dieCount;   //����ȷ�������Ƿ�������
        //����Ԥ�Ƽ�
        GameObject origin;

        Vector3[] enemyPoss = new Vector3[8]
        {
            new Vector3(153, 8.5f, 101),
            new Vector3(155, 8.5f, 101),
            new Vector3(163, 8.5f, 101),
            new Vector3(165, 8.5f, 101),
            new Vector3(153, 8.5f, 92),
            new Vector3(155, 8.5f, 92),
            new Vector3(163, 8.5f, 92),
            new Vector3(165, 8.5f, 92),
        };

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            index = 0;
            enemyIndex = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(CheckDetective);
        }

        //������������Ƿ���ʹ����Ч������ЧҪʹ�ú��ٹر�ʱ����Ž���
        bool CheckDetective()
        {
            if(fXSetting == null)
            {
                assembly = Assembly.GetExecutingAssembly();
                Skill.SkillBase skillBase = (Skill.SkillBase)
                    assembly.CreateInstance("Skill.DetectiveView");
                Control.PlayerControl.Instance.AddSkill(skillBase);
                fXSetting = Resources.Load<DefferedRender.PostFXSetting>("Render/PostFX/PostFX");
                return false;
            }
            switch (index)
            {
                case 0:     //��ʼ״̬���ж��Ƿ���ʹ��
                    if (fXSetting.Fog.fogMaxDepth < 0.085f)
                        index++;
                    return false;
                case 1:     //ʹ�ú�ر���
                    if (fXSetting.Fog.fogMaxDepth > 0.085f)
                        index++;
                    return false;
                default:    //��ʼ���ŶԻ�
                    UI.SmallDialog.Instance.ShowSmallDialog(
                        chapter.GetDiglogText(3), () =>
                        {
                            Common.SustainCoroutine.Instance.AddCoroutine(CreateEnemy);
                            Skill.SkillBase skillBase = (Skill.SkillBase)
                                assembly.CreateInstance("Skill.SingleBullet");
                            Control.PlayerControl.Instance.AddSkill(skillBase);

                        });
                    return true;
            }

        }

        bool CreateEnemy()
        {
            if(origin == null)
            {
                origin = Resources.Load<GameObject>("Prefab/Enemy");
                return false;
            }
            //ѭ���������еĵ���
            for(; enemyIndex < enemyPoss.Length;)
            {
                Control.EnemyControl enemy = (Control.EnemyControl)
                    Common.SceneObjectPool.Instance.GetObject("Enemy", origin, enemyPoss[enemyIndex], 
                    Quaternion.identity);
                Info.EnemyInfo enemyInfo = enemy.GetComponent<Info.EnemyInfo>();
                enemyInfo.dieBehavior = EnemyDieBehavior;
                enemyIndex++;
                return false;
            }
            return true; 
        }

        /// <summary>/// ��������ʱ���е���Ϊ/// </summary>
        void EnemyDieBehavior()
        {
            AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                new InteracteInfo
                {
                    data = "1_1"
                });
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("����������");
        }

        //�����ж��Ƿ�ȫ�����˶������ˣ������˾ͽ�����һ������
        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            if(info.data == "1_1")
            {
                dieCount++;
            }
            if (dieCount == enemyPoss.Length)
                return true;
            else
                return false;
        }
    }
}