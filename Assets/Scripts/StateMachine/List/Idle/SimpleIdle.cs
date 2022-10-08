using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Idle/SimpleIdle")]
    /// <summary>  /// 一个案例的待机  /// </summary>
    public class SimpleIdle : StateMachineBase
    {
        /// <summary> /// 下一个状态   /// </summary>
        public StateMachineBase nextState;
        private Info.EnemyInfo enemyInfo;

        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            if(Control.PlayerControl.Instance == null) return null;
            if (enemyInfo == null) return null;
            float sqrDis = (Control.PlayerControl.Instance.transform.position
                - manage.transform.position).sqrMagnitude;
            if (sqrDis < enemyInfo.seeDistance * enemyInfo.seeDistance)
                return nextState;
            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            enemyInfo = manage.GetComponent<Info.EnemyInfo>();
        }

        public override void ExitState(StateMachineManage manage)
        {
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            if (manage.AnimateManage == null) return;
            manage.AnimateManage.PlayAnimate(AnimateType.Idle);

        }
    }
}