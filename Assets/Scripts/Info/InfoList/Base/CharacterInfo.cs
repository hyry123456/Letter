
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
     [System.Serializable]
    public class CharacterInfo : MonoBehaviour
    {

        public string characterName;
        protected int hp = 10;
        public int maxHP = 10;
        [HideInInspector]
        public int sp = 10;
        public int maxSP = 10;
        public List<string> skills;
        /// <summary>        /// 跑步速度        /// </summary>
        public float runSpeed = 10;
        /// <summary>        /// 行走速度        /// </summary>
        public float walkSpeed = 5;
        public float rotateSpeed = 10;

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
        public bool isDead()
        {
            if (hp > 0)
            {
                return false;
            }
            else
            {
                DealWithDeath();
                return true;
            }
        }
        /// <summary>        /// 操作生命值        /// </summary>
        /// <returns>        ///是否死亡           /// </returns>
        public bool modifyHp(int dealtaHp)
        {
            hp += dealtaHp;
            return isDead();
        }

        /// <summary>        /// 死亡后的操作        /// </summary>
        private void DealWithDeath()
        {
            //Debug.Log("Game Over!");
            //Control.SceneChangeControl.Instance.ReloadActiveScene();//暂时注释掉死亡，便于测试
        }

    }
}