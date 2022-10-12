using UnityEngine;


namespace ScriptAnimate
{
    //旋转敌人并播放特效，需要注意的是这个类并不是动画控制类，只是一个脚本而已
    public class EnemyRotate : MonoBehaviour
    {
        DefferedRender.ParticleDrawData drawData = new DefferedRender.ParticleDrawData()
        {
            beginSpeed = Vector3.right * 10,
            speedMode = DefferedRender.SpeedMode.VerticalVelocityInside,
            useGravity = true,
            followSpeed = true,
            radius = 1,
            radian = 6.28f,
            lifeTime = 3,
            showTime = 3f,
            frequency = 1,
            octave = 5,
            intensity = 10,
            sizeRange = new Vector2(0.1f, 0.3f),
            colorIndex = DefferedRender.ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = DefferedRender.SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 30
        };
        float nowRadio; bool isUp;

        public float changeSpeed = 1;

        public Vector2 offsetPos = new Vector2(7, 12);

        private void Start()
        {
            isUp = true; nowRadio = 0;   
        }

        Vector3 end = new Vector3(0, 135, 0);
        Vector3 beg = new Vector3(0, 45, 0);

        private void FixedUpdate()
        {
            Control.PlayerControl player = Control.PlayerControl.Instance;
            if ((player.transform.position - transform.position).sqrMagnitude
                > 90000)
                return;
            if (player == null) return;
            nowRadio += Time.deltaTime * (1.0f / changeSpeed);
            if(nowRadio >= 1.0f)
            {
                drawData.beginPos = transform.position;
                drawData.beginPos.y += offsetPos.y;
                if (isUp)
                    drawData.beginPos.z += offsetPos.x;
                else
                    drawData.beginPos.z -= offsetPos.x;
                DefferedRender.ParticleNoiseFactory.Instance.DrawShape(drawData);
                nowRadio = 0;
                isUp = !isUp;
                return;
            }
            if (isUp)
                transform.rotation = Quaternion.Euler(Vector3.Lerp(beg, end, nowRadio));
            else
                transform.rotation = Quaternion.Euler(Vector3.Lerp(end, beg, nowRadio));
        }
    }
}