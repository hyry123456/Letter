namespace Interaction
{
    /// <summary>
    /// һ������Ϊ�յĽ��������е���Ϊ����紴��ʱ�����ί�о���
    /// </summary>
    public class InteracteDelegate : InteractionBase
    {
        /// <summary>   /// �޲��޷���ֵί�У�������Ҫִ�еķ���  /// </summary>
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