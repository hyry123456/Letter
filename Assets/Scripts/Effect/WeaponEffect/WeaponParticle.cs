using UnityEngine;

public class WeaponParticle : MonoBehaviour
{
    DefferedRender.ParticleDrawData particleDrawData;
    public Vector3 cubeOffset;
    public Transform begin;
    public Transform end;

    void Start()
    {
        particleDrawData = new DefferedRender.ParticleDrawData
        {
            speedMode = DefferedRender.SpeedMode.JustBeginSpeed,
            useGravity = true,
            followSpeed = false,
            cubeOffset = cubeOffset,
            lifeTime = 1f,
            showTime = 1f,
            frequency = 1,
            octave = 4,
            intensity = 1,
            sizeRange = Vector2.up * 0.3f,
            colorIndex = DefferedRender.ColorIndexMode.HighlightToAlpha,
            sizeIndex = DefferedRender.SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }

    private void FixedUpdate()
    {
        particleDrawData.beginPos = begin.position;
        particleDrawData.endPos = end.position;
        DefferedRender.ParticleNoiseFactory.Instance.DrawCube(particleDrawData);
    }

}
