using UnityEngine;

namespace Skill
{
    /// <summary> /// 光柱，Boss的普通攻击之一，是远程攻击 /// </summary>
    public class LightCross : SkillBase
    {
        public LightCross()
        {
            expendSP = 0;
            nowCoolTime = 0;
            coolTime = 5f;
            releaseTime = sustainTime + waitTime;
            skillName = "Skill Name";
            skillType = SkillType.LongDisAttack;

            drawData = new DefferedRender.ParticleDrawData
            {
                beginSpeed = Vector3.up,
                speedMode = DefferedRender.SpeedMode.JustBeginSpeed,
                useGravity = false,
                followSpeed = false,
                radius = 3,
                radian = 6.28f,
                lifeTime = 1f,
                showTime = 1f,
                frequency = 1,
                octave = 4,
                intensity = 1,
                sizeRange = new Vector2(0.2f, 0.4f),
                colorIndex = DefferedRender.ColorIndexMode.HighlightToAlpha,
                sizeIndex = DefferedRender.SizeCurveMode.Small_Hight_Small,
                textureIndex = 0,
                groupCount = 5,
            };
        }

        Vector3 start, end;
        GameObject origin;
        /// <summary>    /// 攻击距离   /// </summary>
        float attackDistance = 50;
        /// <summary>   /// 移动到终点需要的时间  /// </summary>
        float sustainTime = 1, waitTime = 3;
        /// <summary>  /// 当前累计的移动比例    /// </summary>
        float nowRadio = -1;
        /// <summary>/// 使用技能时创建的游戏对象  /// </summary>
        Sphere_Pooling useObj;
        Vector3 beginScale;
        /// <summary>   /// 存一个数据，判断是否已经打过了  /// </summary>
        bool isAttack = false;

        DefferedRender.ParticleDrawData drawData;


        public override void OnSkillRelease(SkillManage mana)
        {
            if (origin == null)
                origin = Resources.Load<GameObject>("Prefab/Sphere_Pooling");

            Transform playerTrans = Control.PlayerControl.Instance?.transform;
            if (playerTrans == null || nowRadio > 0) return;
            Vector3 offset = (playerTrans.position - mana.transform.position).normalized;
            start = mana.transform.position + offset * 3;
            //移动到攻击距离
            end = mana.transform.position + offset * attackDistance;

            nowRadio = 0; isAttack = false;
            Common.SustainCoroutine.Instance.AddCoroutine(SustainMove);
        }

        private bool SustainMove()
        {
            nowRadio += Time.deltaTime;
            if (nowRadio > waitTime + sustainTime)      //大于两个时间之和
            {
                nowRadio = -1;
                return true;
            }
            else if(nowRadio < waitTime)
            {
                drawData.beginPos = start;
                DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
                return false;
            }

            drawData.beginPos = Vector3.Lerp(start, end, (nowRadio - waitTime) / sustainTime);
            DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
            Debug.DrawLine(start, drawData.beginPos);

            if (!isAttack)      //避免多次伤害
            {
                RaycastHit raycastHit;
                float distance = (drawData.beginPos - start).magnitude;
                if ( Physics.SphereCast(start, 3, drawData.beginPos - start, out raycastHit, distance))
                {
                    isAttack = true;
                    Info.CharacterInfo character = raycastHit.collider.gameObject.GetComponent<Info.CharacterInfo>();
                    if (character == null) return false;
                    character.modifyHp(-10);
                    Rigidbody rigidbody = character.GetComponent<Rigidbody>();
                    rigidbody.velocity += (drawData.beginPos - start).normalized * 30;
                }
            }
            return false;
        }
    }
}