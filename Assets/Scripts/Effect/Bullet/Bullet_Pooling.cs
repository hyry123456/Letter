using Common;
using DefferedRender;
using UnityEngine;

/// <summary>
/// 池化的子弹，由对象池进行创建以及删除
/// </summary>
public class Bullet_Pooling : ObjectPoolBase
{
    float time;
    /// <summary> /// 子弹最多存活事件，超过就会自动死亡 /// </summary>
    [SerializeField]
    float maxLifeTime = 10;
    /// <summary>  /// 子弹的移动速度 /// </summary>
    [SerializeField]
    float moveSpeed = 10;
    ParticleDrawData drawData;
    /// <summary>    /// 每一帧初始化一下时间，只有时间需要次次初始化    /// </summary>
    protected override void OnEnable()
    {
        time = 0;
    }

    //绘制数据只用初始化一次就够了，因此放在这里初始化
    public override void InitializeObject(Vector3 positon, Vector3 lookAt)
    {
        base.InitializeObject(positon, lookAt);
        Vector2 sizeRange = new Vector2(0.1f, 0.2f);
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up,
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = true,
            followSpeed = true,
            radian = 3.14f,
            radius = 1f,
            cubeOffset = new Vector3(0.1f, 0.1f, 0.1f),
            lifeTime = 1,
            showTime = 1,
            frequency = 1f,
            octave = 8,
            intensity = 20,
            sizeRange = sizeRange,
            colorIndex = (int)ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }

    public override void InitializeObject(Vector3 positon, Quaternion quaternion)
    {
        base.InitializeObject(positon, quaternion);
        Vector2 sizeRange = new Vector2(0.1f, 0.2f);
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up,
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = true,
            followSpeed = true,
            radian = 3.14f,
            radius = 1f,
            cubeOffset = new Vector3(0.1f, 0.1f, 0.1f),
            lifeTime = 1,
            showTime = 1,
            frequency = 1f,
            octave = 8,
            intensity = 20,
            sizeRange = sizeRange,
            colorIndex = (int)ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }

    /// <summary>  /// 攻击的目标，当打到该目标的敌人时会扣血 /// </summary>
    public string attackTargetTag;

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if(time > maxLifeTime)
        {
            CloseObject();
            return;
        }
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
        drawData.beginPos = transform.position;
        drawData.beginSpeed = transform.forward * moveSpeed * 0.5f;
        ParticleNoiseFactory.Instance.DrawPos(drawData);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == attackTargetTag)
        {

            Info.CharacterInfo character = collision.gameObject.GetComponent<Info.CharacterInfo>();
            character.modifyHp(-5);
        }

        drawData.groupCount = 30;
        drawData.beginPos = collision.contacts[0].point;
        drawData.speedMode = SpeedMode.VerticalVelocityOutside;
        drawData.beginSpeed = collision.contacts[0].normal * moveSpeed;
        drawData.lifeTime = 5; drawData.showTime = 5f;

        ParticleNoiseFactory.Instance.DrawPos(drawData);

        drawData.groupCount = 1;
        drawData.speedMode = SpeedMode.JustBeginSpeed;

        CloseObject();

        return;
    }
}
