
namespace Skill {
    public interface ISkill
    {
        /// <summary>        /// 技能释放方法        /// </summary>
        /// <param name="mana">技能拥有者</param>
        void OnSkillRelease(SkillManage mana);
    }
}