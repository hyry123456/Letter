using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>   /// Boss的攻击状态机，用来执行攻击行为  /// </summary>
    public class BossAttack : StateMachineBase
    {
        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState(StateMachineManage manage)
        {
        }

        public override void ExitState(StateMachineManage manage)
        {
            throw new System.NotImplementedException();
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            if(Control.PlayerControl.Instance == null || manage.SkillManage == null)
                return;
            Transform player = Control.PlayerControl.Instance.transform;
            Transform transform = manage.transform;
            Vector3 playerPos = player.position; playerPos.y = transform.position.y;
            //调整方向
            manage.transform.LookAt(playerPos);

            //List<Skill.SkillBase> skills = manage

        }
    }
}