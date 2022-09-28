using Control;
using DefferedRender;
using UnityEngine;

namespace Info
{
    public class EnemyInfo : CharacterInfo
    {
        /// <summary>   /// 敌人的可视距离   /// </summary>
        public float seeDistance = 50;
        EnemyControl enemyControl;
        /// <summary>   /// 敌人死亡后播放爆炸特效   /// </summary>
        ParticleDrawData drawData;

        private void Start()
        {
            Vector2 sizeRange = new Vector2(0.4f, 1.5f);
            drawData = new ParticleDrawData
            {
                //beginPos = transform.position,
                beginSpeed = Vector3.up * 10,
                speedMode = SpeedMode.VerticalVelocityOutside,
                useGravity = false,
                followSpeed = false,
                radian = 3.14f,
                radius = 1f,
                //cubeOffset = new Vector3(0.1f, 0.1f, 0.1f),
                lifeTime = 4,
                showTime = 4f,
                frequency = 1f,
                octave = 4,
                intensity = 20,
                sizeRange = sizeRange,
                colorIndex = (int)ColorIndexMode.AlphaToAlpha,
                sizeIndex = (int)SizeCurveMode.SmallToBig_Subken,
                textureIndex = 1,
                groupCount = 10,
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            enemyControl = GetComponent<EnemyControl>();
        }

        public override void modifyHp(int dealtaHp)
        {
            base.modifyHp(dealtaHp);
        }

        protected override void DealWithDeath()
        {
            drawData.beginPos = transform.position;
            ParticleNoiseFactory.Instance.DrawPos(drawData);
            enemyControl.CloseObject();
        }
    }
}