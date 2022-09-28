using System.Collections.Generic;
using UnityEngine;

namespace Info
{
     [System.Serializable]
    public abstract class CharacterInfo : MonoBehaviour
    {

        public string characterName;
        [SerializeField]
        protected int hp = 10;
        public int maxHP = 10;
        [HideInInspector]
        public int sp = 10;
        public int maxSP = 10;
        /// <summary>        /// 跑步速度        /// </summary>
        public float runSpeed = 10;
        /// <summary>        /// 行走速度        /// </summary>
        public float walkSpeed = 5;
        public float rotateSpeed = 10;
        /// <summary>  /// 近战攻击距离  /// </summary>
        public float nearAttackDistance;

        /// <summary>        /// 判断角色是否死亡        /// </summary>
        public bool isDie => hp <= 0;

        /// <summary>        /// 角色得分         /// </summary>
        protected int score = 0;

        /// <summary>        /// 初始化方法        /// </summary>

        protected virtual void OnEnable()
        {
            hp = maxHP;
            sp = maxSP;
            score = 0;
        }
        /// <summary>        /// 得分        /// </summary>
        public void gainScore()
        {
            score++;
        }
        /// <summary>        /// 返回分数        /// </summary>
        public int getScore()
        {
            return score;
        }

        /// <summary>        /// 判断是否死亡        /// </summary>
        protected void CheckDead()
        {
            if(isDie)
                DealWithDeath();
        }

        /// <summary>        /// 操作生命值        /// </summary>
        public virtual void modifyHp(int dealtaHp)
        {
            hp += dealtaHp;
            CheckDead();
            return;
        }

        /// <summary>        /// 死亡后的操作        /// </summary>
        protected abstract void DealWithDeath();
    }
}