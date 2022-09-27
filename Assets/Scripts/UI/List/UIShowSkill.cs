
using UnityEngine.UI;

namespace UI
{
    /// <summary>  /// 临时实现的技能显示   /// </summary>
    public class UIShowSkill : UIUseBase
    {
        private Control.PlayerSkillControl skillControl;
        private Text text;

        protected override void Awake()
        {
            base.Awake();
            control.init += ShowSelf;
        }

        private void Start()
        {
            skillControl = Control.PlayerControl.Instance.GetComponent<Control.PlayerSkillControl>();
            text = GetComponent<Text>();
        }

        private void FixedUpdate()
        {
            text.text = skillControl.nowSkill.ToString();
        }

    }

}