namespace Interaction
{
    /// <summary>
    /// 一个内容为空的交互，所有的行为由外界创建时定义的委托决定
    /// </summary>
    public class InteracteDelegate : InteractionBase
    {
        /// <summary>   /// 无参无返回值委托，传入需要执行的方法  /// </summary>
        public Common.INonReturnAndNonParam nonReturnAndNonParam;

        public override void InteractionBehavior()
        {
            if (nonReturnAndNonParam != null)
                nonReturnAndNonParam();
        }

        protected override void OnEnable()
        {

        }
    }
}