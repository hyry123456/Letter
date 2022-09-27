
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class SkillManage : MonoBehaviour
    {
        [SerializeField]
        /// <summary>        /// 技能释放者的技能列表        /// </summary>
        protected List<SkillBase> skills;
        /// <summary>        /// 公开技能释放者的所有技能，但是不允许修改        /// </summary>
        public List<SkillBase> Skills
        {
            get
            {
                return skills;
            }
        }

        private Info.CharacterInfo characterInfo;
        /// <summary>        /// 技能释放者的技能信息基类        /// </summary>
        public Info.CharacterInfo CharacterInfo
        {
            get
            {
                if(characterInfo == null)
                    characterInfo = GetComponentInChildren<Info.CharacterInfo>();
                return characterInfo;
            }
        }

        protected virtual void Start()
        {
            characterInfo = GetComponent<Info.CharacterInfo>();
        }

        protected void FixedUpdate()
        {
            if (skills == null) return;
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].nowCoolTime > 0)
                {
                    skills[i].nowCoolTime -= Time.fixedDeltaTime;
                }
            }
        }

        /// <summary>        /// 检查并释放该技能        /// </summary>
        /// <param name="skill">技能对象</param>
        public void CheckAndRelase(SkillBase skill)
        {
            if (skill == null) return;
            if(skill.nowCoolTime <= 0)
            {
                skill.nowCoolTime = skill.coolTime;
                skill.OnSkillRelease(this);
            }
        }

        /// <summary>       /// 获得技能列表中的指定技能        /// </summary>
        /// <param name="index">技能编号</param>
        /// <returns>技能对象</returns>
        public SkillBase GetSkillByIndex(int index)
        {
            if(index > 0 && skills != null && skills.Count > index)
            {
                SkillBase skill = skills[index];
                return skill;
            }
            return null;
        }

        /// <summary>        /// 获得所有可以使用的技能        /// </summary>
        /// <returns>技能列表</returns>
        public List<SkillBase> GetCanUseSkill()
        {
            if (skills == null) return null;
            List<SkillBase> canUse = new List<SkillBase>(skills.Count);
            for(int i=0; i<skills.Count; i++)
            {
                if(skills[i].nowCoolTime <= 0)
                {
                    canUse.Add(skills[i]);
                }
            }
            return canUse;
        }

        /// <summary>
        /// 获得可以使用的技能，且技能类型与传入类型对应，需要注意的是支持多匹配，
        /// 也就是通过按位与可以匹配多种技能类型
        /// </summary>
        /// <param name="type">技能类型</param>
        /// <returns>可以使用的技能列表，注意为空的情况</returns>
        public List<SkillBase> GetCanUseSkillByType(SkillType type)
        {
            if (skills == null) return null;
            List<SkillBase> canUse = new List<SkillBase>();
            for(int i=0; i<skills.Count; i++)
            {
                if(skills[i].nowCoolTime <= 0 && (skills[i].skillType & type) != 0)
                {
                    canUse.Add(skills[i]);
                }
            }
            return canUse;
        }

        /// <summary> /// 添加技能，根据名称进行技能剔除，避免重复添加   /// </summary>
        public void AddSkill(SkillBase skill)
        {
            if(skills == null)
            {
                skills = new List<SkillBase>();
                skills.Add(skill);
                return;
            }
            for(int i=0; i<skills.Count; i++)
            {
                if (skills[i].skillName == skill.skillName)
                    return;
            }
            skills.Add(skill);
        }
    }
}