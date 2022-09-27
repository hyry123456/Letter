
namespace Skill
{
    [System.Serializable]
    public abstract class SkillBase : ISkill
    {
        public abstract void OnSkillRelease(SkillManage mana);

        public int expendSP;
        /// <summary>        /// ��ǰ��ȴʱ�䣬���������ܿ������жϼ����ܲ����ͷ�        /// </summary>
        public float nowCoolTime;
        /// <summary>        /// ������ȴʱ�䣬��ȴʱ��û�н���������ֹͣ����        /// </summary>
        public float coolTime;
        /// <summary>        /// ��������        /// </summary>
        public string skillName;
        /// <summary>        /// �������ͣ���������        /// </summary>
        public SkillType skillType;
    }
}