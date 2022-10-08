
namespace Common
{
    /// <summary> /// 池化NPC的标准方式，用来保证该NPC被删除后不会剩下交互 /// </summary>
    public class NPC_Pooling : ObjectPoolBase
    {
        protected override void OnEnable()
        {
        }

        /// <summary> /// 清除多余的交互 /// </summary>
        public override void CloseObject()
        {
            base.CloseObject();
            Interaction.InteractionBase[]
                interactions = GetComponents<Interaction.InteractionBase>();
            for(int i=0; i<interactions.Length; i++)
            {
                Destroy(interactions[i]);
            }
        }
    }
}