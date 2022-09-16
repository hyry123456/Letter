
using UnityEngine;


namespace Motor
{
    public class OribitCamera : MonoBehaviour
    {
        /// <summary>        /// 锁定的位置        /// </summary>
        [SerializeField]
        Transform focus = default;
        /// <summary>        /// 用户的鼠标输入        /// </summary>
        Vector2 playerInput;

        /// <summary>        /// 摄像机距离锁定点的距离        /// </summary>
        [SerializeField, Range(1f, 20f)]
        float distance = 5f;
        /// <summary>        /// 锁定的偏移距离，当目标点与实际锁定点大于该距离时，才会接近锁定点        /// </summary>
        [SerializeField, Min(0f)]
        float focusRadius = 1f;
        /// <summary>    /// 要锁定的目标点，摄像机会移动到该点上,
        /// 但是这个不是实际目标所在位置，可能会有一定的偏移    /// </summary>
        Vector3 focusPoint;
        /// <summary>   /// 上一次锁定的目标点，用来判断移动方向，用来确定视角接近到哪里    /// </summary>
        Vector3 previousFocusPoint;
        /// <summary>    /// 每秒的接近比例，设为0.5就是每秒缩小一半    /// </summary>
        [SerializeField, Range(0f, 1f)]
        float focusCentering = 0.5f;

        /// <summary>    /// 存储当前旋转角度    /// </summary>
        Vector2 oribitAngles = new Vector2(0f, 0f);

        /// <summary>    /// 垂直旋转角度限制    /// </summary>
        [SerializeField, Range(-89f, 89f)]
        float minVerticalAngle = -30f, maxVerticalAngle = 60;

        /// <summary>    /// 当玩家经过这里设置的时间未输入后，进行摄像机对齐    /// </summary>
        [SerializeField]
        float alignDelay = 5;
        /// <summary>    /// 控制摄像机移动时在小于该角度时会进行缓慢移动，而不是每时每刻都用摄像机移动的最大速度移动    /// </summary>
        [SerializeField, Range(0, 90)]
        float alignSmoothRange = 45f;

        /// <summary>0    /// 上一次输入旋转的时间    /// </summary>
        float lastManualRotationTime = 0;

        [SerializeField, Range(1f, 360f)]
        float rotationSpeed = 90f;

        /// <summary>    /// 设置不能被摄像机穿过的层    /// </summary>
        [SerializeField]
        LayerMask layerMask = -1;

        /// <summary>    /// 摄像机组件    /// </summary>
        Camera regularCamera;

        /// <summary>    /// 确定摄像机的视锥体的大小    /// </summary>
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                //确定投影矩形的Y轴一半大小，不直接使用近平面的一半是因为fieldOfView会进行一定的缩放，我们需要将缩放变回去
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                //根据比例计算出X轴大小
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                //Z根据投影长度来设置，没有影响
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        void Start()
        {
            //开头先看向目标位置
            focusPoint = focus.position;
            regularCamera = GetComponent<Camera>();
        }
        Quaternion lookRotation;

        /// <summary>
        /// 摄像机是逐帧刷新的，因此摄像机行为也要逐帧刷新
        /// </summary>
        void Update()
        {
            UpdateFocusPoint();
            if (ManualRotation() || AutomaticRotation())
            {
                //确定旋转角度
                ConstrainAngle();
                lookRotation = Quaternion.Euler(oribitAngles);
            }
            else
                lookRotation = transform.localRotation;

            //将世界空间的前方的X以及Z值投影到目前的选择角度上
            Vector3 lookDirection = lookRotation * Vector3.forward;

            //锁定的位置不是实际几何体位置，但是因为可能我们的焦点存在于几何体内部(因为有偏移)，
            //因此这个锁定的方向不一定是正确的，需要进行一定的偏移
            Vector3 lookPosition = focusPoint - lookDirection * distance;

            Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
            //确定经过偏移后的摄像机位置
            Vector3 rectPosition = lookPosition + rectOffset;
            Vector3 castFrom = focus.position;
            //确定偏移的摄像机位置距离实际锁定的位置的方向
            Vector3 castLine = rectPosition - castFrom;
            //确定距离
            float castDistance = castLine.magnitude;
            //标准化方向
            Vector3 castDirection = castLine / castDistance;

            //方形投影，毕竟摄像机是一个方形，投影方向是从锁定点到目标点，由于摄像机有近平面，
            //因此不用考虑需要沿射中方向前移之类的问题
            if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation,
                castDistance, layerMask))
            {
                //确定实际碰撞位置，为了避免焦点在中心，因此这里投影方向实际上是从球体位置投影到摄像机位置，
                //判断有没有碰撞点，然后计算距离
                rectPosition = castFrom + castDirection * hit.distance;
                //因为是偏移计算，因此需要将偏移值转换回来
                lookPosition = rectPosition - rectOffset;
            }

            //让摄像机看向这个方向以及设置在这个位置上
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        /// <summary>        /// 更新目标观测点        /// </summary>
        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            //理想应该看向的点
            Vector3 targetPoint = focus.position;
            //只有需要范围聚焦才会移动延迟移动摄像机
            if (focusRadius > 0f)
            {
                //确定实际看向点距离理想看向点的距离
                float distance = Vector3.Distance(targetPoint, focusPoint);
                float t = 1f;
                //用第二个插值控制内部的锁定，也就是说在中间时也会接近锁定点，但是在中间时缩小的比较满
                if (focusCentering > 0f)
                {
                    //确定在内部的渐变速度，控制每秒的接近比例
                    t = 1f - focusCentering * Time.unscaledDeltaTime;
                }
                //只有大于最大偏移距离时才会移动锁定点
                if (distance > focusRadius)
                {
                    //确定渐变大小，取两个插值的最小值
                    t = Mathf.Min(t, focusRadius / distance);
                }
                //需要注意的是，渐变的方式，begin才是目标位置，因此只有差距越大，值越小，越接近目标速度
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            }
            else
                focusPoint = targetPoint;
        }

        /// <summary>        /// 设置玩家输入值，用来旋转摄像机        /// </summary>
        /// <param name="mouseY">鼠标上下移动值</param>
        /// <param name="mouseX">鼠标左右移动值</param>
        public void SetCameraInput(float mouseY, float mouseX)
        {
            playerInput = new Vector2(mouseY, mouseX);
        }

        /// <summary>   /// 管理旋转角度  /// </summary>
        /// <returns>是否需要进行旋转</returns>
        bool ManualRotation()
        {
            //鼠标输入值
            float e = 0.001f;
            //判断是否有输入,注意是鼠标输入
            if (playerInput.x < -e || playerInput.x > e || playerInput.y < -e || playerInput.y > e)
            {
                //根据输入值控制角度
                oribitAngles += rotationSpeed * Time.unscaledDeltaTime * playerInput;
                //更新输入时间
                lastManualRotationTime = Time.unscaledTime;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 自动旋转方法，用来实现大部分游戏的移动时在一段时间没有输入，
        /// 同时在这期间进行了移动时会进行摄像机锁定方向在模型的后部的效果
        /// </summary>
        /// <returns>是否要进行旋转</returns>
        bool AutomaticRotation()
        {
            //判断是否到达转视角的时间
            if (Time.unscaledTime - lastManualRotationTime < alignDelay)
            {
                return false;
            }
            //判断移动方向
            Vector2 movement = new Vector2(focusPoint.x - previousFocusPoint.x,
                focusPoint.z - previousFocusPoint.z);
            //确定移动距离
            float movementDeltaSqr = movement.sqrMagnitude;
            //如果移动的太小，就不旋转
            if (movementDeltaSqr < 0.0000001f)
                return false;
            //确定要变化到的角度
            float headingAngle = GetAngle(movement.normalized);
            //得到当前旋转角度与目标旋转角度的最小差距度数，且返回绝对值
            float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(oribitAngles.y, headingAngle));
            //控制移动速度，最大不能大于帧差距，但是可能移动幅度太小，因此改变速度也小
            float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
            //控制旋转速度，在与目标旋转角度比较接近时，减小旋转速度
            if (deltaAbs < alignSmoothRange)
            {
                rotationChange *= deltaAbs / alignSmoothRange;
            }
            //考虑方向相反，但是夹角也小的情况
            else if (180f - deltaAbs < alignSmoothRange)
            {
                rotationChange *= (180f - deltaAbs) / alignSmoothRange;
            }
            //渐变模式
            oribitAngles.y = Mathf.MoveTowardsAngle(oribitAngles.y, headingAngle, rotationChange);

            return true;
        }

        /// <summary>
        /// 根据移动的差距值判断旋转角度，注意传入值要标准化，
        /// 设置为静态因为这个函数不需要用到对象数据，因此只用开辟一个函数体就够了
        /// </summary>
        static float GetAngle(Vector2 direction)
        {
            //通过反余弦函数计算出旋转到这个移动方向所需要的y值角度
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            //判断是哪边，也就是顺时针还是逆时针
            return direction.x < 0f ? 360f - angle : angle;
        }

        /// <summary>    /// 限制角度大小    /// </summary>
        void ConstrainAngle()
        {
            oribitAngles.x = Mathf.Clamp(oribitAngles.x, minVerticalAngle, maxVerticalAngle);
            if (oribitAngles.y < 0f)
            {
                oribitAngles.y += 360f;
            }
            else if (oribitAngles.y >= 360f)
            {
                oribitAngles.y -= 360f;
            }
        }

    }
}