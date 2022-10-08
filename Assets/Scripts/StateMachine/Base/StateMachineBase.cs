using UnityEngine;

namespace StateMachine
{
    /// <summary> /// 状态机的基类，用来定义状态机行为  /// </summary>
    public abstract class StateMachineBase : ScriptableObject
    {
        /// <summary>     /// 逐固定帧执行的行为      /// </summary>
        public abstract void OnFixedUpdate(StateMachineManage manage);

        /// <summary>    /// 检查该帧行为，判断是否需要切换状态    /// </summary>
        public abstract StateMachineBase CheckState(StateMachineManage manage);
        /// <summary>   /// 退出状态的行为     /// </summary>
        public abstract void ExitState(StateMachineManage manage);
        /// <summary>   /// 进入状态的行为     /// </summary>
        public abstract void EnterState(StateMachineManage manage);
    }
}