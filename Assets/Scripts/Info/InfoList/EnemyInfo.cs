using Control;
using UnityEngine;

namespace Info
{
    public class EnemyInfo : CharacterInfo
    {
        /// <summary>   /// 敌人的可视距离   /// </summary>
        public float seeDistance = 50;
        EnemyControl enemyControl;

        protected override void OnEnable()
        {
            base.OnEnable();
            enemyControl = GetComponent<EnemyControl>();
        }

        public override void modifyHp(int dealtaHp)
        {
            base.modifyHp(dealtaHp);
        }

        protected override void DealWithDeath()
        {
            enemyControl.CloseObject();
        }
    }
}