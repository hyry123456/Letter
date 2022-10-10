using UnityEngine;

namespace StateMachine
{
    /// <summary>  
    /// 状态机管理类，用来控制敌人的状态，拥有了状态机后敌人不再需要控制器，
    /// 因为本身状态机就是一个控制器，一个更加完善的控制器
    /// </summary>
    public class StateMachineManage : MonoBehaviour
    {
        /// <summary>    
        /// 初始状态，用来作为状态机的初始行为，以及方便直接回退
        /// </summary>
        public StateMachineBase beginState;
        [SerializeField]
        /// <summary>    /// 当前状态，用来作为实时行为    /// </summary>
        StateMachineBase nowState;
        private AnimateManage animate;
        /// <summary>   /// 角色的动画管理类    /// </summary>
        public AnimateManage AnimateManage => animate;

        private Motor.EnemyMotor motor;
        /// <summary>     /// 敌人的运动引擎     /// </summary>
        public Motor.EnemyMotor EnemyMotor => motor;

        private Skill.SkillManage skillManage;
        public Skill.SkillManage SkillManage => skillManage;


        private void Start()
        {
            if(beginState != null)
            {
                beginState.EnterState(this);
                nowState = beginState;
            }
            animate = GetComponent<AnimateManage>();
            motor = GetComponent<Motor.EnemyMotor>();
            skillManage = GetComponent<Skill.SkillManage>();
        }

        /// <summary>    /// 逐固定帧运行当前状态机的行为     /// </summary>
        private void FixedUpdate()
        {
            if (nowState == null)
                return;
            nowState.OnFixedUpdate(this);

            StateMachineBase tempState = nowState.CheckState(this);
            if(tempState != null)
            {
                nowState.ExitState(this);
                tempState.EnterState(this);
                nowState = tempState;
            }
        }

    }
}