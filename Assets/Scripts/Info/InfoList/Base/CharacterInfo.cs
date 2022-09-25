
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
        //得分
        public void gainScore()
        {
            score++;
        }
        //返回分数
        public int getScore()
        {
            return score;
        }
        //判断是否死亡
        public bool isDead()
        {
            if (hp > 0)
            {
                return false;
            }
            else
            {
                //Debug.Log("Game Over!");
                Control.SceneChangeControl.Instance.ReloadActiveScene();
                return true;
            }
        }
        //操作生命值，返回是否死亡
        public bool modifyHp(int dealtaHp)
        {
            hp += dealtaHp;
            return isDead();
        }
        
    }
}