using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Move/FindPlayer")]
    /// <summary>
    /// 状态机的查找主角案例，一个简单的搜索主角的方案
    /// </summary>
    public class FindPlayer : StateMachineBase
    {
        private Motor.EnemyMotor enemyMotor;
        private Info.EnemyInfo enemyInfo;
        /// <summary> /// 检查球体的半径大小    /// </summary>
        public float CheckRadius = 2f;

        public StateMachineBase attackState;
        public LayerMask shelter = -1;

        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            if (Control.PlayerControl.Instance == null
                || enemyMotor == null) return null;
            Transform player = Control.PlayerControl.Instance.transform;
            Transform transform = manage.transform;

            float sqrDis = (player.position - transform.position).sqrMagnitude;
            //跑出去了就恢复到开始状态
            if(sqrDis > enemyInfo.seeDistance * enemyInfo.seeDistance)
            {
                return manage.beginState;
            }
            if(sqrDis < enemyInfo.attackDistance)
            {
                return attackState;
            }
            return null;

        }

        public override void EnterState(StateMachineManage manage)
        {
            enemyMotor = manage.GetComponent<Motor.EnemyMotor>();
            enemyInfo = manage.GetComponent<Info.EnemyInfo>();
        }

        public override void ExitState(StateMachineManage manage)
        {
            manage.AnimateManage?.PlayAnimate(AnimateType.Idle);
        }

        /// <summary>   /// 走向主角，判断是否遮挡    /// </summary>
        public override void OnFixedUpdate(StateMachineManage manage)
        {
            //不判断Info，因为在前一个状态Info就已经判断过了
            if (Control.PlayerControl.Instance == null 
                || enemyMotor == null) return;
            Transform player = Control.PlayerControl.Instance.transform;
            Transform transform = manage.transform;

            Vector3 playerDir = player.position - transform.position;

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(playerDir.normalized), Time.deltaTime * 15);

            Vector3 moveDir = transform.forward;

            //播放移动动画
            manage.AnimateManage?.PlayAnimate(AnimateType.Move);

            RaycastHit hit;
            if(Physics.SphereCast(transform.position, CheckRadius, playerDir.normalized, out hit, 
                playerDir.magnitude, shelter))
            {
                if (hit.collider.tag != "Player")
                {
                    moveDir += hit.normal;
                    //有碰撞就调整移动的方向
                    enemyMotor.Move(moveDir.z, moveDir.x);
                    return;
                }
            }
            enemyMotor.Move(moveDir.z, moveDir.x);
            return;
        }
    }
}