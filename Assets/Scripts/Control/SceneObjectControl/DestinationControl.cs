using System.Collections;
using System.Collections.Generic;
using DefferedRender;
using UnityEngine;

public class DestinationControl : MonoBehaviour
{
    /// <summary>///信物收集总数，未使用 /// </summary>
    public int LetterCount = 0;//Not Used
    /// <summary>///通关条件，应至少收集的数量 /// </summary>
    public int Goal = 0;
    [SerializeField]
    string targetSceneName;

    private ParticleDrawData drawData;

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        if (target.gameObject.name == "Player")
        {
            if (target.GetComponent<Info.CharacterInfo>().getScore() > Goal)//根据收集数量判断是否能通关
            {
                SolveEnd();
            }
        }
    }
    private void Awake()
    {
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up,
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = false,
            followSpeed = true,
            radian = 3.14f,
            radius = 100f,
            cubeOffset = new Vector3(0.1f, 50f, 0.1f),
            lifeTime = 2,
            showTime = 2,
            frequency = 1f,
            octave = 1,
            intensity = 0.1f,
            sizeRange = Vector2.up * 0.1f,
            colorIndex = (int)ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 5,
        };
    }
    /// <summary>///生成粒子 /// </summary>
    void summonParticle() {
        ParticleNoiseFactory.Instance.DrawCube(drawData);
    }
    /// <summary>///处理游戏结束事件 /// </summary>
    void SolveEnd()
    {
        if (targetSceneName != null && targetSceneName.Length != 0)
            Control.SceneChangeControl.Instance.ChangeScene(targetSceneName);
    }
    private void Update()
    {
        summonParticle();//暂时先持续生成，后续考虑根据达成条件进行生成
    }
}
