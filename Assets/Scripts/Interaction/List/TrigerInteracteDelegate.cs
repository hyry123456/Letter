using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// 触发式交互事件，建议放在一个空物体上，因为触发后会立刻销毁，
    /// 且只会被主角给触发
    /// </summary>
    public class TrigerInteracteDelegate : InteractionBase
    {
        /// <summary>        /// 触发时执行的行为        /// </summary>
        public Common.INonReturnAndNonParam trigerDelegate;
        public override void InteractionBehavior()
        {
        }

        /// <summary>   
        /// 当触发发生时执行的方法，用来执行交互，当触发结束后立刻结束该交互
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            //只触发主角
            if(other.tag == "Player")
            {
                if(trigerDelegate != null)
                {
                    trigerDelegate();
                    trigerDelegate = null;
                }
                //删除自身
                Destroy(gameObject);
            }
        }

        protected override void OnEnable()
        {
        }
    }
}