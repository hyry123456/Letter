using UnityEngine;

namespace StateMachine
{
    /// <summary> /// ״̬���Ļ��࣬��������״̬����Ϊ  /// </summary>
    public abstract class StateMachineBase : ScriptableObject
    {
        /// <summary>     /// ��̶�ִ֡�е���Ϊ      /// </summary>
        public abstract void OnFixedUpdate(StateMachineManage manage);

        /// <summary>    /// ����֡��Ϊ���ж��Ƿ���Ҫ�л�״̬    /// </summary>
        public abstract StateMachineBase CheckState(StateMachineManage manage);
        /// <summary>   /// �˳�״̬����Ϊ     /// </summary>
        public abstract void ExitState(StateMachineManage manage);
        /// <summary>   /// ����״̬����Ϊ     /// </summary>
        public abstract void EnterState(StateMachineManage manage);
    }
}