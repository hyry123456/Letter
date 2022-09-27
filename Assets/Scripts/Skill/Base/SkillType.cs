
namespace Skill
{
    [System.Serializable]
    public enum SkillType 
    {
        /// <summary>        /// 远程攻击技能        /// </summary>
        LongDisAttack = 1,
        /// <summary>        /// 近战技能        /// </summary>
        NearDisAttack = 2,
        /// <summary>        /// 辅助技能，加伤害之类的        /// </summary>
        Auxiliary = 4,
        /// <summary>        /// 治疗技能        /// </summary>
        Heal = 8,
        /// <summary>        /// 闪避技能        /// </summary>
        Dodge = 16,
    }
}