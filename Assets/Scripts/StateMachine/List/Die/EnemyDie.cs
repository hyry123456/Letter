using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine {

    [CreateAssetMenu(menuName = "StateMachine/Die/EnemyDie")]
    /// <summary>  ///敌人死亡  /// </summary>
    public class EnemyDie : StateMachineBase
    {


        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            //应该需要关闭一些东西
            manage.AnimateManage.PlayAnimate(AnimateType.Die);//播放动画
        }

        public override void ExitState(StateMachineManage manage)
        {
            
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            
        }

    }


}


