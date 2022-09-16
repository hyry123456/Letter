
using UnityEngine;
using Common.ResetInput;

namespace Control
{
    public class PlayerControl : MonoBehaviour
    {
        private static PlayerControl instance;
        private Motor.RigibodyMotor motor;

        /// <summary>        /// 主角的摄像机，目前是直接挂摄像机上来        /// </summary>
        public GameObject view;
        private Motor.OribitCamera oribitCamera;
        

        public static PlayerControl Instance {
            get
            {
                if(instance == null)
                    instance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
                return instance; 
            }
        }

        public string verticalName = "Vertical";
        public string horizontalName = "Horizontal";
        public string jumpName = "Jump";
        public string setName = "Setting";
        public string begName = "Bag";
        public string interacteName = "Interaction";
        public string rollName = "Roll";

        private void Awake()
        {
            instance = this;
        }

        protected void Start()
        {
            motor = GetComponent<Motor.RigibodyMotor>();

            oribitCamera = view.GetComponent<Motor.OribitCamera>();
        }

        /// <summary>
        /// 时时刷新的控制属性的存放位置
        /// </summary>
        private void Update()
        {
            float xMouse = Input.GetAxis("Mouse X");
            float yMouse = -Input.GetAxis("Mouse Y");
            oribitCamera.SetCameraInput(yMouse, xMouse);
        }
        /// <summary>
        /// 物理帧刷新的属性计算位置，一些没有必要逐帧计算的可以在这里进行计算
        /// </summary>
        private void FixedUpdate()
        {
            //先获取这些，之后补充其他
            float vertical = MyInput.Instance.GetAsis(verticalName);
            float horizontal = MyInput.Instance.GetAsis(horizontalName);
            bool jump = MyInput.Instance.GetButtonDown(jumpName);


            motor.Move(horizontal, vertical);
            if (jump)
                motor.DesireJump();
        }

        /// <summary>
        /// 获得主角看向的位置，也就是摄像机前方
        /// </summary>
        public Vector3 GetLookatDir()
        {
            if (Camera.main == null) return Vector3.zero;
            return Camera.main.transform.forward;
        }

        /// <summary>
        /// 获得摄像机的世界坐标
        /// </summary>
        public Vector3 GetCameraPos()
        {
            Camera camera = Camera.main;
            if (camera != null)
                return camera.transform.position;
            else return Vector3.zero;
        }

        ///// <summary>
        ///// 获得移动基类
        ///// </summary>
        //public Motor.Motor GetMotor()
        //{
        //    return motor;
        //}
    }
}