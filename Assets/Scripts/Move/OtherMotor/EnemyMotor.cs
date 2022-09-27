using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motor
{
    public class EnemyMotor : MonoBehaviour
    {

        /// <summary>    /// 当前速度, 期望速度, 连接物体的速度    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;
        /// <summary>    /// 加速度    /// </summary>
        public float groundAcceleration = 10f, airAcceleration = 5;

        Rigidbody body;
        GameObject connectObj, preConnectObj;

        /// <summary>    /// 是否在地面上    /// </summary>
        private bool onGround = false;

        /// <summary>        /// 最大地面的倾斜夹角，以及楼梯倾斜夹角        /// </summary>
        [Range(0, 90)]
        public float maxGroundAngle = 25f, maxStairAngle = 25;
        private float minGroundDot = 0, minStairsDot = 0;

        /// <summary>
        /// 接触面的法线，这个法线是平均法线，用来确定移动面的方向
        /// </summary>
        Vector3 contactNormal;

        /// <summary>    /// 判断时候可以贴地的速度，如果速度大于该值，不允许贴地    /// </summary>
        [SerializeField, Range(0, 100f)]
        float maxSnapSpeed = 100f;
        /// <summary>    /// 贴地的检测距离    /// </summary>
        [SerializeField, Range(0, 10f)]
        float probeDistance = 3f;
        /// <summary>     /// 贴地检测计数    /// </summary>
        int stepSinceLastGround;

        /// <summary>    /// 贴地检查的层，以及楼梯检查层    /// </summary>
        [SerializeField]
        LayerMask probeMask, stairsMask = -1;

        Info.CharacterInfo characterInfo;
        Vector3 connectionWorldPostion;


        /// <summary>
        /// 攀爬检测的偏移数值，x是前方缩放，y是向上偏移,z是添加的力的大小
        /// </summary>
        [SerializeField]
        Vector3 climbData = Vector3.one;

        void Awake()
        {
            velocity = Vector3.zero;
            body = GetComponent<Rigidbody>();
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDot = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
            characterInfo = GetComponent<Info.CharacterInfo>();
            if (characterInfo == null) Debug.LogError("角色信息为空");
        }

        public void Move(float x, float y)
        {
            //实际上就是加速度是定值，角色输入值是目标速度，逐帧将速度变化为目标速度
            Vector2 playInput = new Vector2(x, y);
            playInput = Vector2.ClampMagnitude(playInput, 1);
            desiredVelocity = Vector3.forward * playInput.x + Vector3.right * playInput.y;

            desiredVelocity = desiredVelocity * characterInfo.runSpeed;
        }

        private void FixedUpdate()
        {
            //更新数据，用来对这一个物理帧的数据进行更新之类的
            UpdateState();

            //确定在空中还是在地面
            AdjustVelocity();
            Rotate();

            velocity = Vector3.ClampMagnitude(velocity, 40);

            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            velocity = body.velocity;
            //当不在地面时执行贴近地面方法
            if (onGround/*在地上*/ || SnapToGround()/*可以贴近地面，也就是刚刚未经过跳跃，但是飞了出去*/)
            {

                contactNormal.Normalize();
                LoadTargetY(desiredVelocity);
            }
            else
                contactNormal = Vector3.up;

            if (connectObj && connectObj.tag == "CheckMove")
            {
                UpdateConnectionState();
            }
        }

        void UpdateConnectionState()
        {
            //只有物体相同，才有必要计算
            if (connectObj == preConnectObj)
            {
                Vector3 connectionMovment =
                    connectObj.transform.position - connectionWorldPostion;
                connectionVelocity = connectionMovment / Time.deltaTime;
            }
            connectionWorldPostion = connectObj.transform.position;
        }

        private void OnCollisionExit(Collision collision)
        {
            EvaluateCollision(collision);
        }


        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            float minDot = GetMinDot(collision.gameObject.layer);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(Vector3.up, normal);
                if (upDot >= minDot)
                {
                    onGround = true;
                    //保证如果有多个接触面时能够正确的获取法线
                    contactNormal += normal;
                    connectObj = collision.gameObject;
                }
                //陡峭面移动控制，但是避免彻底的垂直面
                else if (upDot > -0.01f)
                {
                    connectObj = collision.gameObject;
                }

            }
        }


        /// <summary>
        /// 调整移动方向，用来保证移动的方向是沿着平面的
        /// </summary>
        void AdjustVelocity()
        {
            //因为速度用的也是世界坐标，因此移动时投影也依靠的是世界坐标，其中right控制x轴，foward控制Y轴
            //将1，0，0投影到接触平面上，
            Vector3 xAixs = ProjectDirectionOnPlane(Vector3.right, contactNormal);
            //将0，0，1投影到接触平面上
            Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;
            //确定实际上在这个平面上的X移动值
            float currentX = Vector3.Dot(relativeVelocity, xAixs);
            //确定实际上在这个平面上的Z移动值
            float currentZ = Vector3.Dot(relativeVelocity, zAxis);

            float acceleration = onGround ? groundAcceleration : airAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            //确定根据期望得到的移动值
            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

            //移动要根据这个平面的方向来移动，因此根据实际值与期望值的差确定要增加的速度大小，
            //然后乘以投影计算出来的X值以及Z值确定最后的移动值
            velocity += xAixs * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        /// <summary>
        /// 清除数据，把一些数据归为初始化
        /// </summary>
        void ClearState()
        {
            onGround = false;
            contactNormal = connectionVelocity = Vector3.zero;
            preConnectObj = connectObj;
            connectObj = null;
        }

        /// <summary>
        /// 用于贴近地面用的方法，减少移动时会飞出去的效果
        /// </summary>
        /// <returns>用来配合一些地面检测使用，因此有返回值</returns>
        bool SnapToGround()
        {
            //贴地行为只进行一次
            if (stepSinceLastGround > 1)
            {
                return false;
            }
            float speed = velocity.magnitude;
            //大于最大速度，不进行贴地
            if (speed > maxSnapSpeed)
                return false;

            RaycastHit hit;
            if (!Physics.Raycast(body.position, -Vector3.up, out hit, probeDistance, probeMask))
                return false;

            float upDot = Vector3.Dot(Vector3.up, hit.normal);
            //如果射中的面不能作为可以站立的面，就不进行贴近
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
                return false;

            contactNormal = hit.normal;

            //确定速度在法线上的大小
            float dot = Vector3.Dot(velocity, hit.normal);
            //保证只有速度朝上时才会往下压，不会减少下落速度
            if (dot > 0)
            {
                //根据速度的大小往平面上压
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectObj = hit.collider.gameObject;
            return true;
        }

        float GetMinDot(int layer)
        {
            //判断是楼梯还是正常地面
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDot : minStairsDot;
        }


        /// <summary>    /// 确定该方向投影到该平面上的方向值，进行过标准化的    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        float targetRotateY;
        /// <summary>       /// 旋转模型        /// </summary>
        void Rotate()
        {
            Vector3 angle = transform.eulerAngles;
            //移动角度
            angle.y = Mathf.MoveTowardsAngle(angle.y, targetRotateY, characterInfo.rotateSpeed);

            transform.eulerAngles = angle;
        }

        /// <summary>
        /// 加载旋转目标Y值，只有在地上时或者蹬强时才会旋转
        /// </summary>
        /// <param name="desire">期望移动的世界方向</param>
        void LoadTargetY(Vector3 desire)
        {
            Vector2 vector2 = new Vector2(desire.x, desire.z);
            //太小就不调整，避免旋转到错误方向
            if (Mathf.Abs(vector2.y) < 0.0001) return;
            targetRotateY = GetAngle(vector2.normalized);
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



    }
}