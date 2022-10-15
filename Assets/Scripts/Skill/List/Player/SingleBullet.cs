
using UnityEngine;

namespace Skill
{
    public class SingleBullet : SkillBase
    {
        GameObject originBullet;

        public SingleBullet()
        {
            expendSP = 0;
            nowCoolTime = 0;
            coolTime = 1;
            skillName = "Single Bullet";
            skillType = SkillType.LongDisAttack;
            originBullet = Resources.Load<GameObject>("Prefab/poolingBullet");
        }

        public override void OnSkillRelease(SkillManage mana)
        {
            Camera camera = Camera.main;
            if (camera == null) return;
            Bullet_Pooling bullet_Pooling = 
                Common.SceneObjectPool.Instance.GetObject<Bullet_Pooling>("Pooling_Bullet", 
                originBullet, mana.transform.position + camera.transform.forward, 
                camera.transform.rotation);
            bullet_Pooling.attackTargetTag = "Enemy";
        }
    }
}