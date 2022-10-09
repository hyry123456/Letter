using Interaction;
using System.Reflection;
using UnityEngine;

namespace Task
{

    public class Chapter1_Task1 : ChapterPart
    {
        DefferedRender.PostFXSetting fXSetting;
        Assembly assembly;

        int index,      //用来判断是否有使用技能
            enemyIndex, //用来确定生成的敌人位置
            dieCount;   //用来确定敌人是否死完了
        //敌人预制件
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

        //用来检查主角是否有使用特效，且特效要使用后再关闭时任务才结束
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
                case 0:     //初始状态，判断是否有使用
                    if (fXSetting.Fog.fogMaxDepth < 0.085f)
                        index++;
                    return false;
                case 1:     //使用后关闭了
                    if (fXSetting.Fog.fogMaxDepth > 0.085f)
                        index++;
                    return false;
                default:    //开始播放对话
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
            //循环创建所有的敌人
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

        /// <summary>/// 敌人死亡时进行的行为/// </summary>
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
            Debug.Log("敌人死完了");
        }

        //用来判断是否全部敌人都死完了，死完了就进入下一个任务
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