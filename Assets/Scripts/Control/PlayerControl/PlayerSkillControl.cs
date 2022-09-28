using UnityEngine;
using Skill;

namespace Control
{
    /// <summary>
    /// 主角技能控制器，一个个选择技能过于复杂，全部放在主角管理不方便
    /// </summary>
    public class PlayerSkillControl : MonoBehaviour
    {
        private SkillManage skillManage;
        /// <summary>    /// 当前主角选中的技能    /// </summary>
        public int nowSkill;
        private Common.ResetInput.MyInput myInput;
        private void Start()
        {
            skillManage = GetComponent<SkillManage>();
            myInput = Common.ResetInput.MyInput.Instance;
            nowSkill = 0;
        }

        private void FixedUpdate()
        {
            string namePrefit = "Alpha";
            int index = -1;
            for (int i = 1; i <= 4; i++)
            {
                if( myInput.GetButtonDown(namePrefit + i.ToString()))
                {
                    index = i - 1;
                    break;
                }
            }
            if (index == -1) return;
            nowSkill = (skillManage.Skills.Count > index) ? index : 0;
        }

        /// <summary>        /// 选择当前已经选择好的技能        /// </summary>
        public void ReleaseChooseSkill()
        {
            if (skillManage == null || skillManage.Skills == null || skillManage.Skills.Count == 0)
                return;
            skillManage.CheckAndRelase(skillManage.Skills[nowSkill]);
        }
    }
}