
namespace Skill
{
    [System.Serializable]
    public abstract class SkillBase : ISkill
    {
        public abstract void OnSkillRelease(SkillManage mana);

        public int expendSP;
        /// <summary>        /// 当前冷却时间，用来给技能控制器判断技能能不能释放        /// </summary>
        public float nowCoolTime;
        /// <summary>        /// 技能冷却时间，冷却时间没有结束，不能停止技能        /// </summary>
        public float coolTime;
        /// <summary>        /// 技能名称        /// </summary>
        public string skillName;
        /// <summary>        /// 技能类型，用来分类        /// </summary>
        public SkillType skillType;

        /// <summary>
        /// 初始化案例
        /// </summary>
        //public SkillBase()
        //{
        //    expendSP = 0;
        //    nowCoolTime = 0;
        //    coolTime = 0;
        //    skillName = "Skill Name;
        //    skillType = SkillType.TYPE;
        //}
    }
}