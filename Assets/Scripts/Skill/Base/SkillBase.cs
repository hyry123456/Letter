
namespace Skill
{
    public abstract class SkillBase : ISkill
    {
        public abstract void OnSkillRelease(SkillManage mana);

        public int expendSP;
        /// <summary>        /// 当前冷却时间，用来给技能控制器判断技能能不能释放        /// </summary>
        public float nowCoolTime;
        /// <summary>        /// 技能冷却时间，冷却时间没有结束，不能停止技能        /// </summary>
        public float coolTime;
        /// <summary>        /// 技能的释放时间，释放技能后经过该时间会调用技能结束        /// </summary>
        public float relaseTime;
        /// <summary>        /// 技能名称        /// </summary>
        public string skillName;
        /// <summary>        /// 技能类型，用来分类        /// </summary>
        public SkillType skillType;
    }
}