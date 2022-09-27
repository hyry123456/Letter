using DefferedRender;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        GameObject target;
        if (rb != null)//判断目标是否有刚体
        {
            target = rb.gameObject;
            if(target.name == "Player")//通过名字判断
            {
                target.GetComponent<Info.CharacterInfo>().modifyHp(-5);//后续操作
            }
        }
        Destroy(gameObject);//销毁子弹
    }

    ParticleDrawData drawData;
    private void Awake()
    {
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
            intensity = 10,
            sizeRange = Vector2.up * 0.1f,
            colorIndex = (int)ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }
    private void Update()
    {
        drawData.beginPos = transform.position;
        //ParticleNoiseFactory.Instance.DrawCube(drawData);
        ParticleNoiseFactory.Instance.DrawPos(drawData);

    }
}
