using UnityEngine;

namespace StateMachine
{
    /// <summary>  
    /// ״̬�������࣬�������Ƶ��˵�״̬��ӵ����״̬������˲�����Ҫ��������
    /// ��Ϊ����״̬������һ����������һ���������ƵĿ�����
    /// </summary>
    public class StateMachineManage : MonoBehaviour
    {
        /// <summary>    
        /// ��ʼ״̬��������Ϊ״̬���ĳ�ʼ��Ϊ���Լ�����ֱ�ӻ���
        /// </summary>
        public StateMachineBase beginState;
        [SerializeField]
        /// <summary>    /// ��ǰ״̬��������Ϊʵʱ��Ϊ    /// </summary>
        StateMachineBase nowState;
        private AnimateManage animate;
        /// <summary>   /// ��ɫ�Ķ���������    /// </summary>
        public AnimateManage AnimateManage => animate;

        private Motor.EnemyMotor motor;
        /// <summary>     /// ���˵��˶�����     /// </summary>
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
        }

        /// <summary>    /// ��̶�֡���е�ǰ״̬������Ϊ     /// </summary>
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