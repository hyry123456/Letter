
namespace Interaction
{
    /// <summary>
    /// 交互类型，注意，交互类型实际上是
    /// </summary>
    public enum InteractionType
    {
        Object = 1,
        PasserBy = 2,
        Task = 8,
        /// <summary>       /// 运动交互        /// </summary>
        Move = 16,
        Other = 32,     //其他交互，不在UI中显示，但是会交互
    }
}