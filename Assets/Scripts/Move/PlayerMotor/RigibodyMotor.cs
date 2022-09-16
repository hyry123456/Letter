
using UnityEngine;

namespace Motor
{

    /// <summary>
    /// ģ����˵��ƶ��࣬��һ���ǳ�����İ汾
    /// </summary>
    public class RigibodyMotor : MonoBehaviour
    {

        /// <summary>    /// ��ǰ�ٶ�, �����ٶ�, ����������ٶ�    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;
        /// <summary>    /// ���ٶ�    /// </summary>
        public float groundAcceleration = 10f, airAcceleration = 5;

        Rigidbody body;
        GameObject connectObj, preConnectObj;

        private bool desiredJump = false;

        public float jumpHeight = 2f;
        /// <summary>    /// �����Ծ����    /// </summary>
        public int maxAirJumps = 2;
        private int airJumps = 0;

        /// <summary>    /// �Ƿ��ڵ�����    /// </summary>
        private bool onGround = false;

        /// <summary>    /// �ֶ�OnSteep��ȷ���Ƿ���б����    /// </summary>
        private bool OnSteep => steepContractCount > 0;

        /// <summary>        /// ���������б�нǣ��Լ�¥����б�н�        /// </summary>
        [Range(0, 90)]
        public float maxGroundAngle = 25f, maxStairAngle = 25;
        private float minGroundDot = 0, minStairsDot = 0;

        /// <summary>
        /// �Ӵ���ķ��ߣ����������ƽ�����ߣ�����ȷ���ƶ���ķ����Լ���Ծ�ķ���
        /// </summary>
        Vector3 contactNormal, steepNormal;
        /// <summary>
        /// �������⣬������ﱻ���ڶ������У���Ҫ���м������ж��Ƿ�Ҫ��������Ծ
        /// </summary>
        int steepContractCount = 0;

        /// <summary>
        /// ����ȷ����ʱ�뿪�����ʱ��(stepSinceLastGround)���ڵ���ʱ���Ϊ0��
        /// ����ʱ��������֡ˢ��
        /// </summary>
        int stepSinceLastGround = 0;
        /// <summary>    /// ����ȷ����Ծ��ʱ�䣬����Ծʱ����㣬������֡ʱ��֡����    /// </summary>
        int stepSinceLastJump = 0;

        /// <summary>    /// �ж�ʱ��������ص��ٶȣ�����ٶȴ��ڸ�ֵ������������    /// </summary>
        [SerializeField, Range(0, 100f)]
        float maxSnapSpeed = 100f;
        /// <summary>    /// ���صļ�����    /// </summary>
        [SerializeField, Range(0, 10f)]
        float probeDistance = 3f;

        /// <summary>    /// ���ؼ��Ĳ㣬�Լ�¥�ݼ���    /// </summary>
        [SerializeField]
        LayerMask probeMask, stairsMask = -1;

        /// <summary>    /// ����ռ䣬�������ݸÿռ�������ģ���ƶ�    /// </summary>
        [SerializeField]
        Transform playerInputSpace;

        Info.CharacterInfo characterInfo;

        Vector3 connectionWorldPostion;

        private bool IsWaiting
        {
            get
            {
                if (waitTime > 0) return true;
                return false;
            }
        }

        void Start()
        {
            velocity = Vector3.zero;
            body = GetComponent<Rigidbody>();
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDot = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
            characterInfo = GetComponent<Info.CharacterInfo>();
            if (characterInfo == null) Debug.LogError("��ɫ��ϢΪ��");
        }



        public void Move(float horizontal, float vertical)
        {
            //ʵ���Ͼ��Ǽ��ٶ��Ƕ�ֵ����ɫ����ֵ��Ŀ���ٶȣ���֡���ٶȱ仯ΪĿ���ٶ�
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

            //�����㣬��ֹ���뱻�ر�
            desiredJump |= Input.GetButtonDown("Jump");

        }

        private void FixedUpdate()
        {
            //�������ݣ���������һ������֡�����ݽ��и���֮���
            UpdateState();
            if (IsWaiting) return;
            //ȷ���ڿ��л����ڵ���
            AdjustVelocity();


            if (desiredJump)
            {
                Jump();
                desiredJump = false;
            }

            Rotate();

            body.velocity = velocity;
            ClearState();
        }


        void UpdateState()
        {
            stepSinceLastGround += 1;
            stepSinceLastJump += 1;
            velocity = body.velocity;
            //�����ڵ���ʱִ���������淽��
            if (onGround/*�ڵ���*/ || SnapToGround()/*�����������棬Ҳ���Ǹո�δ������Ծ�����Ƿ��˳�ȥ*/
                || CheckSteepContacts()/*��б���ϣ��ұ�б���Χ*/)
            {
                stepSinceLastGround = 0;
                airJumps = 0;

                contactNormal.Normalize();
                LoadTargetY(desiredVelocity);
            }
            else
                contactNormal = Vector3.up;

            UpdateWeakTime();

            if (connectObj && connectObj.tag == "CheckMove")
            {
                UpdateConnectionState();
            }
        }

        void UpdateConnectionState()
        {
            //ֻ��������ͬ�����б�Ҫ����
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
            //ȷ����Ծ����
            if (onGround)
            {
                //�ڵ��ϣ�ֱ�Ӹ��ݽӴ�����
                jumpDirction = contactNormal;
            }
            else if (OnSteep)
            {
                //��б�����б�淽��ͬʱ���һ�����ϵķ��򣬱�֤�ܹ�������
                jumpDirction = (steepNormal + Vector3.up).normalized;
                airJumps = -1;
                //��б��ʱ����б�淽�������ת����
                LoadTargetY(steepNormal);
            }
            else if (airJumps < maxAirJumps)
            {
                //������ڵ���Ҳ����б�棬���ҿ����ڿ�����Ծ
                jumpDirction = Vector3.up;

                //����������������
                LoadTargetY(desiredVelocity);
            }
            //���������˳�
            else return;

            //���������Ĵ�С��ȷ���ƶ��ٶȣ���������ɱ���
            float jumpSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            float aligneSpeed = Vector3.Dot(velocity, jumpDirction);
            if (aligneSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - aligneSpeed, 0);
            }
            velocity += jumpDirction * jumpSpeed;
            airJumps++;

            //��Ծʱˢ����Ծʱ�䣬��֤��ǰ�����ʱ�䲻���������
            stepSinceLastJump = 0;

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
                    //��֤����ж���Ӵ���ʱ�ܹ���ȷ�Ļ�ȡ���ߣ�����ü�
                    contactNormal += normal;
                    connectObj = collision.gameObject;
                }
                //�������ƶ����ƣ����Ǳ��⳹�׵Ĵ�ֱ��
                else if (upDot > -0.01f)
                {
                    steepContractCount++;
                    steepNormal += normal;
                    connectObj = collision.gameObject;
                }

            }
        }


        /// <summary>
        /// �����ƶ�����������֤�ƶ��ķ���������ƽ���
        /// </summary>
        void AdjustVelocity()
        {
            //��Ϊ�ٶ��õ�Ҳ���������꣬����ƶ�ʱͶӰҲ���������������꣬����right����x�ᣬfoward����Y��
            //��1��0��0ͶӰ���Ӵ�ƽ���ϣ�
            //Vector3 xAixs = ProjectOnContactPlane(Vector3.right).normalized;
            Vector3 xAixs = ProjectDirectionOnPlane(Vector3.right, contactNormal);
            //��0��0��1ͶӰ���Ӵ�ƽ����
            //Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
            Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;
            //ȷ��ʵ���������ƽ���ϵ�X�ƶ�ֵ
            float currentX = Vector3.Dot(relativeVelocity, xAixs);
            //ȷ��ʵ���������ƽ���ϵ�Z�ƶ�ֵ
            float currentZ = Vector3.Dot(relativeVelocity, zAxis);

            float acceleration = onGround ? groundAcceleration : airAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            //ȷ�����������õ����ƶ�ֵ
            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

            //�ƶ�Ҫ�������ƽ��ķ������ƶ�����˸���ʵ��ֵ������ֵ�Ĳ�ȷ��Ҫ���ӵ��ٶȴ�С��
            //Ȼ�����ͶӰ���������Xֵ�Լ�Zֵȷ�������ƶ�ֵ
            velocity += xAixs * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        /// <summary>
        /// ������ݣ���һЩ���ݹ�Ϊ��ʼ��
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
        /// �������������õķ����������ƶ�ʱ��ɳ�ȥ��Ч��
        /// </summary>
        /// <returns>�������һЩ������ʹ�ã�����з���ֵ</returns>
        bool SnapToGround()
        {
            //������Ϊֻ����һ�Σ�ͬʱ����Ծʱ�������Ծʱ����
            if (stepSinceLastGround > 1 || stepSinceLastJump <= 2)
            {
                return false;
            }
            float speed = velocity.magnitude;
            //��������ٶȣ�����������
            if (speed > maxSnapSpeed)
                return false;

            RaycastHit hit;
            if (!Physics.Raycast(body.position, -Vector3.up, out hit, probeDistance, probeMask))
                return false;

            float upDot = Vector3.Dot(Vector3.up, hit.normal);
            //������е��治����Ϊ����վ�����棬�Ͳ���������
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
                return false;

            contactNormal = hit.normal;

            //ȷ���ٶ��ڷ����ϵĴ�С
            float dot = Vector3.Dot(velocity, hit.normal);
            //��ֻ֤���ٶȳ���ʱ�Ż�����ѹ��������������ٶ�
            if (dot > 0)
            {
                //�����ٶȵĴ�С��ƽ����ѹ
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectObj = hit.collider.gameObject;
            return true;
        }

        float GetMinDot(int layer)
        {
            //�ж���¥�ݻ�����������
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDot : minStairsDot;
        }

        /// <summary>
        /// ���б�棬����������Χ��һ��ʱ�����ڵ��ϣ���ʱ���Ӵ��������Χ�Ƶķ��߷���
        /// </summary>
        /// <returns>�Ƿ�б���Χ���޷��ƶ�</returns>
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

        /// <summary>    /// ȷ���÷���ͶӰ����ƽ���ϵķ���ֵ�����й���׼����    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }


        /// <summary>
        /// ���ö�������
        /// </summary>
        //void SetAnimatorValue()
        //{
        //    if (playerInfo == null) return;
        //    Info.PlayerAction playerAction = playerInfo.characterAction as Info.PlayerAction;
        //    if (Mathf.Abs(velocity.x) >= 0.1f || Mathf.Abs(velocity.z) >= 0.1f)
        //        playerAction.Move = true;
        //    else playerAction.Move = false;
        //    playerAction.Jump = !onGround;
        //}

        public float waitTime;
        Common.INonReturnAndNonParam onWaitEnd;


        bool UpdateWeakTime()
        {
            if (waitTime < 0) return false;
            waitTime -= Time.fixedDeltaTime;
            if(waitTime < 0)
            {
                if(onWaitEnd != null)
                {
                    onWaitEnd();
                    onWaitEnd = null;
                }
            }
            return true;
        }


        /// <summary>
        /// ������⣬��������
        /// </summary>
        void ClimbCheck()
        {

            //if (Physics.Raycast(ClimbCheckPoint.position, Vector3.down, out RaycastHit hit, 0.1f, probeMask))
            //{
            //    if (hit.normal.y > minGroundDot)
            //    {
            //        rigidbody.useGravity = false;
            //        rigidbody.velocity = Vector3.zero;
            //        waitTime = 0.8f;
            //        if (playerInfo != null)
            //        {
            //            playerInfo.allSystemPort.AnimaControl.StartRootAnimate();
            //            Info.PlayerAction playerAction = playerInfo.characterAction as Info.PlayerAction;
            //            playerAction.Climb = true;
            //            onWaitEnd = () =>
            //            {
            //                rigidbody.position = hit.point;
            //                playerInfo.allSystemPort.AnimaControl.StopRootAnimate();
            //                rigidbody.useGravity = true;
            //                playerAction.Climb = false;
            //            };
            //        }
            //    }
            //}
        }

        float targetRotateY;

        /// <summary>       /// ��תģ��        /// </summary>
        void Rotate()
        {
            Vector3 angle = transform.eulerAngles;
            //�ƶ��Ƕ�
            angle.y = Mathf.MoveTowardsAngle(angle.y, targetRotateY, characterInfo.rotateSpeed);

            transform.eulerAngles = angle;
        }

        /// <summary>
        /// ������תĿ��Yֵ��ֻ���ڵ���ʱ���ߵ�ǿʱ�Ż���ת
        /// </summary>
        /// <param name="desire">�����ƶ������緽��</param>
        void LoadTargetY(Vector3 desire)
        {
            Vector2 vector2 = new Vector2(desire.x, desire.z);
            //̫С�Ͳ�������������ת��������
            if (Mathf.Abs(vector2.y) < 0.0001) return;
            targetRotateY = GetAngle(vector2.normalized);
        }

        /// <summary>
        /// �����ƶ��Ĳ��ֵ�ж���ת�Ƕȣ�ע�⴫��ֵҪ��׼����
        /// ����Ϊ��̬��Ϊ�����������Ҫ�õ��������ݣ����ֻ�ÿ���һ��������͹���
        /// </summary>
        static float GetAngle(Vector2 direction)
        {
            //ͨ�������Һ����������ת������ƶ���������Ҫ��yֵ�Ƕ�
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            //�ж����ıߣ�Ҳ����˳ʱ�뻹����ʱ��
            return direction.x < 0f ? 360f - angle : angle;
        }


    }
}
