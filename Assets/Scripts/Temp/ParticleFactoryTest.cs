using UnityEngine;
using UI;
using UnityEngine.EventSystems;
using DefferedRender;

public class ParticleFactoryTest : UIUseBase
{
    public int index = 0;
    ParticleDrawData drawData;

    [SerializeField, GradientUsage(true)]
    public Gradient gradient;

    protected override void Awake()
    {
        base.Awake();
        control.init += ShowSelf;
        widgrt.pointerClick += Factory;
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up * 3,
            useGravity = false,
            followSpeed = true,
            radian = 3.14f,
            radius = 10f,
            cubeOffset = Vector3.one * 100,
            liftTime = 10,
            showTime = 8,
            frequency = 10f,
            octave = 8,
            intensity = 100,
            sizeRange = Vector2.up,
            colorIndex = (int)ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };

        GradientColorKey[] gradientColorKeys = gradient.colorKeys;
        //for (int j = 0; j < gradientColorKeys.Length && j < 6; j++)
        //{
        //    Debug.Log("Color " + j + " = " + gradientColorKeys[j].color);
        //    Debug.Log("Time " + j + " = " + gradientColorKeys[j].time);
        //}
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        //for (int j = 0; j < alphaKeys.Length && j < 6; j++)
        //{
        //    Debug.Log("Alpha " + j + " = " + alphaKeys[j].alpha);
        //    Debug.Log("Alpha Time " + j + " = " + alphaKeys[j].time);
        //}

    }

    private void Factory(PointerEventData eventData)
    {
        Vector3 noiseData = new Vector3(2, 1, 30);
        if (index == 0)
        {
            ParticleNoiseFactory.Instance.DrawShape(drawData);
        }
        else if(index == 1)
        {
            ParticleNoiseFactory.Instance.DrawCube(drawData);
        }
        else
        {
            ParticleNoiseFactory.Instance.DrawPos(drawData);
        }

    }


    private void Update()
    {
        //DefferedRender.ParticleNoiseFactory.Instance.DrawShape(
        //    Vector3.zero, Vector3.up, false, 0, 10,
        //     3.14f, 10, 7, noiseData, Vector2.up, 0, 0, 1, true);
        //ParticleNoiseFactory.Instance.DrawCube(drawData);
        //ParticleNoiseFactory.Instance.DrawShape(drawData);
    }

}
