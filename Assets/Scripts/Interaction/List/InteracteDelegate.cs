namespace Interaction
{
    /// <summary>
    /// 一个内容为空的交互，所有的行为由外界创建时定义的委托决定
    /// </summary>
    public class InteracteDelegate : InteractionBase
    {
        /// <summary>   /// 无参无返回值委托，传入需要执行的方法  /// </summary>
        public Common.INonReturnAndNonParam interDelegate;

        public override void InteractionBehavior()
        {
            if (interDelegate != null)
                interDelegate();
        }

        protected override void OnEnable()
        {

        }
    }
}