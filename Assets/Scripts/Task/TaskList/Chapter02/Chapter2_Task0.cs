using Interaction;
using System.Collections.Generic;
using UnityEngine;

namespace Task
{
    public class Chapter2_Task0 : ChapterPart
    {
        GameObject enemy;
        Chapter belongChapter;
        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            Debug.Log("进入了第二章");
            Common.SustainCoroutine.Instance.AddCoroutine(CreateEnemy);
            belongChapter = chapter;
        }

        List<Info.EnemyInfo> enemyInfos;

        public bool CreateEnemy()
        {
            //首先查找物体
            if(enemy == null)
            {
                enemy = Resources.Load<GameObject>("Prefab/Enemy");
                return false;
            }
            //使用游戏对象Map映射表来查找对象
            if(enemyInfos == null)
            {
                Vector3[] poss = new Vector3[3]
                {
                    new Vector3(263, 29, 83),
                    new Vector3(270, 29, 100),
                    new Vector3(270, 29, 120),
                };
                enemyInfos = new List<Info.EnemyInfo>();
                for(int i=0; i<3; i++)
                {
                    Control.EnemyControl enemyControl = (Control.EnemyControl)
                        Common.SceneObjectPool.Instance.GetObject("Enemy", enemy,
                        poss[i], Quaternion.identity);
                    enemyInfos.Add(enemyControl.GetComponent<Info.EnemyInfo>());
                }
                return false;
            }

            if (enemyInfos.Count == 0)
            {
                //发送个信息，表示完成任务
                AsynTaskControl.Instance.CheckChapter(belongChapter.chapterID, new InteracteInfo
                {
                    data = "2_0"
                });
                return true;
            }
            for(int i= enemyInfos.Count - 1; i >= 0; i--)
            {
                if (enemyInfos[i].isDie)
                {
                    enemyInfos.RemoveAt(i);
                    Debug.Log("死了一个");
                }
            }
            return false;
        }


        public override void ExitTaskEvent(Chapter chapter)
        {
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            Debug.Log("章节2");
            return false;
        }
    }
}