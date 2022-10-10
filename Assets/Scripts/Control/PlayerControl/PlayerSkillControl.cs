using UnityEngine;
using Skill;
using System.Reflection;

namespace Control
{
    /// <summary>
    /// 主角技能控制器，一个个选择技能过于复杂，全部放在主角管理不方便
    /// </summary>
    public class PlayerSkillControl : MonoBehaviour
    {
        private SkillManage skillManage;
        private Common.ResetInput.MyInput myInput;
        /// <summary>    /// 当前主角选中的技能    /// </summary>
        public int nowSkill;
        /// <summary>    /// 所有技能的数量      /// </summary>
        public int SkillCount
        {
            get
            {
                if (skillManage.Skills == null)
                    return 0;
                return skillManage.Skills.Count;
            }
        }
        public SkillManage SkillManage => skillManage;


        private void Start()
        {
            skillManage = GetComponent<SkillManage>();
            myInput = Common.ResetInput.MyInput.Instance;
            nowSkill = -1;
        }

        private void Update()
        {
            if (skillManage.Skills == null || skillManage.Skills.Count == 0) return;
            if(nowSkill < 0) nowSkill = 0;
            for(int i=0; i<skillManage.Skills.Count; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    nowSkill = i;
                    return;
                }
            }
        }

        /// <summary>        /// 选择当前已经选择好的技能        /// </summary>
        public void ReleaseChooseSkill()
        {
            if(nowSkill < 0) return;
            skillManage.CheckAndRelase(skillManage.Skills[nowSkill]);
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// 根据名称添加技能，传入的技能名称是技能类的名称
        /// </summary>
        /// <param name="skillClassName">技能的类名称</param>
        public void AddSkill(string skillClassName)
        {
            SkillBase skillBase = (Skill.SkillBase)
                assembly.CreateInstance("Skill." + skillClassName);
            skillManage.AddSkill(skillBase);
        }
    }
}