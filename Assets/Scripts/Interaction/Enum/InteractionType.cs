
namespace Interaction
{
    /// <summary>
    /// �������ͣ�ע�⣬��������ʵ������
    /// </summary>
    public enum InteractionType
    {
        Object = 1,
        PasserBy = 2,
        Enemy = 4,
        Task = 8,
        /// <summary>       /// �˶�����        /// </summary>
        Move = 16,
        Other = 32,     //��������������UI����ʾ�����ǻύ��
    }
}