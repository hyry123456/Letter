using System.Reflection;
using UnityEngine;


namespace Info
{
    public class BossInfo : CharacterInfo
    {
        [SerializeField]
        /// <summary>  /// Boss的默认技能，可以不赋予值   /// </summary>
        private string[] defaultSkill;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (defaultSkill == null)
                return;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Skill.SkillManage skillManage = GetComponent<Skill.SkillManage>();
            if (skillManage == null) return;
            string prefit = "Skill.";
            for (int i = 0; i < defaultSkill.Length; i++)
            {
                Skill.SkillBase skillBase = (Skill.SkillBase)
                    assembly.CreateInstance(prefit + defaultSkill[i]);
                skillManage.AddSkill(skillBase);
            }
        }

        protected override void DealWithDeath()
        {
        }
    }

}