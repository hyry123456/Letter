using Control;
using UnityEngine;

namespace Info
{
    public class EnemyInfo : CharacterInfo
    {
        /// <summary>   /// ���˵Ŀ��Ӿ���   /// </summary>
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