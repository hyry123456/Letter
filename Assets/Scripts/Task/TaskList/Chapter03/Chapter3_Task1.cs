using Interaction;
using UnityEngine;

namespace Task
{
    //第二节，生成小怪以及碰撞
    public class Chapter3_Task1 : ChapterPart
    {
        GameObject origin;
        Control.EnemyControl[] enemys;

        Vector3[] poss = new Vector3[10]
        {
            new Vector3(-75, 54, 251),
            new Vector3(-65, 54, 253),
            new Vector3(-65, 54, 300),
            new Vector3(-76, 54, 304),
            new Vector3(-140, 54, 230),
            new Vector3(-120, 54, 278f),
            new Vector3(-120, 54, 270f),
            new Vector3(-198, 54, 230f),
            new Vector3(-178, 54, 288f),
            new Vector3(-190, 54, 313f),
        };

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            this.chapter = chapter;
            if (isLoaded)
                Common.SustainCoroutine.Instance.AddCoroutine(MovePlayer);

            Common.SustainCoroutine.Instance.AddCoroutine(CreateEnemy);
            Common.SustainCoroutine.Instance.AddCoroutine(CreateTrigger);
        }

        bool MovePlayer()
        {
                //加载下位置，不要重新开始了
            Control.PlayerControl.Instance.transform.position = new Vector3(-74, 54, 275);
            //添加技能
            Control.PlayerControl.Instance.AddSkill("HookRope");
            Control.PlayerControl.Instance.AddSkill("SingleBullet");
            Control.PlayerControl.Instance.AddSkill("WaveSickle");
            return true;
        }

        //创建敌人
        bool CreateEnemy()
        {
            if(origin == null)
            {
                origin = Resources.Load<GameObject>("Prefab/Enemy");
                return false;
            }
            if(enemys == null)
            {
                enemys = new Control.EnemyControl[poss.Length];
                return false;
            }
            for(int i=0; i<poss.Length; i++)
            {
                enemys[i] = (Control.EnemyControl)Common.SceneObjectPool.Instance.GetObject(
                    "Enemy", origin, poss[i], Quaternion.identity);
                enemys[i].transform.localScale = Vector3.one * 4;
                Info.CharacterInfo character = enemys[i].GetComponent<Info.CharacterInfo>();
                character.attackDistance = 10;
            }
            return true;
        }

        //创建触发器
        bool CreateTrigger()
        {
            GameObject tem = new GameObject("TempColler");
            tem.transform.position = new Vector3(-200, 58, 275);
            BoxCollider box = tem.AddComponent<BoxCollider>();
            box.size = new Vector3(10, 15, 10);
            box.isTrigger = true;
            TrigerInteracteDelegate interace = tem.AddComponent<TrigerInteracteDelegate>();
            interace.trigerDelegate = () =>
            {
                AsynTaskControl.Instance.CheckChapter(chapter.chapterID,
                    new InteracteInfo { data = "3_1" });
            };
            return true;
        }

        public override void ExitTaskEvent(Chapter chapter)
        {
            for(int i=0; i<enemys.Length; i++)
            {
                enemys[i].transform.localPosition = Vector3.one;
            }
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}