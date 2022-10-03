using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    DefferedRender.ParticleDrawData particleDrawData;
    public GameObject begin, end;
    void Start()
    {
        particleDrawData = new DefferedRender.ParticleDrawData
        {
            speedMode = DefferedRender.SpeedMode.JustBeginSpeed,
            useGravity = false,
            followSpeed = false,
            cubeOffset = Vector3.one * 10,
            lifeTime = 1f,
            showTime = 1f,
            frequency = 1,
            octave = 4,
            intensity = 30,
            sizeRange = new Vector2(0.2f, 0.4f),
            colorIndex = (int)DefferedRender.ColorIndexMode.HighlightToAlpha,
            sizeIndex = (int)DefferedRender.SizeCurveMode.SmallToBig_Epirelief,
            textureIndex = 0,
            groupCount = 5,
        };

    }

    private void Update()
    {
        particleDrawData.beginPos = begin.transform.position;
        particleDrawData.endPos = end.transform.position;
        DefferedRender.ParticleNoiseFactory.Instance.DrawCube(particleDrawData);
    }

}
