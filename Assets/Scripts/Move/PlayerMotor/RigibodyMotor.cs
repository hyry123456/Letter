
using DefferedRender;
using UnityEngine;

namespace Motor
{

    /// <summary>
    /// 模拟别人的移动类，是一个非常成熟的版本
    /// </summary>
    public class RigibodyMotor : MonoBehaviour
    {

        /// <summary>    /// 当前速度, 期望速度, 连接物体的速度    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;
        /// <summary>    /// 加速度    /// </summary>
        public float groundAcceleration = 10f, airAcceleration = 5;

        Rigidbody body;
        GameObject connectObj, preConnectObj;

        private bool desiredJump = false;

        public float jumpHeight = 2f;
        /// <summary>    /// 最大跳跃次数    /// </summary>
        public int maxAirJumps = 2;
        private int airJumps = 0;

        /// <summary>    /// 是否在地面上    /// </summary>
        private bool onGround = false;

        /// <summary>    /// 字段OnSteep，确定是否在斜面上    /// </summary>
        private bool OnSteep => steepContractCount > 0;

        /// <summary>        /// 最大地面的倾斜夹角，以及楼梯倾斜夹角        /// </summary>
        [Range(0, 90)]
        public float maxGroundAngle = 25f, maxStairAngle = 25;
        private float minGroundDot = 0, minStairsDot = 0;

        /// <summary>
        /// 接触面的法线，这个法线是平均法线，用来确定移动面的方向以及跳跃的方向
        /// </summary>
        Vector3 contactNormal, steepNormal;
        /// <summary>
        /// 陡峭面检测，如果人物被卡在陡峭面中，需要进行计数，判断是否要陡峭面跳跃
        /// </summary>
        int steepContractCount = 0;

        /// <summary>
        /// 用来确定此时离开地面的时间(stepSinceLastGround)，在地面时会变为0，
        /// 不在时会逐物理帧刷新
        /// </summary>
        int stepSinceLastGround = 0;
        /// <summary>    /// 用来确定跳跃的时间，当跳跃时会归零，在物理帧时逐帧增加    /// </summary>
        int stepSinceLastJump = 0;

        /// <summary>    /// 判断时候可以贴地的速度，如果速度大于该值，不允许贴地    /// </summary>
        [SerializeField, Range(0, 100f)]
        float maxSnapSpeed = 100f;
        /// <summary>    /// 贴地的检测距离    /// </summary>
        [SerializeField, Range(0, 10f)]
        float probeDistance = 3f;

        /// <summary>    /// 贴地检查的层，以及楼梯检查层    /// </summary>
        [SerializeField]
        LayerMask probeMask, stairsMask = -1;

        /// <summary>    /// 输入空间，用来根据该空间来控制模型移动    /// </summary>
        [SerializeField]
        Transform playerInputSpace;

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

            drawData = new ParticleDrawData
            {
                beginPos = transform.position,
                //beginSpeed = Vector3.up * 3,
                speedMode = SpeedMode.VerticalVelocityOutside,
                useGravity = false,
                followSpeed = true,
                //radian = 3.14f,
                //radius = 10f,
                cubeOffset = new Vector3(1, 0.2f, 1),
                liftTime = 3,
                showTime = 3,
                frequency = 1f,
                octave = 8,
                intensity = 10,
                sizeRange = Vector2.up * 0.5f,
                colorIndex = (int)ColorIndexMode.HighlightToAlpha,
                sizeIndex = (int)SizeCurveMode.Small_Hight_Small,
                textureIndex = 0,
                groupCount = 100,
            };

        }

        public void Move(float horizontal, float vertical)
        {
            //实际上就是加速度是定值，角色输入值是目标速度，逐帧将速度变化为目标速度
            Vector2 playInput = new Vector2(vertical, horizontal);
            playInput = Vector2.ClampMagnitude(playInput, 1);
            if (playerInputSpace)
            {
                desiredVelocity = playerInputSpace.forward * playInput.x + playerInputSpace.right * playInput.y;
            }
            else
            {
                desiredVelocity = Vector3.forward * playInput.x + Vector3.right * playInput.y;
            }
            desiredVelocity = desiredVelocity * characterInfo.runSpeed;

            //与运算，防止输入被关闭
            desiredJump |= Input.GetButtonDown("Jump");

        }

        private void FixedUpdate()
        {
            //更新数据，用来对这一个物理帧的数据进行更新之类的
            UpdateState();

            //传送
            if (!CheckTransing())   //非传送才执行下面的语句
            {
                //确定在空中还是在地面
                AdjustVelocity();
                ClimbCheck();
                if (desiredJump)
                {
                    Jump();
                    desiredJump = false;
                }
                Rotate();
            }

            velocity = Vector3.ClampMagnitude(velocity, 40);

            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            stepSinceLastGround += 1;
            stepSinceLastJump += 1;
            velocity = body.velocity;
            //当不在地面时执行贴近地面方法
            if (onGround/*在地上*/ || SnapToGround()/*可以贴近地面，也就是刚刚未经过跳跃，但是飞了出去*/
                || CheckSteepContacts()/*在斜面上，且被斜面包围*/)
            {
                stepSinceLastGround = 0;
                airJumps = 0;

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
            if(connectObj == preConnectObj)
            {
                Vector3 connectionMovment =
                    connectObj.transform.position - connectionWorldPostion;
                connectionVelocity = connectionMovment / Time.deltaTime;
            }
            connectionWorldPostion = connectObj.transform.position;
        }
        
        public void DesireJump()
        {
            desiredJump = true;
        }
        void Jump()
        {
            Vector3 jumpDirction;
            //确定跳跃方向
            if (onGround)
            {
                //在地上，直接根据接触方向
                jumpDirction = contactNormal;
            }
            else if (OnSteep)
            {
                //在斜面就用斜面方向，同时添加一个向上的方向，保证能够往上爬
                jumpDirction = (steepNormal + Vector3.up).normalized;
                airJumps = -1;
                //在斜面时根据斜面方向调整旋转方向
                LoadTargetY(steepNormal);
            }
            else if (airJumps < maxAirJumps)
            {
                //如果不在地上也不在斜面，并且可以在空中跳跃
                jumpDirction = Vector3.up;

                //根据期望调整方向
                LoadTargetY(desiredVelocity);
            }
            //不能跳，退出
            else return;

            //根据重力的大小来确定移动速度，因此重力可变了
            float jumpSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            float aligneSpeed = Vector3.Dot(velocity, jumpDirction);
            if (aligneSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - aligneSpeed, 0);
            }
            velocity += jumpDirction * jumpSpeed;
            airJumps++;

            //跳跃时刷新跳跃时间，保证在前面这段时间不会进行贴地
            stepSinceLastJump = 0;

        }

        
        Vector3 targetPos;      //传送到的目标点
        Vector3 direct;         //移动方向
        float maxSpeed = -1;         //最大速度

        /// <summary>
        /// 传送到特定点，在传送过程中需要停止其他力，只剩下向目标点的速度
        /// </summary>
        /// <param name="postion">目标位置</param>
        /// <param name="speed">速度</param>
        public void TransferToPosition(Transform postion, float speed)
        {
            //不允许反复传送
            if (maxSpeed > 0) {
                maxSpeed = -1;  //停止钩锁
                //body.useGravity = true;
                HookRopeManage.Instance.CloseHookRope();
                return;
            }
            if (postion == null) return;
            direct = (postion.position - transform.position).normalized;
            Vector3 nowDir = body.velocity.normalized;
            body.velocity = Mathf.Clamp01( Vector3.Dot(nowDir, direct) ) * body.velocity;

            HookRopeManage.Instance.LinkHookRope(postion.position, transform);

            maxSpeed = speed;
            targetPos = postion.position;
        }

        /// <summary>        /// 进行传送        /// </summary>
        /// <returns>是否在传送中</returns>
        private bool CheckTransing()
        {
            if (maxSpeed < 0) return false; 
            
            Vector3 dir = (targetPos - transform.position).normalized;
            if(Vector3.Dot(dir, direct) < 0.3)
            {
                maxSpeed = -1;
                //body.useGravity = true;
                //velocity *= 0.3f;
                //这里还是把速度衰减去掉了，看起来更连贯一点 ————Egg
                HookRopeManage.Instance.CloseHookRope();
                return false;
            }

            velocity += dir * maxSpeed;
            return true;
        }

        private void OnCollisionExit(Collision collision)
        {
            EvaluateCollision(collision);
        }

        ParticleDrawData drawData;

        private void OnCollisionEnter(Collision collision)
        {
            drawData.beginSpeed = collision.contacts[0].normal * 3;
            drawData.beginPos = collision.contacts[0].point;
            drawData.cubeOffset.x = 1.0f - collision.contacts[0].normal.x;
            drawData.cubeOffset.y = 1.0f - collision.contacts[0].normal.y;
            drawData.cubeOffset.z = 1.0f - collision.contacts[0].normal.z;
            ParticleNoiseFactory.Instance.DrawCube(drawData);
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
                    steepContractCount++;
                    steepNormal += normal;
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
            contactNormal = steepNormal = connectionVelocity = Vector3.zero;
            steepContractCount = 0;
            preConnectObj = connectObj;
            connectObj = null;
        }

        /// <summary>
        /// 用于贴近地面用的方法，减少移动时会飞出去的效果
        /// </summary>
        /// <returns>用来配合一些地面检测使用，因此有返回值</returns>
        bool SnapToGround()
        {
            //贴地行为只进行一次，同时用跳跃时间避免跳跃时贴地
            if (stepSinceLastGround > 1 || stepSinceLastJump <= 2)
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

        /// <summary>
        /// 检查斜面，当被陡峭面围在一起时算是在地上，此时将接触方向就是围绕的法线方向
        /// </summary>
        /// <returns>是否被斜面包围且无法移动</returns>
        bool CheckSteepContacts()
        {
            if (steepContractCount > 1)
            {
                steepNormal.Normalize();
                contactNormal = steepNormal;
                float upDot = Vector3.Dot(Vector3.up, steepNormal);
                if (upDot >= minGroundDot)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>    /// 确定该方向投影到该平面上的方向值，进行过标准化的    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }


        /// <summary>
        /// 攀爬检测，进行攀爬
        /// </summary>
        void ClimbCheck()
        {
            Vector3 beginPos, forward;
            if (playerInputSpace)
            {
                forward = playerInputSpace.forward;
            }
            else
            {
                forward = transform.forward;
            }
            forward.y = 0;
            forward = forward.normalized * climbData.x + Vector3.up * climbData.y;
            beginPos = transform.position + forward;
            Debug.DrawRay(beginPos, Vector3.down * 0.3f);

            if (Physics.Raycast(beginPos, Vector3.down, out RaycastHit hit, 0.5f, probeMask))
            {
                if (hit.normal.y > minGroundDot && velocity.y > 0)
                {
                    velocity += Vector3.up * climbData.z;
                }
            }
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
